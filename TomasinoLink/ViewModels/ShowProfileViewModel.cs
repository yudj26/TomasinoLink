using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TomasinoLink.ViewModels
{
    public class ShowProfileViewModel
    {
        public string? NameDisplay { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? Interest { get; set; }
        public int? FacultyId { get; set; }
        public SelectList? FacultyList { get; set; }
        public int UserId { get; set; }

        // New properties
        public int Age { get; set; }
        public string? Gender { get; set; }
        public IEnumerable<PhotoViewModel>? Photos { get; set; } // Assuming you have a PhotoViewModel
    }
}
