using System.ComponentModel.DataAnnotations;

namespace FinancialManagementMVC.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }

        // Add Foreign Key for BankAccount
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; } // Navigation property
    }
}
