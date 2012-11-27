using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Testing.Tests
{
    public class GetDataController : ApiController
    {
        public HttpResponseMessage Post()
        {
            var body = new StreamReader(Request.Content.ReadAsStreamAsync().Result).ReadToEnd();


            Stream stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return new HttpResponseMessage
                       {

                           Content = new StreamContent(stream)
                       };
        }
    }
}