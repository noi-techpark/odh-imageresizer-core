﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using LazZiya.ImageResize;
using System.IO;
using AspNetCore.CacheOutput;
using System.Net;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace odh_imageresizer_core
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public ImageController(IWebHostEnvironment env, IConfiguration configuration)            
        {
            Configuration = configuration;
        }

        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 100)]
        [HttpGet, Route("GetImage")]
        public async Task<IActionResult> GetImage(string imageurl, int width = 0, int height = 0)
        {
            try
            {
                using (var img = GetImage(imageurl))
                {
                    var returnimage = img;

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

                    return File(ImageToByteArray(returnimage, returnimage.RawFormat), returnimage.RawFormat.GetMimeType());
                }
            }
            catch(Exception ex)
            {
                return BadRequest("Failed!");
            }
        }

        //Test Method upload to S3 Bucket


        private byte[] ImageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat imgformat)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imgformat);
            return ms.ToArray();
        }

        private Image GetImage(string imageUrl)
        {
            WebClient wc = new WebClient();

            string bucketurl = Configuration["S3BucketUrl"];

            byte[] bytes = wc.DownloadData(bucketurl + imageUrl);
            MemoryStream ms = new MemoryStream(bytes);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

            return img;
        }

              
    }
    
}
