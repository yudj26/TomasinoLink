using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using TomasinoLink.Models;
using TomasinoLink.ViewModels; // Ensure you have this using directive for PhotoViewModel
using System.Collections.Generic; // For IEnumerable

public class PhotosController : Controller
{
    private readonly TomasinoLinkDbContext db;
    private readonly IWebHostEnvironment webHostEnvironment;

    public PhotosController(TomasinoLinkDbContext context, IWebHostEnvironment env)
    {
        db = context;
        webHostEnvironment = env;
    }

    public IActionResult EditPhotos()
    {
        // Create a list of PhotoViewModels from the Photo data model
        var photoViewModels = db.Photos.Select(p => new PhotoViewModel
        {
            PhotoID = p.PhotoID,
            FileName = p.FileName,
            IsProfilePicture = p.IsProfilePicture
            // Add other properties as necessary
        }).ToList();

        return View(photoViewModels);
    }

    [HttpPost]
    public ActionResult Add(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var fileName = Path.GetFileName(file.FileName);
            // Use the correct path where you want to save the images
            var uploads = Path.Combine(webHostEnvironment.WebRootPath, "Content", "Images");

            // Ensure the directory exists
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var photo = new Photo
            {
                // Replace with your user authentication logic later
                UserID = 1,
                FileName = fileName,
                IsProfilePicture = false
            };

            db.Photos.Add(photo);
            db.SaveChanges();
        }

        return RedirectToAction("EditPhotos");
    }

    [HttpPost]
    public ActionResult Delete(int id)
    {
        var photo = db.Photos.Find(id);
        if (photo != null)
        {
            db.Photos.Remove(photo);
            db.SaveChanges();

            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", photo.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        return RedirectToAction("EditPhotos");
    }

    [HttpPost]
    public ActionResult SetAsMain(int id)
    {
        var allPhotos = db.Photos.ToList();
        foreach (var photo in allPhotos)
        {
            photo.IsProfilePicture = false;
        }

        var selectedPhoto = db.Photos.FirstOrDefault(p => p.PhotoID == id);
        if (selectedPhoto != null)
        {
            selectedPhoto.IsProfilePicture = true;
            db.SaveChanges();
        }

        return RedirectToAction("EditPhotos");
    }
}
