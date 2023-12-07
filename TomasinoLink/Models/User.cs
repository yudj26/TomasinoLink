public class Profile
{
    public int ProfileId { get; set; }
    public int UserId { get; set; } // Foreign key to User
    public string NameDisplay { get; set; }
    public string Bio { get; set; }
    public string Location { get; set; }
    public int? FacultyId { get; set; } // Foreign key to Faculty, can be nullable

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Faculty Faculty { get; set; }
}
