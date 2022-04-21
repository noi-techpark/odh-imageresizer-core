using Microsoft.AspNetCore.Mvc;
using AspNetCore.Proxy;
using System.Threading.Tasks;
using System;

namespace odh_imageresizer_core
{
    [Route("api/[controller]")]
    [ApiController]
    public class ODHProxyController : ControllerBase
    {
        [HttpGet, Route("ODHProxy/{*url}")]
        public Task GetODHProxy(string url)
        {
            try
            {
                var parameter = "?";

                foreach (var paramdict in HttpContext.Request.Query)
                {
                    parameter = parameter + paramdict.Key + "=" + paramdict.Value;
                }

                var fullurl = url + parameter;

                //Quick production fix
                fullurl = fullurl.Replace("https:/", "https://");

                Console.WriteLine("Url to proxy: " + fullurl);

                return this.HttpProxyAsync(fullurl);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
    }
}
