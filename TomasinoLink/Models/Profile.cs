namespace TomasinoLink.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; } // Foreign key to the User
        public string Bio { get; set; }
        public int FacultyId { get; set; } // Foreign key to the Faculty
        public string Location { get; set; }
        public string Interests { get; set; } // You can make this more complex based on your requirements

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Faculty Faculty { get; set; }
    }
}