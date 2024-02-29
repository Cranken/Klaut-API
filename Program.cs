using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/upload", async (HttpContext inp) =>
{
    using (var reader = new StreamReader(inp.Request.Body))
    {
        var data = await reader.ReadToEndAsync();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();

        var id = new string(Enumerable.Repeat(chars, 5)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        using (var f = new StreamWriter(File.Create($"data/{@id}.txt")))
        {
            await f.WriteAsync(data);
        }
        Console.WriteLine(data);
    }
    return "test";
})
.WithName("Upload")
.WithOpenApi();

app.Run();

