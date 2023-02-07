namespace SpyrosoftLearn.Models
{
    public class Round
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string CreatorName { get; set; } = default!;

        public bool IsActive { get; set; }

        public int? MinNumber { get; set; }

        public int? MaxNumber { get; set; }

        public int? WinnerNumber { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? FinishedOn { get; set; }
    }
}
