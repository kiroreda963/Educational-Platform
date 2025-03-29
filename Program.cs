using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext with a connection string
builder.Services.AddDbContext<Milestone3Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // Make sure your connection string is correct in appsettings.json

// Register session services
builder.Services.AddDistributedMemoryCache();  // For in-memory session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set the session timeout (optional)
    options.Cookie.HttpOnly = true;  // Set the cookie to be HttpOnly
    options.Cookie.IsEssential = true;  // Make the session cookie essential
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use session middleware
app.UseSession();  // This needs to be before UseRouting

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
