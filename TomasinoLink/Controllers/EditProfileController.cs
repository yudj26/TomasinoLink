using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomasinoLink.Models;
using TomasinoLink.ViewModels;
using System.Diagnostics; // Add this to use Activity for error handling
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EditProfileController : Controller
{
    private readonly TomasinoLinkDbContext _context;

    public EditProfileController(TomasinoLinkDbContext context)
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
            // Log error and handle the case where the user ID is not found or invalid
            return -1; // Return an invalid ID indicator
        }
    }

    public async Task<IActionResult> EditProfiles()
    {
        int userId = GetCurrentUserId();
        if (userId == -1)
        {
            // Redirect to a custom error view or handle the error as required
            return RedirectToAction("Error");
        }

        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
        {
            // No profile exists for this user, so create one
            profile = new Profile
            {
                UserId = userId,
                // Set default values or leave them blank to be filled out by the user
            };

            // Add the new profile to the context
            _context.Profiles.Add(profile);
            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        var viewModel = new ProfileViewModel
        {
            NameDisplay = profile.NameDisplay,
            Bio = profile.Bio,
            Location = profile.Location,
            Interest = profile.Interest,
            FacultyId = profile.FacultyId,
            FacultyList = new SelectList(await _context.Faculties.ToListAsync(), "FacultyId", "Name", profile.FacultyId)
        };

        return View(viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfiles(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.FacultyList = new SelectList(_context.Faculties, "FacultyId", "Name", model.FacultyId);
            return View(model);
        }

        int userId = GetCurrentUserId();
        if (userId == -1)
        {
            // Redirect to a custom error view or handle the error as required
            return RedirectToAction("Error");
        }

        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
        {
            // Redirect to a custom error view or handle the error as required
            return RedirectToAction("Error");
        }

        profile.NameDisplay = model.NameDisplay;
        profile.Bio = model.Bio;
        profile.Location = model.Location;
        profile.Interest = model.Interest;
        profile.FacultyId = model.FacultyId;

        _context.Update(profile);
        await _context.SaveChangesAsync();

        return RedirectToAction("ShowProfile", "Profiles");
    }

    // You may also want to implement the Error action if you haven't already
    public IActionResult Error()
    {
        // Create an error model with the necessary data
        var errorViewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View(errorViewModel);
    }
}
