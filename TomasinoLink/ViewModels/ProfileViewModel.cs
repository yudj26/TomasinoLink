using Microsoft.AspNetCore.Mvc.Rendering;

namespace TomasinoLink.ViewModels
{
    public class ProfileViewModel
    {
        public string NameDisplay { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string Interest { get; set; }
        public int? FacultyId { get; set; }
        public SelectList FacultyList { get; set; } // This will be used to populate the dropdown list for faculties
    }
}
