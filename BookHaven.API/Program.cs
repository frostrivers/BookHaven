using BookHaven.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Enable CORS to allow your webpage to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", b =>
    {
        b.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add Entity Framework Core with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=bookhaven.db";

builder.Services.AddDbContext<BookHavenDbContext>(options =>
    options.UseSqlite(connectionString));

// Register the seeder
builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

app.UseCors("AllowLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BookHavenDbContext>();
        dbContext.Database.EnsureCreated();

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors(b =>
    b.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
