// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

﻿using Microsoft.AspNetCore.Mvc;
using AspNetCore.Proxy;
using System.Threading.Tasks;
using System;
using AspNetCore.CacheOutput;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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

                return this.HttpProxyAsync(fullurl);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 0)]
        [HttpGet, Route("ODHProxyCachedonClient/{*url}")]
        public Task GetODHProxyCachedonClient(string url)
        {
            try
            {
                var parameter = "?";

                foreach (var paramdict in HttpContext.Request.Query)
                {
                    parameter = parameter + paramdict.Key + "=" + paramdict.Value;
                }

                var fullurl = url + parameter;

                return this.HttpProxyAsync(fullurl);
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }


        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 14400)]
        [HttpGet, Route("ODHProxyCustomCached/{contenttype}/{*url}")]
        public async Task<IActionResult> GetODHProxyCustomCached(string contenttype, string url)
        {
            var parameter = "?";

            foreach (var paramdict in HttpContext.Request.Query)
            {
                parameter = parameter + paramdict.Key + "=" + paramdict.Value;
            }

            var fullurl = url + parameter;

            var _client = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false
            });
            
            var request = HttpContext.CreateProxyHttpRequest(new Uri(fullurl));
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            await HttpContext.CopyProxyHttpResponse(response, contenttype);
            return Ok();
        }

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 0)]
        [HttpGet, Route("ODHProxyCustomCachedonClient/{contenttype}/{*url}")]
        public async Task<IActionResult> GetODHProxyCustomCachedonClient(string contenttype, string url)
        {
            var parameter = "?";

            foreach (var paramdict in HttpContext.Request.Query)
            {
                parameter = parameter + paramdict.Key + "=" + paramdict.Value;
            }

            var fullurl = url + parameter;

            var _client = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false
            });

            var request = HttpContext.CreateProxyHttpRequest(new Uri(fullurl));
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            await HttpContext.CopyProxyHttpResponse(response, contenttype);
            return Ok();
        }

        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 14400)]
        [HttpGet, Route("ODHProxyCustomCachedonServer/{contenttype}/{*url}")]
        public async Task<IActionResult> GetODHProxyCustomCachedonServer(string contenttype, string url)
        {
            var parameter = "?";

            foreach (var paramdict in HttpContext.Request.Query)
            {
                parameter = parameter + paramdict.Key + "=" + paramdict.Value;
            }

            var fullurl = url + parameter;

            var _client = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false
            });

            var request = HttpContext.CreateProxyHttpRequest(new Uri(fullurl));
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            await HttpContext.CopyProxyHttpResponse(response, contenttype);
            return Ok();
        }
    }


    public static class MyHttpContextExtensions
    {
        public static HttpRequestMessage CreateProxyHttpRequest(this HttpContext context, Uri uri)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage();
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            //Remove the Accept-Encoding Header
            requestMessage.Headers.Remove("Accept-Encoding");

            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            return requestMessage;
        }

        public static async Task CopyProxyHttpResponse(this HttpContext context, HttpResponseMessage responseMessage, string? usecustomcontenttype = "")
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;

            
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("Transfer-Encoding");
            response.Headers.Remove("Connection");
            response.Headers.Remove("Cache-Control");
            response.Headers.Remove("Pragma");
            response.Headers.Remove("Expires");
            response.Headers.Remove("Vary");
            response.Headers.Remove("set-cookie");
            response.Headers.Remove("CF-Cache-Status");
            response.Headers.Remove("Expect-CT");
            response.Headers.Remove("Server");
            response.Headers.Remove("CF-RAY");
            response.Headers.Remove("Date");
            response.Headers.Remove("Content-Encoding");


            //https://localhost:44373/api/ODHProxyCustomCached/https://www.dolomitisuperski.com/file/?uuidLift=01e04d73-8ab9-44b8-9b37-d693d4ddf49f

            var contenttypetouse = GetCorrectContentType(usecustomcontenttype);

            //"application/vnd.google-earth.kml+xml"
            if (!string.IsNullOrEmpty(contenttypetouse) && contenttypetouse != "nochange")
            {
                response.Headers.Remove("content-type");
                response.Headers.Add("content-type", contenttypetouse);
            }                

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(response.Body, 4096, context.RequestAborted);
            }
        }


        private static string GetCorrectContentType(string contenttype) =>        
            contenttype switch
            {
                "kml" => "application/vnd.google-earth.kml+xml",
                _ => "nochange"
            };
        

    }
}
