// Controllers/UsersController.cs
using FinancialManagementMVC.Data;
using FinancialManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialManagementMVC.Utilities;
using FinancialManagementMVC.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FinancialManagementMVC.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly FinancialContext _context;
    //private readonly JwtTokenGenerator _jwtTokenGenerator;

    private readonly IConfiguration _configuration;

    public UsersController(FinancialContext context, IConfiguration configuration /*JwtTokenGenerator jwtTokenGenerator*/)
    {
        _context = context;
        _configuration = configuration;
        //_jwtTokenGenerator = jwtTokenGenerator;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.Include(u => u.BankAccounts).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Email = userDto.Email;
        user.Password = userDto.Password; // Consider securely updating passwords, if applicable

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(UserDto userDto)
    {
        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Password = userDto.Password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.Id }, user);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
    {
        // Check if the email is already in use
        var existingUser = await _context.Users
                                        .AnyAsync(u => u.Email == registrationDto.Email);
        if (existingUser)
        {
            return BadRequest("Email already in use."); // Return a BadRequest if the email is already registered
        }

        // Hash the password
        var hasher = new PasswordHasher();
        var user = new User
        {
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            Email = registrationDto.Email,
            Password = hasher.HashPassword(registrationDto.Password)
        };

        // Create a default bank account for the new user
        var defaultBankAccount = new BankAccount
        {
            AccountNumber = hasher.GenerateAccountNumber(),  // Assuming you have a method to generate unique account numbers
            Balance = 0,  // Starting balance could be set to 0 or another default value
            Currency = "USD",  // Default currency, this could be dynamic based on user preferences or locale
            User = user  // Link the bank account to the user
        };

        // Add the new user and their default bank account to the database
        _context.Users.Add(user);
        _context.BankAccounts.Add(defaultBankAccount);
        await _context.SaveChangesAsync();

        // Return a 201 Created response
        return StatusCode(201); // Or use CreatedAtAction if you want to redirect and provide a location header
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result != PasswordVerificationResult.Success)
        {
            return Unauthorized("Invalid credentials.");
        }

        // Add this line to get the IConfiguration instance
        var tokenGen = new JwtTokenGenerator(_configuration); // Pass the IConfiguration instance to the JwtTokenGenerator constructor
        var token = tokenGen.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }


    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
