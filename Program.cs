using System.Data.SqlTypes;
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
    using (var reader = inp.Request.Body)
    {
        if (inp.Request.ContentLength.HasValue)
        {
            var cl = inp.Request.ContentLength.Value;
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            var id = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            using (var f = File.Create($"data/{@id}"))
            {
                long read = 0;
                var convertedLength = Convert.ToInt32(cl);
                while (read < cl)
                {
                    var dataBuf = new byte[convertedLength];
                    var curRead = await inp.Request.Body.ReadAsync(dataBuf, 0, convertedLength);
                    await f.WriteAsync(dataBuf, 0, curRead);
                    read += curRead;
                }
            }
        }
    }
    return "test";
})
.WithName("Upload")
.WithOpenApi();

app.Run();

