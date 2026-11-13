using ChatApp.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Slide 8: mendaftarkan SignalR
builder.Services.AddSignalR();
builder.Services.AddControllers();

// Angular dev server berjalan di localhost:4200 -> izinkan akses lintas origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // SignalR membutuhkan AllowCredentials saat origin spesifik
});

// Backend berjalan di http://localhost:5000
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

app.UseCors("AngularClient");

// Slide 8: app.MapHub<...>("/...")
app.MapHub<ChatHub>("/chathub");
app.MapControllers();

app.MapGet("/", () => "ChatApp API berjalan.");

app.Run();
