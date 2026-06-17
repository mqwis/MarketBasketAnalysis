using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketBasketAnalysis.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        [StringLength(30)]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; } = 0;

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}