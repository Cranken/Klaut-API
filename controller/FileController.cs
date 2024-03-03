
using FileAPI.PostgreSQL;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FileAPI.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileContext _context;

        public FileController(FileContext fc)
        {
            _context = fc;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FileAPI.PostgreSQL.File>> GetFileAsync(string id)
        {
            Console.WriteLine(id);
            var file = await _context.Files.FindAsync(id);

            if (file == null)
            {
                return NotFound();
            }
            return file;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> UploadFile()
        {
            if (Request.ContentType is null)
            {
                return BadRequest();
            }

            if (Request.ContentLength.HasValue)
            {
                if (Request.ContentType!.Contains("multipart/form-data"))
                {
                    // Multipart data
                    var mpartBoundary = Request.GetMultipartBoundary();
                    var reader = new MultipartReader(mpartBoundary, Request.Body);
                    var section = await reader.ReadNextSectionAsync();
                    var ids = new List<string>();
                    while (section is not null)
                    {
                        var cDispHeader = section.GetContentDispositionHeader();
                        if (cDispHeader is not null)
                        {
                            var id = Utils.GenerateRandomAlphanumericalString(5);

                            using var f = new FileStream($"data/{@id}", FileMode.Append);
                            await section.Body.CopyToAsync(f);
                            ids.Add(id);
                        }

                        section = await reader.ReadNextSectionAsync();
                    }
                    return String.Join("\n", ids);

                }
                else
                {
                    // Conventional body post
                    var contentLength = Request.ContentLength.Value;
                    var id = Utils.GenerateRandomAlphanumericalString(5);

                    using var f = new FileStream($"data/{@id}", FileMode.Append);
                    await Request.Body.CopyToAsync(f);
                    return id;
                }
            }
            return BadRequest();
        }
    }

}