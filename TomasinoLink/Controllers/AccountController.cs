using Microsoft.AspNetCore.Mvc;
using TomasinoLink.ViewModels;
using TomasinoLink.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

public class AccountController : Controller
{
    private readonly TomasinoLinkDbContext _context;

    public AccountController(TomasinoLinkDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == model.Email);
            if (existingUser)
            {
                ModelState.AddModelError("", "Email already in use.");
                return View(model);
            }

            // Create a new User entity
            var user = new User
            {
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                PasswordHash = HashPassword(model.Password)
            };

            // Add the new User to the context
            _context.Users.Add(user);

            // Save changes so the User gets an ID
            await _context.SaveChangesAsync();

            // Create a new Profile entity with the ID of the newly created User
            var profile = new Profile
            {
                UserId = user.UserId,
                // Set other properties with default values or as null
                NameDisplay = "", // or some default value
                Bio = "", // or some default value
                Location = "", // or some default value
                Interest = "", // or some default value
                               // Assume a default FacultyId, or set as null if the column allows it
                FacultyId = null // or a valid default FacultyId if you have one
            };

            // Add the new Profile to the context
            _context.Profiles.Add(profile);

            // Save changes to create the Profile
            await _context.SaveChangesAsync();

            // Redirect to the Login page after successful registration
            return RedirectToAction("Login");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && VerifyPassword(user.PasswordHash, model.Password))
            {
                await SignInUser(user);
                return RedirectToLocal(returnUrl);
            }

            // If the login fails, set an error message in ModelState
            ModelState.AddModelError("", "Invalid email or password.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with the salt
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // Combine the salt and the hash for storage
        string savedPasswordHash = $"{Convert.ToBase64String(salt)}.{hashed}";
        return savedPasswordHash;
    }

    private bool VerifyPassword(string hashedPassword, string inputPassword)
    {
        // Split the hash from the salt
        var parts = hashedPassword.Split('.', 2);
        if (parts.Length != 2)
        {
            throw new FormatException("The hashed password is not in the expected format '<salt>.<hash>'");
        }

        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];

        // Hash the input password with the salt from the stored hash
        string inputHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: inputPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // Compare the hash of the input password with the hash from the stored password
        return hash == inputHashed;
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
