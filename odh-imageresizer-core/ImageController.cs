using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using LazZiya.ImageResize;
using System.IO;
using AspNetCore.CacheOutput;

namespace odh_imageresizer_core
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 100)]
        [HttpGet, Route("GetImage")]
        public async Task<IActionResult> GetImage(string imageurl, int width = 0, int height = 0)
        {
            using (var img = Image.FromFile("c:\\temp\\images\\testbild.jpg"))
            {
                var returnimage = default(System.Drawing.Image);

                if (width > 0 || height > 0)
                {
                    if (width > 0 && height == 0)
                        returnimage = ImageResize.ScaleByWidth(img, width);
                    else if (width == 0 && height > 0)
                        returnimage = img.ScaleByHeight(height);
                    else
                    {
                        returnimage = ImageResize.Scale(img, width, height);
                    }
                }

                return File(ImageToByteArray(returnimage, System.Drawing.Imaging.ImageFormat.Jpeg), "image/jpeg");
            }
        }

        //Test Method upload to S3 Bucket


        private byte[] ImageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat imgformat)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imgformat);
            return ms.ToArray();
        }
    }
}
