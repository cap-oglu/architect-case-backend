namespace FinancialManagementMVC.Models;
public class Transfer
{
    public int Id { get; set; }
    public int? FromAccountId { get; set; }  // FK to BankAccount
    public int? ToAccountId { get; set; }    // FK to BankAccount
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public BankAccount FromAccount { get; set; }
    public BankAccount ToAccount { get; set; }
}