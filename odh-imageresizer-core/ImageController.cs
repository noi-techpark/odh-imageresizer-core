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
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;

namespace odh_imageresizer_core
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        private readonly IHttpClientFactory _httpClientFactory;

        public ImageController(IWebHostEnvironment env, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 100)]
        [HttpGet, Route("GetImage")]
        public async Task<IActionResult> GetImage(string imageurl, int? width = null, int? height = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var (img, imgrawformat) = await GetImage(imageurl, cancellationToken);
                using var _ = img; // Lazy way to dispose the image resource ;)

                if (width != null || height != null)
                {
                    (int w, int h) = (width ?? 0, height ?? 0);
                    float ratio = (float)img.Width / img.Height;
                    var size = (w > h)
                        ? new Size(w, (int)(w / ratio))
                        : new Size((int)(h * ratio), h);

                    img.Mutate(ctx =>
                    {
                        ctx.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = size
                        });
                    });
                }

                var stream = await ImageToStream(img, imgrawformat, cancellationToken);
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
            IImageEncoder encoder = ConfigureImageEncoder(imgformat);

            var ms = new MemoryStream();
            await imageIn.SaveAsync(ms, encoder, cancellationToken);
            ms.Position = 0;
            return ms;
        }

        private static IImageEncoder ConfigureImageEncoder(IImageFormat imgformat)
        {
            var mngr = SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager;
            var encoder = mngr.FindEncoder(imgformat);
            if (encoder is JpegEncoder jpegEncoder)
            {
                jpegEncoder.Quality = 90;
            }
            else if (encoder is PngEncoder pngEncoder)
            { }
            else if (encoder is GifEncoder gifEncoder)
            { }

            return encoder;
        }

        private async Task<(Image, IImageFormat)> GetImage(string imageUrl, CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient("buckets");
            using var stream = await client.GetStreamAsync(imageUrl, cancellationToken);
            var img = Image.Load(stream, out var imageFormat);
            return (img, imageFormat);
        }
    }
    
}
