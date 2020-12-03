using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using AspNetCore.CacheOutput;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

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
        public async Task<IActionResult> GetImage(string imageurl, int? width = null, int? height = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var (img, imgrawformat) = await GetImage(imageurl, cancellationToken);
                using var _ = img; // Lazy way to dispose the image resource ;)

                img.Mutate(ctx =>
                {
                    ctx.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(width ?? height ?? 0, height ?? width ?? 0)
                    });
                });

                using var stream = await ImageToStream(img, imgrawformat, cancellationToken);
                return File(stream, imgrawformat.DefaultMimeType);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message + " " + ex.InnerException);
            }
        }

        //Test Method upload to S3 Bucket

        private async Task<Stream> ImageToStream(Image imageIn, IImageFormat imgformat, CancellationToken cancellationToken = default)
        {
            var ms = new MemoryStream();
            await imageIn.SaveAsync(ms, imgformat, cancellationToken);
            return ms;
        }

        private async Task<(Image, IImageFormat)> GetImage(string imageUrl, CancellationToken cancellationToken)
        {
            string bucketurl = Configuration["S3BucketUrl"] ?? throw new InvalidProgramException("No S3 Bucket URL provided.");

            using var client = new HttpClient(); // TODO: use HttpClientFactory
            byte[] bytes = await client.GetByteArrayAsync(bucketurl + imageUrl, cancellationToken);
            var img = Image.Load(bytes, out var imageFormat);
            return (img, imageFormat);
        }
    }
    
}
