namespace SpyrosoftLearn.Models
{
    public class LuckyNumber
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public Round Round { get; set; } = default!;

        public int RoundId { get; set; }
    }
}
