namespace WebAPI.Testing.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class JsonController : ApiController
    {
        public HttpResponseMessage Post(ADocument documentToStore)
        {
            if (Request.Headers.Contains("test"))
            {
                var testHeaderValues = Request.Headers.GetValues("test");
                TestHeaderValue = testHeaderValues.First();
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public static string TestHeaderValue { get; private set; }
    }

    public class ADocument
    {
        public string Name { get; set; }
        public Dictionary<string, int> ChapterIndex { get; set; }
    }
}