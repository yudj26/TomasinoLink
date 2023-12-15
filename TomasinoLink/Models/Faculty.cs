namespace TomasinoLink.Models
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string? Name { get; set; }

        // Navigation property to Profiles
        public virtual ICollection<Profile>? Profiles { get; set; }
    }
}