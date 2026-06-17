using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public int ProductID { get; set; }

    [Required]
    [StringLength(150)]
    public string ProductName { get; set; } = string.Empty;

    public int CategoryID { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;       

    [Required]
    public string Subcategory { get; set; } = string.Empty;    

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }                            

    public bool IsActive { get; set; } = true;
}