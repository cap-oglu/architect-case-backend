using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialManagementMVC.Models;
using FinancialManagementMVC.Data;
using FinancialManagementMVC.DTOs;

namespace FinancialManagementMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure all actions in this controller require authentication
    public class TransfersController : ControllerBase
    {
        private readonly FinancialContext _context;

        public TransfersController(FinancialContext context)
        {
            _context = context;
        }

        // GET: api/Transfers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transfer>>> GetTransfers()
        {
           var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            // Fetch all transfers where the current user's bank account is either the source or destination
            var transfers = await _context.Transfers
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Where(t => t.FromAccount.UserId == userId || t.ToAccount.UserId == userId)
                .ToListAsync();

            if (transfers == null || transfers.Count == 0)
            {
                return NotFound("No transfers found for the current user.");
            }

            // Optionally, you might want to convert these transfers to a DTO to hide certain fields or format data
            return Ok(transfers);
        }
        // POST: api/Transfers
        [HttpPost]
        public async Task<ActionResult<Transfer>> PostTransfer(TransferDto transferDto)
        {
            // Validate the transfer DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the user ID from the claims
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            // Retrieve the source account
            var fromAccount = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == transferDto.FromAccountId && a.UserId == userId);
            if (fromAccount == null)
            {
                return BadRequest("Unauthorized attempt to access bank account or account not found.");
            }

            // Check if the source account has sufficient balance
            if (fromAccount.Balance < transferDto.Amount)
            {
                return BadRequest("Insufficient funds.");
            }

            // Deduct the amount from the source account
            fromAccount.Balance -= transferDto.Amount;

            // Retrieve the destination account
            var toAccount = await _context.BankAccounts.FindAsync(transferDto.ToAccountId);
            if (toAccount == null)
            {
                return NotFound("Destination account not found.");
            }

            // Add the amount to the destination account
            toAccount.Balance += transferDto.Amount;

            // Create a new transfer record
            var transfer = new Transfer
            {
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                Amount = transferDto.Amount,
                Description = transferDto.Description
            };

            // Add the transfer to the context and save changes
            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();

            // Return the created transfer
            return CreatedAtAction("GetTransfer", new { id = transfer.Id }, transfer);
        }
        

        // GET: api/Transfers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transfer>> GetTransfer(int id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            var transfer = await _context.Transfers
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .FirstOrDefaultAsync(t => t.Id == id && (t.FromAccount.UserId == userId || t.ToAccount.UserId == userId));

            if (transfer == null)
            {
                return NotFound();
            }

            return transfer;
        }

        // PUT: api/Transfers/5 (Modify transfer, optional based on business rules)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransfer(int id, Transfer transfer)
        {
            if (id != transfer.Id)
            {
                return BadRequest();
            }

            var userIdString = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("User ID is missing or invalid.");
            }

            var existingTransfer = await _context.Transfers
                .Include(t => t.FromAccount)
                .FirstOrDefaultAsync(t => t.Id == id && t.FromAccount.UserId == userId);

            if (existingTransfer == null)
            {
                return NotFound();
            }

            // Check if modification is allowed based on business rules, e.g., transfers cannot be modified after a certain period
            existingTransfer.Description = transfer.Description;
            _context.Entry(existingTransfer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransferExists(id))
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

        private bool TransferExists(int id) => _context.Transfers.Any(e => e.Id == id);
    }
}
