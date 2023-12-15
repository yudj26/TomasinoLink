using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomasinoLink.Models;
using TomasinoLink.ViewModels;
using System.Security.Claims;

public class LinkedController : Controller
{
    private readonly TomasinoLinkDbContext _context;

    public LinkedController(TomasinoLinkDbContext context)
    {
        _context = context;
    }

    // Method to handle the linking action
    public async Task<IActionResult> LinkUser(int linkId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Check if the link already exists
        var existingLink = await _context.Linked
            .FirstOrDefaultAsync(l => l.UserId == currentUserId && l.LinkId == linkId);

        if (existingLink == null)
        {
            // If no link exists, create a new one
            var link = new Linked
            {
                UserId = currentUserId,
                LinkId = linkId
            };
            _context.Linked.Add(link);
        }
        else
        {
            // If a link exists, remove it
            _context.Linked.Remove(existingLink);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("FindYourLink", "Users");
    }

    public async Task<IActionResult> Matches()
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var matches = await _context.Linked
            .Where(l => l.UserId == currentUserId || l.LinkId == currentUserId)
            .ToListAsync();

        var matchedUserIds = matches
            .Where(m => m.UserId == currentUserId ? matches.Any(mm => mm.LinkId == currentUserId && mm.UserId == m.LinkId) : matches.Any(mm => mm.UserId == currentUserId && mm.LinkId == m.UserId))
            .Select(m => m.UserId == currentUserId ? m.LinkId : m.UserId)
            .Distinct();

        var users = await _context.Users
            .Include(u => u.Profile)
    .ThenInclude(p => p.Faculty)
    .Include(u => u.Photos)
    .Where(u => matchedUserIds.Contains(u.UserId))
    .ToListAsync();

        var viewModel = users.Select(u => new UserCardViewModel
        {
            UserId = u.UserId,
            Name = u.Profile?.NameDisplay ?? "Unknown", // Safe navigation and coalescing operator
            Age = DateTime.Today.Year - u.DateOfBirth.Year, // Directly access Year
            Faculty = u.Profile?.Faculty?.Name ?? "Unknown Faculty", // Check for null Faculty
            ProfilePictureUrl = u.Photos.Any(p => p.IsProfilePicture) ? u.Photos.FirstOrDefault(p => p.IsProfilePicture)?.FileName ?? "default-profile.jpg" : "default-profile.jpg",
            MessageToUser = matches.FirstOrDefault(m => m.UserId == currentUserId && m.LinkId == u.UserId)?.Message,
            MessageFromUser = matches.FirstOrDefault(m => m.UserId == u.UserId && m.LinkId == currentUserId)?.Message,
            Message = matches.FirstOrDefault(m => (m.UserId == currentUserId && m.LinkId == u.UserId) ||
                                          (m.LinkId == currentUserId && m.UserId == u.UserId))?.Message, // Safe navigation
            LinkedId = matches.Where(m => (m.UserId == currentUserId && m.LinkId == u.UserId) ||
                                          (m.LinkId == currentUserId && m.UserId == u.UserId))
              .Select(m => m.MatchId)
              .FirstOrDefault() // This will get the correct MatchId for the current user pair
        }).ToList();



        return View(viewModel);
    }

    // Method to send a message
    [HttpPost]
    public async Task<IActionResult> SendMessage(int userId, string message)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Determine the correct MatchId based on the current user and the recipient user
        var link = await _context.Linked
            .FirstOrDefaultAsync(l => (l.UserId == currentUserId && l.LinkId == userId) || 
                                      (l.UserId == userId && l.LinkId == currentUserId));
    
        if (link != null)
        {
            // Update the message for the correct MatchId
            link.Message = message;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Matches");
    }

    [HttpPost]
    public async Task<IActionResult> EditMessage(int linkedId, string message)
    {
        var link = await _context.Linked.FindAsync(linkedId);
        if (link != null)
        {
            // Update the message
            link.Message = message;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Matches");
    }

}
