using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MarketBasketAnalysis
{
    public partial class ProductWindow : Window
    {
        public ProductItem ProductResult { get; set; }

        private Dictionary<string, List<string>> categoriesData = new Dictionary<string, List<string>>
        {
            { "Компьютеры и комплектующие", new List<string> { "Настольные ПК", "Ноутбуки", "Процессоры", "Видеокарты", "Материнские платы", "SSD и жёсткие диски", "Охлаждение" } },
            { "Комплектующие для ноутбуков", new List<string> { "Аккумуляторы", "Клавиатуры для ноутбуков", "Матрицы", "Кулеры", "Док-станции" } },
            { "Периферия и аксессуары", new List<string> { "Мониторы", "Принтеры и МФУ", "Клавиатуры и мыши", "Наушники и гарнитуры", "Флешки и внешние диски" } },
            { "Мобильные устройства", new List<string> { "Смартфоны", "Планшеты", "Умные часы", "Чехлы и стекла", "Зарядные устройства" } },
            { "Телевизоры и аудио-видео", new List<string> { "Телевизоры", "Проекторы", "Саундбары", "Портативные колонки" } },
            { "Бытовая техника", new List<string> { "Холодильники", "Стиральные машины", "Микроволновки", "Пылесосы", "Мелкая кухонная техника" } },
            { "Климатическая техника", new List<string> { "Кондиционеры", "Обогреватели", "Увлажнители воздуха", "Вентиляторы" } },
            { "Фото и видеотехника", new List<string> { "Фотоаппараты", "Экшн-камеры", "Объективы", "Штативы" } },
            { "Игровые товары", new List<string> { "Игровые консоли", "Игры для ПК/консолей", "Геймпады", "Игровые кресла" } },
            { "Автоэлектроника", new List<string> { "Видеорегистраторы", "Радар-детекторы", "Автоакустика" } },
            { "Сети и безопасность", new List<string> { "Роутеры и модемы", "Коммутаторы", "Камеры видеонаблюдения" } },
            { "Инструменты", new List<string> { "Электроинструменты", "Измерительные приборы" } },
            { "Умный дом и гаджеты", new List<string> { "Умные лампы и розетки", "Датчики", "Центральные хабы" } },
            { "Расходные материалы", new List<string> { "Батарейки", "Лампочки", "Картриджи и тонеры", "Бумага для печати" } }
        };

        public ProductWindow()
        {
            InitializeComponent();
            TxtWinTitle.Text = "Добавление товара";
            InitializeCategories();
        }

        public ProductWindow(ProductItem existingProduct) : this()
        {
            TxtWinTitle.Text = "Редактирование товара";
            TbName.Text = existingProduct.Name;
            TbPrice.Text = existingProduct.Price.ToString();
            TbStock.Text = existingProduct.Stock.ToString();

            ParseAndSelectCategory(existingProduct.Category);
        }

        private void InitializeCategories()
        {
            CbCategory.ItemsSource = categoriesData.Keys;
            CbCategory.SelectedIndex = 0;
        }

        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCategory.SelectedItem is string selectedCategory)
            {
                CbSubCategory.ItemsSource = categoriesData[selectedCategory];
                CbSubCategory.SelectedIndex = 0;
            }
        }

        private void ParseAndSelectCategory(string fullCategoryString)
        {
            if (string.IsNullOrEmpty(fullCategoryString)) return;

            string[] parts = fullCategoryString.Split(new string[] { ": " }, StringSplitOptions.None);
            string mainCat = parts[0];
            string subCat = parts.Length > 1 ? parts[1] : string.Empty;

            if (categoriesData.ContainsKey(mainCat))
            {
                CbCategory.SelectedItem = mainCat;
                if (categoriesData[mainCat].Contains(subCat))
                {
                    CbSubCategory.SelectedItem = subCat;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CbCategory.SelectedItem == null || CbSubCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию и подкатегорию товара!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!double.TryParse(TbPrice.Text.Replace(".", ","), out double price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(TbStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string combinedCategory = $"{CbCategory.SelectedItem}: {CbSubCategory.SelectedItem}";
            ProductResult = new ProductItem
            {
                Name = TbName.Text.Trim(),
                Category = combinedCategory,
                Price = price,
                Stock = stock
            };
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}