using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;
using TomasinoLink.Models;
using TomasinoLink.ViewModels;
using System.Linq;

namespace TomasinoLink.Controllers
{
    [Authorize] // Ensure only authenticated users can access methods
    public class PhotosController : Controller
    {
        private readonly TomasinoLinkDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<IdentityUser> userManager;

        public PhotosController(TomasinoLinkDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            db = context;
            webHostEnvironment = env;
            this.userManager = userManager;
        }

        public IActionResult EditPhotos()
        {
            var userId = userManager.GetUserId(User); // Get the current authenticated user's ID
            var photoViewModels = db.Photos
                .Where(p => p.UserID == userId)
                .Select(p => new PhotoViewModel
                {
                    PhotoID = p.PhotoID,
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
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(); // or redirect to login
                }

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
                    UserID = user.Id,
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
            var user = await userManager.GetUserAsync(User);
            var photo = await db.Photos.FindAsync(id);

            if (photo != null && photo.UserID == user.Id)
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
            var user = await userManager.GetUserAsync(User);
            var allPhotos = db.Photos.Where(p => p.UserID == user.Id).ToList();

            foreach (var photo in allPhotos)
            {
                photo.IsProfilePicture = false;
            }

            var selectedPhoto = allPhotos.FirstOrDefault(p => p.PhotoID == id);
            if (selectedPhoto != null)
            {
                selectedPhoto.IsProfilePicture = true;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("EditPhotos");
        }
    }
}
