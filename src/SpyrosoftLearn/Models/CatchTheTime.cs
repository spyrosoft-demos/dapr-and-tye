namespace SpyrosoftLearn.Models;

public class CatchTheTime
{
    public int Id { get; set; }

    public string UserId { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public DateTime ClickTime { get; set; }    
    
    public int NumberOfPoints { get; set; }
}
