namespace FinancialManagementMVC.DTOs
{
    public class UserRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

     public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class BankAccountDto
    {
        public string AccountNumber { get; set; }
        public decimal   Balance { get; set; }
        public string Currency { get; set; }
        //public int UserId { get; set; }

        public int Id { get; set; }
        
        
    }

    public class TransactionDto
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public int BankAccountId { get; set; }
    }

    public class TransferDto
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string Description { get; set; }
    }
}