using Microsoft.AspNetCore.HttpLogging;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpLogging(logging =>  logging.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestHeaders | HttpLoggingFields.ResponseHeaders);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
}

app.UseAuthorization();
app.MapControllers();

//-----------------------------------------
// environment variables dump
//-----------------------------------------
Console.WriteLine("GetEnvironmentVariables: ");
var envs = Environment.GetEnvironmentVariables()
                      .Cast<DictionaryEntry>()
                      .Where(e => e.Key.ToString()?.StartsWith("SERVICE")??false)
                      .OrderBy(e => e.Key);
Console.WriteLine("Environment variables starting with 'SERVICE': {0}", envs.Count());
foreach (DictionaryEntry de in envs)
    Console.WriteLine("  {0} = {1}", de.Key, de.Value);

await app.RunAsync();
