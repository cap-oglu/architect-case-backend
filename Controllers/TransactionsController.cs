using FinancialManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialManagementMVC.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FinancialManagementMVC.DTOs;

namespace FinancialManagementMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly FinancialContext _context;

        public TransactionsController(FinancialContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            var userTransactions = await _context.Transactions
                .Where(t => _context.BankAccounts.Any(b => b.Id == t.BankAccountId && b.UserId == userId))
                .ToListAsync();
            return userTransactions;
        }


        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            var transaction = await _context.Transactions
                .Include(t => t.BankAccount)
                .SingleOrDefaultAsync(t => t.Id == id && t.BankAccount.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }
            return transaction;
        }


        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(TransactionDto transactionDto)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            if (!_context.BankAccounts.Any(b => b.Id == transactionDto.BankAccountId && b.UserId == userId))
            {
                return BadRequest("Invalid bank account.");
            }

            // Ensure the bank account exists and belongs to the user
            var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.Id == transactionDto.BankAccountId && b.UserId == userId);
            if (bankAccount == null)
            {
                return BadRequest("Invalid bank account.");
            }

            bankAccount.Balance -= transactionDto.Amount;

            var transaction = new Transaction
            {
                BankAccountId = transactionDto.BankAccountId,
                Amount = transactionDto.Amount,
                Description = transactionDto.Description,
                TransactionDate = transactionDto.TransactionDate
            };

            _context.Transactions.Add(transaction);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Unable to record transaction: " + ex.Message);
            }
            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }


        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, TransactionDto transactionDto)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            // Fetch the transaction to update
            var transaction = await _context.Transactions.Include(t => t.BankAccount)
                                                         .FirstOrDefaultAsync(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Check if the user owns the bank account from which the transaction is made
            if (transaction.BankAccount.UserId != userId)
            {
                return BadRequest("Operation not allowed on this transaction.");
            }

            // Validate the destination bank account
            var destinationAccount = await _context.BankAccounts.FindAsync(transactionDto.BankAccountId);
            if (destinationAccount == null || destinationAccount.UserId != userId)
            {
                return BadRequest("Destination bank account not valid or does not belong to user.");
            }

            // Update properties if the above checks are passed
            transaction.Description = transactionDto.Description;
            transaction.Amount = transactionDto.Amount;
            transaction.TransactionDate = transactionDto.TransactionDate;

            // Optional: Update the BankAccount only if it's allowed and validated
            if (transaction.BankAccountId != transactionDto.BankAccountId)
            {
                transaction.BankAccountId = transactionDto.BankAccountId;
            }

            _context.Entry(transaction).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(t => t.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }



        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            var transaction = await _context.Transactions.Include(t => t.BankAccount)
                .SingleOrDefaultAsync(t => t.Id == id && t.BankAccount.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
