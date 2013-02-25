using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;


namespace WebAPI.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides the capability of executing a request with WebAPI, using a specific configuration provided by an <see cref="IWebAPIBootstrapper"/> instance.
    /// </summary>
    public class Browser : IHideObjectMembers
    {
        private readonly HttpServer _server;

        private readonly IDictionary<string, string> cookies = new Dictionary<string, string>();

        public HttpClient BrowserHttpClient { get; set; }

        public Browser()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(name: "Default", routeTemplate: "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            _server = new HttpServer(config);
        }

        public Browser(HttpConfiguration httpConfiguration)
        {
            HttpConfiguration config = httpConfiguration;
            _server = new HttpServer(config);
        }

        /// <summary>
        /// Performs a DELETE requests against WebAPI.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        public HttpResponseMessage Delete(string path, Action<BrowserContext> browserContext = null)
        {
            return this.HandleRequest(HttpMethod.Delete, path, browserContext);
        }

        /// <summary>
        /// Performs a GET requests against WebAPI.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        public HttpResponseMessage Get(string path, Action<BrowserContext> browserContext = null)
        {
            return this.HandleRequest(HttpMethod.Get, path, browserContext);
        }

        /// <summary>
        /// Performs a HEAD requests against WebAPI.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        public HttpResponseMessage Head(string path, Action<BrowserContext> browserContext = null)
        {
            return this.HandleRequest(HttpMethod.Head, path, browserContext);
        }

        /// <summary>
        /// Performs a OPTIONS requests against WebAPI.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        public HttpResponseMessage Options(string path, Action<BrowserContext> browserContext = null)
        {
            return this.HandleRequest(HttpMethod.Options, path, browserContext);
        }

        ///// <summary>
        ///// Performs a PATCH requests against WebAPI.
        ///// </summary>
        ///// <param name="path">The path that is being requested.</param>
        ///// <param name="browserContext">An closure for providing browser context for the request.</param>
        ///// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        //public HttpResponseMessage Patch(string path, Action<BrowserContext> browserContext = null)
        //{
        //    return this.HandleRequest(HttpMethod., path, browserContext);
        //}

        /// <summary>
        /// Performs a POST requests against WebAPI.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        public HttpResponseMessage Post(string path, Action<BrowserContext> browserContext = null)
        {
            return this.HandleRequest(HttpMethod.Post, path, browserContext);
        }

        /// <summary>
        /// Performs a PUT requests against WebAPI.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> instance of the executed request.</returns>
        public HttpResponseMessage Put(string path, Action<BrowserContext> browserContext = null)
        {
            return this.HandleRequest(HttpMethod.Put, path, browserContext);
        }

        private HttpResponseMessage HandleRequest(HttpMethod method, string path, Action<BrowserContext> browserContext)
        {
            var request =
                CreateRequest(method, path, browserContext ?? this.DefaultBrowserContext);

            //this.CaptureCookies(response);

            if (BrowserHttpClient == null)
                BrowserHttpClient = new HttpClient(_server);

            //var client = new HttpClient(_server);
            HttpResponseMessage response = BrowserHttpClient.SendAsync(request).Result;

            request.Dispose();

            if (_server != null)
            {
                _server.Dispose();
            }

            return response;
        }



        private void DefaultBrowserContext(BrowserContext context)
        {
            context.HttpRequest();
        }

        private void SetCookies(BrowserContext context)
        {
            if (!this.cookies.Any())
            {
                return;
            }

            var cookieString = this.cookies.Aggregate(string.Empty, (current, cookie) => current + string.Format("{0}={1};", HttpUtility.UrlEncode(cookie.Key), HttpUtility.UrlEncode(cookie.Value)));

            context.Header("Cookie", cookieString);
        }

        private void CaptureCookies(HttpResponseMessage response)
        {
            IEnumerable<string> cookies = null;
            if (!response.Headers.TryGetValues("Set-Cookie", out cookies))
            {
                return;
            }

            //if (response.Cookies == null || !response.Cookies.Any())
            //{
            //    return;
            //}

            //foreach (var cookie in cookies)
            //{

            //    if (string.IsNullOrEmpty(cookie.))
            //    {
            //        this.cookies.Remove(cookie.Name);
            //    }
            //    else
            //    {
            //        this.cookies[cookie.Name] = cookie.Value;
            //    }
            //}
        }

        private static void BuildRequestBody(IBrowserContextValues contextValues)
        {
            if (contextValues.Body != null)
            {
                return;
            }

            var useFormValues = !String.IsNullOrEmpty(contextValues.FormValues);
            var bodyContents = useFormValues ? contextValues.FormValues : contextValues.BodyString;
            var bodyBytes = bodyContents != null ? Encoding.UTF8.GetBytes(bodyContents) : new byte[] { };

            if (useFormValues && !contextValues.Headers.ContainsKey("Content-Type"))
            {
                contextValues.Headers["Content-Type"] = new[] { "application/x-www-form-urlencoded" };
            }

            contextValues.Body = new MemoryStream(bodyBytes);
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string path, Action<BrowserContext> browserContext)
        {
            var context =
                new BrowserContext();

            this.SetCookies(context);

            browserContext.Invoke(context);

            var contextValues =
                (IBrowserContextValues)context;

            BuildRequestBody(contextValues);

            //var requestStream =
            //    RequestStream.FromStream(contextValues.Body, 0, true);

            var request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = new Uri(contextValues.Protocol + "://" + contextValues.UserHostAddress + path + contextValues.QueryString);

            request.Content = new StreamContent(contextValues.Body);

            foreach (var header in contextValues.Headers)
            {

                if (header.Key.StartsWith("Content") && !request.Content.Headers.Contains(header.Key))
                {
                    request.Content.Headers.Add(header.Key, header.Value);
                }
                else if (!header.Key.StartsWith("Content") && !request.Headers.Contains(header.Key))
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }


    }
}