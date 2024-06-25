// Data/FinancialContext.cs
using Microsoft.EntityFrameworkCore;
using FinancialManagementMVC.Models;

namespace FinancialManagementMVC.Data
{
    public class FinancialContext : DbContext
    {
        public FinancialContext(DbContextOptions<FinancialContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Transfer> Transfers { get; set; }  // Add this


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.BankAccounts)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            // Other model configurations if necessary

            modelBuilder.Entity<BankAccount>()
            .HasMany(b => b.Transactions)
            .WithOne(t => t.BankAccount)
            .HasForeignKey(t => t.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Transfer>()
            .HasOne(t => t.FromAccount)
            .WithMany()  // Assuming no navigation property back to Transfers from BankAccount
            .HasForeignKey(t => t.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion if there are associated transfers

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.ToAccount)
                .WithMany()  // Assuming no navigation property back to Transfers from BankAccount
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion if there are associated transfers
        }
    }
}