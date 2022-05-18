using Microsoft.AspNetCore.Mvc;
using AspNetCore.Proxy;
using System.Threading.Tasks;
using System;
using AspNetCore.CacheOutput;

namespace odh_imageresizer_core
{
    [Route("api")]
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

                Console.WriteLine("Url to proxy: " + fullurl);

                return this.HttpProxyAsync(fullurl);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 14400)]
        [HttpGet, Route("ODHProxyCached/{*url}")]
        public Task GetODHProxyCached(string url)
        {
            try
            {
                var parameter = "?";

                foreach (var paramdict in HttpContext.Request.Query)
                {
                    parameter = parameter + paramdict.Key + "=" + paramdict.Value;
                }

                var fullurl = url + parameter;

                Console.WriteLine("Url to proxy: " + fullurl);

                //AspNetCore.Proxy.Options.HttpProxyOptions proxyoptions = new AspNetCore.Proxy.Options.HttpProxyOptions()
                //{
                    
                //}

                return this.HttpProxyAsync(fullurl);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
    }
}
