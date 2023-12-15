using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TomasinoLink.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }

    // Navigation property
    public virtual Profile? Profile { get; set; }
    public virtual ICollection<Photo>? Photos { get; set; }
    public virtual ICollection<Linked> InitiatedLinks { get; set; } = new HashSet<Linked>();
    public virtual ICollection<Linked> ReceivedLinks { get; set; } = new HashSet<Linked>();

}
