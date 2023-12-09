using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TomasinoLink.Models;
using TomasinoLink.ViewModels;
using System.Security.Claims;

namespace TomasinoLink.Controllers
{
    [Authorize] // Ensure only authenticated users can access methods
    public class PhotosController : Controller
    {
        private readonly TomasinoLinkDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PhotosController(TomasinoLinkDbContext context, IWebHostEnvironment env)
        {
            db = context;
            webHostEnvironment = env;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Handle the case where userId cannot be parsed to an integer
            throw new UnauthorizedAccessException("User ID not found or invalid.");
        }

        public IActionResult EditPhotos()
        {
            int userId = GetCurrentUserId();
            var photoViewModels = db.Photos
                .Where(p => p.UserId == userId)
                .Select(p => new PhotoViewModel
                {
                    PhotoID = p.PhotoId,
                    FileName = p.FileName,
                    IsProfilePicture = p.IsProfilePicture
                }).ToList();

            return View(photoViewModels);
        }

        [HttpPost]
        public async Task<ActionResult> Add(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                int userId = GetCurrentUserId();
                var fileName = Path.GetFileName(file.FileName);
                var uploads = Path.Combine(webHostEnvironment.WebRootPath, "Content", "Images");

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var photo = new Photo
                {
                    UserId = userId,
                    FileName = fileName,
                    IsProfilePicture = false
                };

                db.Photos.Add(photo);
                await db.SaveChangesAsync();

                return RedirectToAction("EditPhotos");
            }

            return View("EditPhotos");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            var photo = await db.Photos.FindAsync(id);

            if (photo != null && photo.UserId == userId)
            {
                db.Photos.Remove(photo);
                await db.SaveChangesAsync();

                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", photo.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            return RedirectToAction("EditPhotos");
        }

        [HttpPost]
        public async Task<ActionResult> SetAsMain(int id)
        {
            int userId = GetCurrentUserId();
            var allPhotos = db.Photos.Where(p => p.UserId == userId).ToList();

            foreach (var photo in allPhotos)
            {
                photo.IsProfilePicture = false;
            }

            var selectedPhoto = allPhotos.FirstOrDefault(p => p.PhotoId == id);
            if (selectedPhoto != null)
            {
                selectedPhoto.IsProfilePicture = true;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("EditPhotos");
        }
    }
}
