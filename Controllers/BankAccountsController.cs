using FinancialManagementMVC.Models;
using FinancialManagementMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FinancialManagementMVC.DTOs;

namespace FinancialManagementMVC.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly FinancialContext _context;

        public BankAccountsController(FinancialContext context)
        {
            _context = context;
        }

        // GET: api/BankAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            var bankAccounts = await _context.BankAccounts
                                             .Where(b => b.UserId == userId)
                                             .ToListAsync();

            return bankAccounts;
        }

        // GET: api/BankAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }
            var bankAccount = await _context.BankAccounts
                                            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (bankAccount == null)
            {
                return NotFound();
            }

            return bankAccount;
        }

        // POST: api/BankAccounts
        [HttpPost]
        public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccountDto bankAccountDto)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }
            var bankAccount = new BankAccount
            {
                AccountNumber = bankAccountDto.AccountNumber,
                Balance = bankAccountDto.Balance,
                Currency = bankAccountDto.Currency,
                UserId = userId  // Set the UserId to the current user's ID
            };


            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBankAccount", new { id = bankAccount.Id }, bankAccount);
        }


        // PUT: api/BankAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccountDto bankAccountDto)
        {
            if (id != bankAccountDto.Id)
            {
                return BadRequest();
            }

            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            
            var bankAccount = await _context.BankAccounts
                                            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (bankAccount.UserId != userId)
            {
                return BadRequest("You can only edit your own accounts.");
            }

            bankAccount.AccountNumber = bankAccountDto.AccountNumber;
            bankAccount.Balance = bankAccountDto.Balance;
            bankAccount.Currency = bankAccountDto.Currency;


            _context.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
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
        

        // DELETE: api/BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }
            var bankAccount = await _context.BankAccounts
                                            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (bankAccount == null)
            {
                return NotFound();
            }
            
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.Id == id);
        }
    }
}
