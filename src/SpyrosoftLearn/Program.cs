using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SpyrosoftLearn.Data;
using SpyrosoftLearn.Services;
using Microsoft.AspNetCore.ResponseCompression;
using SpyrosoftLearn.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedEmail = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDaprClient();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ILuckyNumberService, LuckyNumberService>();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});


//add to use blazor components
builder.Services.AddServerSideBlazor();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

//add to uze blazorise
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

var daprClient = new DaprClientBuilder().Build();
var sec = await daprClient.GetSecretAsync("local-secret-store", "mssql");
var connectionString = sec["SqlDB-spyrolearn"];
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();


app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
//add to use blazor components
app.MapBlazorHub();
app.MapHub<IntervalHub>("/intervalhub");



using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync();

app.Run();
