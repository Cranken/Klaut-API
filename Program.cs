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

app.MapPost("/upload", async (HttpContext req) =>
{
    var body = req.Request.Body;
    if (req.Request.ContentLength.HasValue)
    {
        var contentLength = req.Request.ContentLength.Value;
        var id = GenerateRandomAlphanumericalString(5);

        await WriteStreamToFileAsync($"data/{@id}", req.Request.Body, contentLength);
    }
    return "test";
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

static async Task WriteStreamToFileAsync(string name,
                                    Stream data,
                                    long dataLength)
{
    using var f = File.Create(name);
    long read = 0;
    var convertedLength = Convert.ToInt32(dataLength);
    while (read < dataLength)
    {
        var dataBuf = new byte[convertedLength];
        var curRead = await data.ReadAsync(dataBuf, 0, convertedLength);
        await f.WriteAsync(dataBuf, 0, curRead);
        read += curRead;
    }

}

static string GenerateRandomAlphanumericalString(int length)
{
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();

    return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}


app.Run();

