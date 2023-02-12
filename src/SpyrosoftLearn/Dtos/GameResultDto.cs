namespace SpyrosoftLearn.Dtos
{
    public class GameResultDto
    {
        public string UserId { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public int NumberOfPoints { get; set; }

        public int NumberOfLuckyPoints { get; set; }

        public int TotalPoints { get; set; }
    }
}
