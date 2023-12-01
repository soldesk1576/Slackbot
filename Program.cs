using Microsoft.EntityFrameworkCore;
using StatusNotifier.Data;
using StatusNotifier.Scheduler;
using StatusNotifier.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StatusNotifierDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddSingleton<StatusChecker>(); // Add StatusChecker as a singleton
builder.Services.AddSingleton<GeminiStatusNotifier>(); // Add GeminiStatusNotifier as a singleton
builder.Services.AddHostedService<StatusUpdateTask>(); // Add StatusUpdateTask as a hosted service
var app = builder.Build();

app.UseRouting();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
