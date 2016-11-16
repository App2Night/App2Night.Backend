using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    [Route("api/Image")]
    [Authorize]

    public class ImageController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET api/image
        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var source = Path.Combine(uploads, name);
            if (!_hostingEnvironment.WebRootFileProvider.GetFileInfo(Path.Combine("uploads", name)).Exists) return BadRequest();
            return PhysicalFile(source, "image/png");
        }

        //POST api/Image
        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            var files = Request.Form.Files;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            foreach (IFormFile formFile in files)
            {
                var extension = formFile.ContentType;
                if (extension == "image/png" || extension == "image/jpeg" || extension == "image/jpg")
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, formFile.FileName), FileMode.Create))
                    {
                        await formFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    return BadRequest("Wrong format. Allowed types are png, jpg and jpeg");
                }
            }
            return Ok();
        }
    }
}
