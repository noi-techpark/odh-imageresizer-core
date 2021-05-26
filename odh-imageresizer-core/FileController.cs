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
using AspNetCore.Proxy;

namespace odh_imageresizer_core
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        private readonly IHttpClientFactory _httpClientFactory;

        public FileController(IWebHostEnvironment env, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        #region Documents

        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 100)]
        [HttpGet, Route("GetFile/{filename}")]
        public async Task<IActionResult> GetFile(string filename, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("buckets");

                var response = await client.GetAsync(filename, cancellationToken);

                var mimeType = response.Content.Headers.ContentType;                

                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                                
                return File(stream, mimeType.MediaType);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message + " " + ex.InnerException);
            }
        }

        //USING AWS CLIENT
        //[CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 100)]
        //[HttpGet, Route("GetFile2/{filename}")]
        //public async Task<IActionResult> GetFile2(string filename, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var creds = new BasicAWSCredentials(keyid, key);
        //        var config = new AmazonS3Config();
        //        config.RegionEndpoint = RegionEndpoint.EUWest1;
        //        var client = new AmazonS3Client(creds, config);

        //        //
        //        var response = await client.GetAsync(filename, cancellationToken);

        //        var mimeType = response.Content.Headers.ContentType;

        //        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        //        //var stream = await client.GetStreamAsync(filename, cancellationToken);        

        //        var extension = mimeType != null ? mimeType.MediaType : "text/plain";

        //        //mimeType returned is wrong!!

        //        return File(stream, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message + " " + ex.InnerException);
        //    }
        //}

     


        #endregion
    }
}
