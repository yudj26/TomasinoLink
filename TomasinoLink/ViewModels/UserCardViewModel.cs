namespace TomasinoLink.ViewModels
{
    public class UserCardViewModel
    {
        public int UserId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Faculty { get; set; }
        public string? Message { get; set; }
        public bool IsLinked { get; set; }
        public int LinkedId { get; set; }
        public int MatchId { get; set; }
        public string MessageToUser { get; set; } // Message from the current user to the matched user
        public string MessageFromUser { get; set; } // Message from the matched user to the current user
    }
}
