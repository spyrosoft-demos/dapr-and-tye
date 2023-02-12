using LuckyNumberService.DbContexts;
using LuckyNumberService.Repository;
using Microsoft.EntityFrameworkCore;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var daprClient = new DaprClientBuilder().Build();
var sec = await daprClient.GetSecretAsync("local-secret-store", "mssql");
var connectionString = sec["SqlDB-luckynumbers"];

builder.Services.AddDbContext<RoundWinnerContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<IRoundWinnerRepository, RoundWinnerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Starting the app");
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<RoundWinnerContext>();
await context.Database.MigrateAsync();
app.Run();
