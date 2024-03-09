using System.Text;
using FileAPI;
using FileAPI.AuthService;
using FileAPI.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<FileContext>(options =>
                options.UseInMemoryDatabase("dbFile"));
    builder.Services.AddDbContext<AuthContext>(options =>
                options.UseInMemoryDatabase("dbAuth"));
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services
        .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(opt =>
        {
            opt.RequireHttpsMetadata = false; // Dev
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        builder.Configuration["Authentication:JWT:SecretKey"]
                         ?? Utils.GenerateRandomAlphanumericalString(16) // Fallback for development
                        )),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Authentication:JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Authentication:JWT:Audience"]
            };
        });
}
else
{
    builder.Services.AddDbContext<FileContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("FileContext")));
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

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
else
{
    app.UseHttpsRedirection();
}


app.MapControllers();
app.Run();
