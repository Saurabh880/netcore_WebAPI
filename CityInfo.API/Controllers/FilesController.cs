using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new System.ArgumentNullException(
                nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var pathtotFile = "AI-keywords.pdf";

            //Check whether the file exists
            if (!System.IO.File.Exists(pathtotFile))
            {
                return NotFound();
            }

            //We call into fileExtensionContentTypeProvider.TryGetContentType to find the correct content type for our file.
            //For that, we pass through the path to the file and, as an output parameter, ContentType
            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathtotFile, out var contentType))
            {
                //If it cannot be determined, we set it to application/octet‑stream
                contentType = "application/octet-stream";
            }
            var bytes = System.IO.File.ReadAllBytes(pathtotFile);

            return File(bytes, contentType, Path.GetFileName(pathtotFile));
        }


        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            //validate the input. Put a limit in filesize to avoid large upload attacks
            //Only Accepts .pdf files (check content-type)
            if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
            {
                return BadRequest("No file or invalid one has been inputted");
            }

            //it's important that you store files in a safe location, preferably on a separate disk, in a directory without execute privileges
            //it is not a good idea to store the file in the directory hierarchy our web API is hosted in
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"upload_file_{Guid.NewGuid()}.pdf");

            //we initialize a new file stream on this and call CopyToAsync on our inputted file, passing through the stream as a parameter. That should store the file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

    }
}
