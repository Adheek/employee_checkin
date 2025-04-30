using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;                       // for JsonNamingPolicy
using AEET.Models;      // ApplicationDbContext, model classes
using AEET.Code;        // ScanTransactionManager, QrGeneratorManager, plus your other managers

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC + runtime-compilation + camelCase JSON
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// 2. Session (30-minute idle timeout)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. EF Core → SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 4. Your custom services
builder.Services.AddSingleton<Sql>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<RoleManager>();
builder.Services.AddScoped<FullDataManager>();
builder.Services.AddScoped<EmployeeManager>();
builder.Services.AddScoped<EmployeeAssetMappingManager>();
builder.Services.AddScoped<ScanTransactionManager>();

// ★ New manager for “search by asset name” ★
builder.Services.AddScoped<QrGeneratorManager>();

// 5. CORS (for API calls, if needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// 6. Test DB connection at startup
try
{
    using var scope = app.Services.CreateScope();
    var sqlTest = scope.ServiceProvider.GetRequiredService<Sql>();
    sqlTest.TestConnection();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database Connection Failed: {ex.Message}");
}

// 7. Error handling & security
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 8. Standard middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 9. CORS (after routing, before auth)
app.UseCors("AllowAllOrigins");

app.UseSession();
app.UseAuthorization();

// 10. Endpoints
app.MapControllers();   // for [ApiController] endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
