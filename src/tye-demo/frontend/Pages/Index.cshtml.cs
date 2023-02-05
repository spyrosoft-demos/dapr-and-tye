using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace frontend.Pages;

public class IndexModel : PageModel
{
    public WeatherForecastDto[]? Forecasts { get; set; }
    public async Task OnGet([FromServices] WeatherClient client)
    {
        Forecasts = await client.GetWeatherAsync();
    }
}
