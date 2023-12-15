using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TomasinoLink.Models
{
    public class Linked
    {
        [Key]
        public int MatchId { get; set; }

        [ForeignKey("LinkInitiator")]
        public int UserId { get; set; }
        public virtual User LinkInitiator { get; set; }

        [ForeignKey("LinkedUser")]
        public int LinkId { get; set; }
        public virtual User LinkedUser { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Message { get; set; } // Initially null, can be set once when the users match
    }

}
