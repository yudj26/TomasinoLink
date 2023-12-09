using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomasinoLink.ViewModels;

public class ProfilesController : Controller
{
    public ProfilesController(TomasinoLinkDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        if (context == null)
        {
            // You can replace this with your logging mechanism
            throw new ArgumentNullException(nameof(context));
        }

        _context = context;
    }

    private readonly TomasinoLinkDbContext _context;
    // Constructor, GetCurrentUserId, other actions...
    private int GetCurrentUserId()
    {
        // Assuming the user's ID is stored in the NameIdentifier claim
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            else
            {
                throw new ApplicationException("Invalid user ID in claims.");
            }
        }
        else
        {
            throw new ApplicationException("No user ID claim present.");
        }
    }


    // GET: Profiles/EditProfile
    public async Task<IActionResult> EditProfiles()
    {
        int userId = GetCurrentUserId();
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            // Handle the case where the profile does not exist
        }

        var viewModel = new ProfileViewModel
        {
            NameDisplay = profile.NameDisplay,
            Bio = profile.Bio,
            Location = profile.Location,
            Interest = profile.Interest,
            FacultyId = profile.FacultyId,
            FacultyList = new SelectList(_context.Faculties, "FacultyId", "Name", profile.FacultyId)
        };

        return View(viewModel);
    }

    // POST: Profiles/EditProfile
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
        // Log the user ID to verify
        Console.WriteLine("User ID: " + userId);
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile != null)
        {
            profile.NameDisplay = model.NameDisplay;
            profile.Bio = model.Bio;
            profile.Location = model.Location;
            profile.Interest = model.Interest;
            profile.FacultyId = model.FacultyId;

            _context.Update(profile);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index"); // Or whatever view you want to redirect to
    }
}
