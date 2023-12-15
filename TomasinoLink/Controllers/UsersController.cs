using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TomasinoLink.ViewModels;

public class UsersController : Controller
{
    private readonly TomasinoLinkDbContext _context;

    public UsersController(TomasinoLinkDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null || !int.TryParse(userIdString, out int userId))
            return null;

        return await _context.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Faculty)
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<IActionResult> FindYourLink()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            // Handle the case where the current user is not found
            return RedirectToAction("Login", "Account");
        }

        var currentUserId = currentUser.UserId;
        ViewBag.CurrentUserId = currentUserId;

        // Load the user cards data into memory
        var users = await _context.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Faculty)
            .Include(u => u.Photos)
            .Where(u => u.UserId != currentUserId)
            .ToListAsync();

        // Get the IDs of users that the current user has linked
        var linkedUserIds = _context.Linked
            .Where(l => l.UserId == currentUserId)
            .Select(l => l.LinkId)
            .ToList();

        // Now that the data is in memory, we can safely use the null conditional operator
        var userCardViewModels = users.Select(u => new UserCardViewModel
        {
            UserId = u.UserId,
            Name = u.Profile?.NameDisplay ?? "Unknown",
            Age = DateTime.Today.Year - u.DateOfBirth.Year,
            Faculty = u.Profile?.Faculty?.Name ?? "Not Specified",
            ProfilePictureUrl = u.Photos.FirstOrDefault(p => p.IsProfilePicture)?.FileName ?? "default-profile.jpg",
            IsLinked = linkedUserIds.Contains(u.UserId)
        }).ToList();

        return View(userCardViewModels);
    }


}
