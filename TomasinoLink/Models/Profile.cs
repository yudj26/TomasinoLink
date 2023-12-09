namespace TomasinoLink.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; } // Foreign key to the User
        public string? Bio { get; set; }
        public int? FacultyId { get; set; } // Foreign key to the Faculty
        public string? Location { get; set; }
        public string? NameDisplay { get; set; }
        public string? Interest { get; set; }


        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Faculty? Faculty { get; set; }
    }
}