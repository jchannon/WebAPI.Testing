namespace WebAPI.Testing
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;

    public static class BrowserExtensions
    {
        public static HttpResponseMessage PostAsJson(this Browser @this, string path, object body,
            Action<BrowserContext> browserContext = null)
        {
            var jsonBody = JsonConvert.SerializeObject(body);
            HttpResponseMessage result = @this.Post(path, with =>
            {
                with.HttpRequest();
                with.Body(jsonBody);
                with.Header("Content-Type", "application/json");
                if (browserContext != null)
                    browserContext(with);
            });
            return result;
        }
    }
}
