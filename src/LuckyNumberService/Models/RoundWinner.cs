namespace LuckyNumberService.Models
{
    public class RoundWinner
    {
        public int Id { get; set; }

        public int MinNumber { get; set; }

        public int MaxNumber { get; set; }

        public int RoundId { get; set; }

        public int WinnerNumber { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
