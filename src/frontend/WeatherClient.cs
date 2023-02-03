namespace frontend;

public class WeatherClient
{
    private readonly HttpClient client;

    public WeatherClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<WeatherForecastDto[]?> GetWeatherAsync() => await client.GetFromJsonAsync<WeatherForecastDto[]>("/weatherforecast");
}
