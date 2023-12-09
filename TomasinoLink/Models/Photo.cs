namespace TomasinoLink.Models
{
    public class Photo
    {
        public int PhotoId { get; set; } // Primary key
        public int UserId { get; set; } // Foreign key reference to the User
        public string FileName { get; set; }
        public bool IsProfilePicture { get; set; }

        // Navigation property to the User
        public virtual User User { get; set; }
    }
}