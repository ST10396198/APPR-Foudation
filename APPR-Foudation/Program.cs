using APPR_Foudation.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()  // For debugging
           .EnableDetailedErrors());      // For debugging

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("🔄 Checking database connection...");

        // Test connection
        if (context.Database.CanConnect())
        {
            logger.LogInformation("✅ Database connection successful!");
        }
        else
        {
            logger.LogWarning("❌ Cannot connect to database");
        }

        logger.LogInformation("🔄 Applying database migrations...");

        // Apply migrations
        context.Database.Migrate();

        logger.LogInformation("✅ Database migrations applied successfully!");

        // Check if we have any users
        var userCount = context.Users.Count();
        logger.LogInformation("📊 Current users in database: {UserCount}", userCount);

    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred during database initialization");
    }
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();