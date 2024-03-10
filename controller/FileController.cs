
using FileAPI.Attributes.Auth;
using FileAPI.Models.Auth;
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
        public async Task<ActionResult<FileStream>> GetFileAsync(string id)
        {
            var file = await _context.Files.FindAsync(id);

            if (file == null)
            {
                return NotFound();
            }
            var f = new FileStream($"data/{@file.Id}", FileMode.Open, FileAccess.Read);
            return new FileStreamResult(f, file.FileType);
        }

        [AuthorizeRole(Role.User)]
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
                            string id;
                            Models.File? file;

                            // Generate unused ID
                            do
                            {
                                id = Utils.GenerateRandomAlphanumericalString(5);
                                file = await _context.Files.FindAsync(id);
                            } while (file is not null && !ids.Contains(id));

                            var newFile = new Models.File
                            {
                                Id = id,
                                FileType = section.ContentType ?? "",
                                OriginalFileName = cDispHeader.FileName.ToString()
                            };
                            _context.Files.Add(newFile);

                            using var f = new FileStream($"data/{@id}", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            await section.Body.CopyToAsync(f);
                            ids.Add(id);

                            if (section.ContentType != null && section.ContentType.StartsWith("image"))
                            {
                                f.Seek(0, 0); // Reset stream position to avoid image library error
                                await Image.Thumbnail.GenerateThumbnailForImageAsync(f);
                            }
                        }

                        section = await reader.ReadNextSectionAsync();
                    }
                    await _context.SaveChangesAsync();
                    return String.Join("\n", ids);

                }
                else
                {
                    // Conventional body post
                    string id;
                    Models.File? file;

                    // Generate unused ID
                    do
                    {
                        id = Utils.GenerateRandomAlphanumericalString(5);
                        file = await _context.Files.FindAsync(id);
                    } while (file is not null);

                    var newFile = new Models.File
                    {
                        Id = id,
                        FileType = Request.ContentType ?? ""
                    };
                    _context.Files.Add(newFile);
                    await _context.SaveChangesAsync();
                    using var f = new FileStream($"data/{@id}", FileMode.Append);
                    await Request.Body.CopyToAsync(f);
                    return id;
                }
            }
            return BadRequest();
        }

    }

}