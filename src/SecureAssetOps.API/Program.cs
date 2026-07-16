var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    Service = "SecureAssetOps.API",
    Status = "Running"
}));

app.MapControllers();

app.Run();

public partial class Program
{
}
