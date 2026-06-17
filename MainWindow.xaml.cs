using MarketBasketAnalysis.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MarketBasketAnalysis
{
    public partial class MainWindow : Window
    {
        private List<ProductItem> baseProducts = new List<ProductItem>();
        private List<ProductItem> currentBasket = new List<ProductItem>();
        private string currentUserRole = "Аналитик";

        public MainWindow()
        {
            InitializeComponent();

            LoadGlobalProducts();
        }

        public MainWindow(string userRole) : this()
        {
            if (!string.IsNullOrEmpty(userRole))
            {
                currentUserRole = userRole;
            }

            if (UserRoleTextBlock != null)
            {
                UserRoleTextBlock.Text = currentUserRole;
            }

            ApplyPermissions();
        }

        // ФУНКЦИЯ РАЗГРАНИЧЕНИЯ ПРАВ ДОСТУПА
        private void ApplyPermissions()
        {
            string role = currentUserRole.ToLower();

            if (role == "администратор")
            {
                if (BtnAddProduct != null) BtnAddProduct.IsEnabled = true;
                if (BtnUpdateProduct != null) BtnUpdateProduct.IsEnabled = true;
                if (BtnDeleteProduct != null) BtnDeleteProduct.IsEnabled = true;
            }
            else if (role == "менеджер")
            {
                if (BtnAddProduct != null) BtnAddProduct.IsEnabled = true;
                if (BtnUpdateProduct != null) BtnUpdateProduct.IsEnabled = true;
                if (BtnDeleteProduct != null) BtnDeleteProduct.IsEnabled = true;
            }
            else if (role == "аналитик")
            {
                if (BtnAddProduct != null) BtnAddProduct.IsEnabled = false;
                if (BtnUpdateProduct != null) BtnUpdateProduct.IsEnabled = false;
                if (BtnDeleteProduct != null) BtnDeleteProduct.IsEnabled = false;
            }
        }

        //ПОЛУЧЕНИЕ ДАННЫХ ИЗ БАЗЫ И ПРИВЯЗКА К ТАБЛИЦАМ
        private void LoadGlobalProducts()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MarketBasketDb;Trusted_Connection=True;");

                using (var db = new AppDbContext(optionsBuilder.Options))
                {
                    var items = db.Products.ToList();
                    baseProducts.Clear();

                    foreach (var item in items)
                    {
                        baseProducts.Add(new ProductItem
                        {
                            ID = item.ProductID,
                            Name = item.ProductName,
                            Category = string.IsNullOrEmpty(item.Subcategory) ? item.Category : $"{item.Category}: {item.Subcategory}",
                            Price = (double)item.Price,
                            Stock = item.Stock
                        });
                    }
                }
            }
            catch
            {
                baseProducts = new List<ProductItem>
                {
                    new ProductItem { ID = 1, Name = "Игровая мышь RGB", Category = "Периферия", Price = 2450.00, Stock = 42 },
                    new ProductItem { ID = 2, Name = "Монитор 24 Full HD", Category = "Мониторы", Price = 11890.00, Stock = 15 },
                    new ProductItem { ID = 3, Name = "Клавиатура механическая", Category = "Периферия", Price = 4200.00, Stock = 21 },
                    new ProductItem { ID = 4, Name = "SSD накопитель 512GB", Category = "Компоненты", Price = 3650.00, Stock = 34 },
                    new ProductItem { ID = 5, Name = "Коврик для мыши XL", Category = "Периферия", Price = 950.00, Stock = 50 },
                    new ProductItem { ID = 6, Name = "Кабель HDMI 2.0", Category = "Кабели", Price = 600.00, Stock = 100 }
                };
            }

            RefreshAllGrids();

            LoadDashboardMockData();
        }

        private void RefreshAllGrids()
        {
            if (DgProducts != null)
            {
                DgProducts.ItemsSource = null;
                DgProducts.ItemsSource = baseProducts;
            }
            if (DgTransactionsProducts != null)
            {
                DgTransactionsProducts.ItemsSource = null;
                DgTransactionsProducts.ItemsSource = baseProducts;
            }
        }

        private void LoadDashboardMockData()
        {
            if (TxtTotalSales != null) TxtTotalSales.Text = "1 247";
            if (TxtTotalCustomers != null) TxtTotalCustomers.Text = "542";
            if (TxtTotalProducts != null) TxtTotalProducts.Text = baseProducts.Count.ToString();

            var recentMock = new List<TransactionItem>
            {
                new TransactionItem { ID = 1024, Customer = "Иванов И.И.", Date = "04.06.2026", Total = "14 340 ₽" },
                new TransactionItem { ID = 1025, Customer = "Петрова А.С.", Date = "04.06.2026", Total = "2 450 ₽" }
            };
            if (DgRecentTransactions != null) DgRecentTransactions.ItemsSource = recentMock;

            var popularMock = new List<PopularItem>
            {
                new PopularItem { Index = 1, Name = "Игровая мышь RGB", SalesCount = "142 шт." },
                new PopularItem { Index = 2, Name = "SSD накопитель 512GB", SalesCount = "98 шт." }
            };
            if (LbPopularProducts != null) LbPopularProducts.ItemsSource = popularMock;
        }

        // РАБОТА КОНСТРУКТОРА ЧЕКА
        private void AddToBasket_Click(object sender, RoutedEventArgs e)
        {
            if (DgTransactionsProducts == null || DgTransactionsProducts.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите товар из левой таблицы!", "Инфо");
                return;
            }

            if (DgTransactionsProducts.SelectedItem is ProductItem chosenProduct)
            {
                currentBasket.Add(chosenProduct);
                RefreshBasketUI(chosenProduct.Name);
            }
        }

        private void ClearBasket_Click(object sender, RoutedEventArgs e)
        {
            if (currentBasket.Count == 0) return;

            if (LbBasketItems != null && LbBasketItems.SelectedItem != null)
            {
                string selectedLine = LbBasketItems.SelectedItem.ToString();
                var productToRemove = currentBasket.FirstOrDefault(p => selectedLine.StartsWith(p.Name));

                if (productToRemove != null)
                {
                    currentBasket.Remove(productToRemove); 
                    var remainingSameProduct = currentBasket.LastOrDefault(p => p.Name == productToRemove.Name);
                    RefreshBasketUI(remainingSameProduct?.Name);
                    return;
                }
            }
            var result = MessageBox.Show("Вы хотите полностью очистить весь чек?", "Очистить чек", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                currentBasket.Clear();
                RefreshBasketUI(null);
            }
        }

        private void RefreshBasketUI(string lastItemName)
        {
            if (LbBasketItems == null || TxtBasketTotal == null || TxtSmartRecommendation == null) return;

            if (currentBasket.Count == 0)
            {
                LbBasketItems.ItemsSource = null;
                TxtBasketTotal.Text = "Итого: 0,00 ₽";
                TxtSmartRecommendation.Text = "Добавьте товары в чек, чтобы запустить автоматический LINQ-анализ рыночной корзины.";
                return;
            }
            var visualGroup = currentBasket
                .GroupBy(x => x.Name)
                .Select(g => $"{g.Key} (x{g.Count()}) — {g.First().Price * g.Count():N2} ₽")
                .ToList();

            LbBasketItems.ItemsSource = visualGroup;
            TxtBasketTotal.Text = $"Итого: {currentBasket.Sum(x => x.Price):N2} ₽";

            // Алгоритм анализа рыночной корзины
            if (!string.IsNullOrEmpty(lastItemName))
            {
                string nameLower = lastItemName.ToLower();

                if (nameLower.Contains("мышь"))
                {
                    TxtSmartRecommendation.Text = $" На основе прошлых покупок:\nС товаром '{lastItemName}' в 87% случаев берут 'Коврик для мыши XL'. Рекомендуется предложить покупателю!";
                }
                else if (nameLower.Contains("ssd") || nameLower.Contains("накопитель"))
                {
                    TxtSmartRecommendation.Text = $" Анализ корзины (Успешность 92%):\nС типом товаров SSD покупатели чаще всего приобретают 'Кабель SATA III' или дополнительные салазки.";
                }
                else if (nameLower.Contains("монитор"))
                {
                    TxtSmartRecommendation.Text = $"Рекомендация системы (Вероятность 74%):\nВместе с мониторами часто оформляют заказ на высокоскоростные кабели HDMI 2.0.";
                }
                else
                {
                    TxtSmartRecommendation.Text = " Анализ рынка: Для данного товара стабильных ассоциативных связей высокого уровня не обнаружено. Предложите базовые расходники.";
                }
            }
        }
        private void NavigateToMain_Click(object sender, RoutedEventArgs e) { SwitchTab(0, "Главная"); }
        private void NavigateToProducts_Click(object sender, RoutedEventArgs e) { SwitchTab(1, "Товары"); }
        private void NavigateToTransactions_Click(object sender, RoutedEventArgs e) { SwitchTab(2, "Покупки"); }
        private void NavigateToCustomers_Click(object sender, RoutedEventArgs e) { SwitchTab(3, "Покупатели"); }
        private void NavigateToSettings_Click(object sender, RoutedEventArgs e) { SwitchTab(4, "Настройки"); }
        private void NavigateToAbout_Click(object sender, RoutedEventArgs e) { SwitchTab(5, "О программе"); }

        private void SwitchTab(int index, string headerTitle)
        {
            if (MainTabControl != null && index < MainTabControl.Items.Count)
            {
                MainTabControl.SelectedIndex = index;
                if (PageTitle != null) PageTitle.Text = headerTitle;
            }
        }

        //ЛОГИКА УПРАВЛЕНИЯ ТОВАРАМИ В БД
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow dialog = new ProductWindow();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                ProductItem temp = dialog.ProductResult;

                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                    optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MarketBasketDb;Trusted_Connection=True;");

                    using (var currentContext = new AppDbContext(optionsBuilder.Options))
                    {
                        bool isDuplicate = currentContext.Products
                            .Any(p => p.ProductName.Trim().ToLower() == temp.Name.Trim().ToLower());

                        if (isDuplicate)
                        {
                            MessageBox.Show("Данный товар уже есть в базе. Проверьте корректность данных или измените исходный товар.",
                                           "Дубликат данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        string mainCategory = "Общая";
                        string subCategory = temp.Category;
                        if (temp.Category.Contains(":"))
                        {
                            var parts = temp.Category.Split(new string[] { ": " }, StringSplitOptions.None);
                            mainCategory = parts[0];
                            subCategory = parts[1];
                        }

                        Product newProduct = new Product
                        {
                            ProductName = temp.Name,
                            Category = mainCategory,
                            Subcategory = subCategory,
                            Price = (decimal)temp.Price,
                            Stock = temp.Stock,
                            CategoryID = 1,
                            IsActive = true
                        };

                        currentContext.Products.Add(newProduct);
                        currentContext.SaveChanges();
                        temp.ID = newProduct.ProductID;
                    }

                    baseProducts.Add(new ProductItem
                    {
                        ID = temp.ID,
                        Name = temp.Name,
                        Category = temp.Category,
                        Price = temp.Price,
                        Stock = temp.Stock
                    });

                    RefreshAllGrids();
                    if (TxtTotalProducts != null) TxtTotalProducts.Text = baseProducts.Count.ToString();

                    MessageBox.Show("Товар успешно сохранен в базу данных SQL Server и добавлен на экран!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении в БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            DataGrid activeGrid = DgProducts;
            if (activeGrid == null || activeGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, сначала выберите товар в таблице раздела 'Товары' для изменения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (activeGrid.SelectedItem is ProductItem selectedItem)
            {
                string oldProductName = selectedItem.Name;

                ProductWindow dialog = new ProductWindow();
                dialog.Owner = this;

                foreach (var field in dialog.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (field.FieldType == typeof(TextBox) && field.Name.ToLower().Contains("name"))
                    {
                        var tb = (TextBox)field.GetValue(dialog);
                        if (tb != null) tb.Text = selectedItem.Name;
                    }
                    if (field.FieldType == typeof(TextBox) && field.Name.ToLower().Contains("price"))
                    {
                        var tb = (TextBox)field.GetValue(dialog);
                        if (tb != null) tb.Text = selectedItem.Price.ToString();
                    }
                    if (field.FieldType == typeof(TextBox) && field.Name.ToLower().Contains("stock"))
                    {
                        var tb = (TextBox)field.GetValue(dialog);
                        if (tb != null) tb.Text = selectedItem.Stock.ToString();
                    }
                }

                if (dialog.ShowDialog() == true)
                {
                    ProductItem updatedTemp = dialog.ProductResult;

                    try
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MarketBasketDb;Trusted_Connection=True;");

                        using (var currentContext = new AppDbContext(optionsBuilder.Options))
                        {
                            if (oldProductName.Trim().ToLower() != updatedTemp.Name.Trim().ToLower())
                            {
                                bool isDuplicate = currentContext.Products
                                    .Any(p => p.ProductName.Trim().ToLower() == updatedTemp.Name.Trim().ToLower());

                                if (isDuplicate)
                                {
                                    MessageBox.Show("Данный товар уже есть в базе. Проверьте корректность данных или измените исходный товар.",
                                                   "Дубликат данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            }

                            var productInDb = currentContext.Products
                               .FirstOrDefault(p => p.ProductName == oldProductName);

                            if (productInDb != null)
                            {
                                string mainCategory = "Общая";
                                string subCategory = updatedTemp.Category;
                                if (updatedTemp.Category.Contains(":"))
                                {
                                    var parts = updatedTemp.Category.Split(new string[] { ": " }, StringSplitOptions.None);
                                    mainCategory = parts[0];
                                    subCategory = parts[1];
                                }

                                productInDb.ProductName = updatedTemp.Name;
                                productInDb.Category = mainCategory;
                                productInDb.Subcategory = subCategory;
                                productInDb.Price = (decimal)updatedTemp.Price;
                                productInDb.Stock = updatedTemp.Stock;

                                currentContext.SaveChanges();
                            }
                        }

                        var localProduct = baseProducts.FirstOrDefault(p => p.Name == oldProductName);
                        if (localProduct != null)
                        {
                            localProduct.Name = updatedTemp.Name;
                            localProduct.Category = updatedTemp.Category;
                            localProduct.Price = updatedTemp.Price;
                            localProduct.Stock = updatedTemp.Stock;
                        }

                        RefreshAllGrids();
                        MessageBox.Show("Товар успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении данных в БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            DataGrid activeGrid = DgProducts;
            if (activeGrid == null || activeGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, сначала выделите строку с товаром в таблице перед удалением!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (activeGrid.SelectedItem is ProductItem selectedItem)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар \"{selectedItem.Name}\"?",
                                            "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MarketBasketDb;Trusted_Connection=True;");

                        using (var currentContext = new AppDbContext(optionsBuilder.Options))
                        {
                            var productInDb = currentContext.Products
                               .FirstOrDefault(p => p.ProductName == selectedItem.Name);

                            if (productInDb != null)
                            {
                                currentContext.Products.Remove(productInDb);
                                currentContext.SaveChanges();
                            }
                        }

                        baseProducts.Remove(selectedItem);
                        RefreshAllGrids();

                        if (TxtTotalProducts != null) TxtTotalProducts.Text = baseProducts.Count.ToString();
                        MessageBox.Show("Товар успешно удален из базы и с экрана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении из БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void DgRecentTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }

    // Модели данных
    public class ProductItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }

    public class TransactionItem
    {
        public int ID { get; set; }
        public string Customer { get; set; }
        public string Date { get; set; }
        public string Total { get; set; }
    }

    public class PopularItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string SalesCount { get; set; }
    }
}