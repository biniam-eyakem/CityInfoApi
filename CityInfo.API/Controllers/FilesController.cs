using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            this._fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ??
                throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }
        [HttpGet]
        public ActionResult GetFile(int fileId)
        {
            var filePath = "files/acg.pdf";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            //string fileContentType = string.Empty;
            if (!this._fileExtensionContentTypeProvider.TryGetContentType(filePath, out var fileContentType))
            {
                fileContentType = "application/octet-stream";
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, fileContentType, Path.GetFileName(filePath));
        }
    }
}
