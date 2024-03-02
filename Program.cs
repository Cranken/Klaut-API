using System.ComponentModel.Design;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;

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

app.MapPost("/upload", async (HttpContext ctx) =>
{
    if (ctx.Request.ContentType is null)
    {
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    if (ctx.Request.ContentLength.HasValue)
    {
        if (ctx.Request.ContentType!.Contains("multipart/form-data"))
        {
            // Multipart data
            var mpartBoundary = ctx.Request.GetMultipartBoundary();
            var reader = new MultipartReader(mpartBoundary, ctx.Request.Body);
            var section = await reader.ReadNextSectionAsync();
            var ids = new List<string>();
            while (section is not null)
            {
                var cDispHeader = section.GetContentDispositionHeader();
                if (cDispHeader is not null)
                {
                    var id = GenerateRandomAlphanumericalString(5);

                    using var f = new FileStream($"data/{@id}", FileMode.Append);
                    await section.Body.CopyToAsync(f);
                    ids.Add(id);
                }

                section = await reader.ReadNextSectionAsync();
            }
            await ctx.Response.WriteAsync(String.Join("\n", ids));

        }
        else
        {
            // Conventional body post
            var contentLength = ctx.Request.ContentLength.Value;
            var id = GenerateRandomAlphanumericalString(5);

            using var f = new FileStream($"data/{@id}", FileMode.Append);
            await ctx.Request.Body.CopyToAsync(f);
            await ctx.Response.WriteAsync(id);
        }
    }
})
.WithName("Upload")
.WithOpenApi();

app.MapGet("/", (HttpContext req) =>
{
    // Frontpage
    return "Hello";
});

app.MapGet("/{id}.{ext?}", (HttpContext req) =>
{
    // Retrieve files
    Console.WriteLine(req.Request.RouteValues["id"]);
    if (req.Request.RouteValues.ContainsKey("ext"))
    {
        Console.WriteLine(req.Request.RouteValues["ext"]);
    }
});

static string GenerateRandomAlphanumericalString(int length)
{
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();

    return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}


app.Run();

