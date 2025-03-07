using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using AEET.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure logging (builder.Logging is available by default)
// You can add additional logging providers here if needed

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session service with a 30-minute idle timeout.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Entity Framework Core to use SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your custom SQL test service.
builder.Services.AddSingleton<Sql>();

// Add CORS policy for development. (Tighten this for production.)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// (Optional) Add authentication and authorization services here if needed in the future.
// For example:
// builder.Services.AddAuthentication(/* options */)
//         .AddCookie(/* options */);
// builder.Services.AddAuthorization();

var app = builder.Build();

// Test the database connection using dependency injection.
try
{
    using (var scope = app.Services.CreateScope())
    {
        var sqlTest = scope.ServiceProvider.GetRequiredService<Sql>();
        sqlTest.TestConnection(); // This prints success/failure in the terminal.
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database Connection Failed: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Use the CORS policy before routing.
app.UseCors("AllowAllOrigins");

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Serve static files (like CSS, JavaScript, images).
app.UseStaticFiles();

// Enable routing.
app.UseRouting();

// Enable session middleware.
app.UseSession();

// (Optional) If you add authentication later, call app.UseAuthentication() before UseAuthorization.
// app.UseAuthentication();

// Enable authorization middleware.
app.UseAuthorization();

// Define the default route (starting at the login page).
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
