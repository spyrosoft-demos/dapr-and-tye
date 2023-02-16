using frontend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient<WeatherClient>(client =>
{
    var host = builder.Configuration.GetValue<string>("service:backend:host");
    var port = builder.Configuration.GetValue<string>("service:backend:port");
    client.BaseAddress = new Uri($"http://{host}:{port}");
    Console.WriteLine("backend url:{0}:{1}", host, port);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

await app.RunAsync();
