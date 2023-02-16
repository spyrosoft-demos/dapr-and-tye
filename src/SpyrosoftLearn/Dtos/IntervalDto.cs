namespace SpyrosoftLearn.Dtos;

public class IntervalDto
{
    public string UserName { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public int? UserConfigNumber { get; set; }

    public int NumberOfClicks { get; set; }

    public bool IsAdmin { get; set; } = false;

    public int? ActiveRoundId { get; set; }
}
