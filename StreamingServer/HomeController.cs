using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;
using System.IO.Compression;

namespace sample.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        
        // Metode til at levere master playlists
        [Authorize(Policy = "ReadAccess")]
        [Route("{fileName}")]
        public IActionResult GetMasterPlaylist(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (extension != ".m3u8") 
            {
                return NotFound();
            }

            var file = Path.Combine(Directory.GetCurrentDirectory(), 
                                    "StaticFiles", "audio", fileName);

            if (!System.IO.File.Exists(file))
            {
                return NotFound();

            }
         
            Console.WriteLine(fileName + " afsendes.");
            return PhysicalFile(file, "application/x-mpegURL");
 
        }

        // Metode til at levere variant-playlists og segmenter        
        [Authorize (Policy = "ReadAccess")]
        [Route("{folderName}/{fileName}")]
        public IActionResult GetFiles(string folderName, string fileName)
        {
           
            var file = Path.Combine(Directory.GetCurrentDirectory(), 
                                    "StaticFiles", "audio", folderName, fileName);
           
            if (!System.IO.File.Exists(file))
            {
                return NotFound();
            }

            var extension = Path.GetExtension(fileName);

            if (extension == ".m3u8") 
            {
                Console.WriteLine(folderName + "/" + fileName + " afsendes");
                return PhysicalFile(file, "application/x-mpegURL");

            } else if (extension == ".ts")
            {
                Console.WriteLine(folderName + "/" + fileName + " afsendes");
                return PhysicalFile(file, "video/MP2T");

            } else 
            {
                return NotFound();
            }
        }

        // Metode til at uploade filer med
        [Authorize (Policy = "WriteAccess")]
        [HttpPost]
        [RequestSizeLimit(200_000_000)]
        public async Task<IActionResult> UploadTrack([FromForm(Name = "file")] IFormFile file)
        {
            var trackId = Path.GetFileNameWithoutExtension(file.FileName);

            var staticFolder = Path.Combine(Directory.GetCurrentDirectory(), 
                                    "StaticFiles");

            var zipFolder = Path.Combine(staticFolder, "ZipFiles");

            var audioFolder = Path.Combine(staticFolder, "audio");

            var filePath = await SaveFile(file, zipFolder);
          
            try
            {
                ZipFile.ExtractToDirectory(filePath, zipFolder);

                System.IO.File.Move(zipFolder + "/" + trackId + ".m3u8", audioFolder + "/" + trackId + ".m3u8");

                Directory.Move(zipFolder + "/" + trackId + "_v0", audioFolder + "/"  + trackId + "_v0");
                Directory.Move(zipFolder + "/" + trackId + "_v1", audioFolder + "/" + trackId + "_v1");

                return Created("", null);

            } catch(Exception e)
            {
                Console.WriteLine("Zip-filen kunne ikke udpakkes: " + e);
                
                return StatusCode(StatusCodes.Status500InternalServerError);                
            }
            finally
            {
                // Slet zip-fil
                System.IO.File.Delete(filePath);
            }
        }

        private async Task<string> SaveFile(IFormFile file, string targetFolder)
        {
            if (file.Length <= 0) 
            {
                return "";
            }
                var filePath = Path.Combine(targetFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filePath;

        }

    }

}