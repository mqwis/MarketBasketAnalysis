using System.ComponentModel.DataAnnotations;

namespace MarketBasketAnalysis.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}