// Models/BankAccount.cs
using System.ComponentModel.DataAnnotations;

namespace FinancialManagementMVC.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        [Required]
        public string AccountNumber { get; set; } // Unique identifier for the bank account
        public decimal Balance { get; set; } // Changed from double to decimal
        [Required]
        public string Currency { get; set; }
        public int UserId { get; set; } // Foreign key
        public User User { get; set; } // Navigation property
        public List<Transaction>? Transactions { get; set; } // Navigation property
        
    }
}
