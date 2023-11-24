namespace TomasinoLink.Models
{
    public class Photo
    {
        public int PhotoID { get; set; }
        public int UserID { get; set; } // Will be linked to user accounts once authentication is set up
        public string FileName { get; set; }
        public bool IsProfilePicture { get; set; }
    }
}
