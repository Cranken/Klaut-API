using FileAPI.PostgreSQL;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<FileContext>(options =>
                options.UseInMemoryDatabase("devDb"));
}
else
{
    builder.Services.AddDbContext<FileContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("FileContext")));
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<FileContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
