var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app
    .UseStaticFiles("/resources");

app
    .MapGroup("api")
    .MapGet("health", () => "healthy");

app.Run();

namespace SampleSite
{
    public class Program { }
}