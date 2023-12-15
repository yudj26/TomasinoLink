using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomasinoLink.Models;
using TomasinoLink.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProfilesController : Controller
{
    private readonly TomasinoLinkDbContext _context;

    public ProfilesController(TomasinoLinkDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        else
        {
            // Handle this gracefully by redirecting to an error or login page
            return -1; // Indicate an invalid user ID
        }
    }

    [HttpGet("Profiles/ShowProfile/{userId?}")]
    public async Task<IActionResult> ShowProfile(int? userId)
    {
        // If no userId is provided, use the ID of the currently logged-in user
        int userToDisplayId = userId ?? GetCurrentUserId();

        if (userToDisplayId == -1)
        {
            // Redirect to login or show error
            return RedirectToErrorView();
        }

        var user = await _context.Users.FindAsync(userToDisplayId);
        if (user == null)
        {
            // User not found, show error
            return RedirectToErrorView();
        }

        var profile = await _context.Profiles
                                    .Include(p => p.Faculty)
                                    .FirstOrDefaultAsync(p => p.UserId == userToDisplayId);
        if (profile == null)
        {
            // Profile not found, show error
            return RedirectToErrorView();
        }

        // Calculate age
        int age = DateTime.Today.Year - user.DateOfBirth.Year;
        if (user.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

        // Get photos
        var photos = await _context.Photos
                                   .Where(p => p.UserId == userToDisplayId)
                                   .Select(p => new PhotoViewModel
                                   {
                                       PhotoID = p.PhotoId,
                                       FileName = p.FileName,
                                       IsProfilePicture = p.IsProfilePicture
                                   })
                                   .ToListAsync();

        // Prepare the ViewModel
        var viewModel = new ShowProfileViewModel
        {
            UserId = userToDisplayId,
            NameDisplay = profile.NameDisplay,
            Bio = profile.Bio,
            Location = profile.Location,
            Interest = profile.Interest,
            FacultyId = profile.FacultyId,
            FacultyList = new SelectList(await _context.Faculties.ToListAsync(), "FacultyId", "Name", profile.FacultyId),
            Age = age,
            Gender = user.Gender,
            Photos = photos
        };

        // Pass the current user ID to the view
        ViewBag.CurrentUserId = GetCurrentUserId();
        return View(viewModel);
    }

    private IActionResult RedirectToErrorView()
    {
        var errorViewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View("Error", errorViewModel);
    }
}
