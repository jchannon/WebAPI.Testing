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

        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage() { Content = new StringContent(Request.RequestUri.Host) };
        }

        public HttpResponseMessage Get(string id)
        {
            switch (id)
            {
                case "scheme":
                    return new HttpResponseMessage() { Content = new StringContent(Request.RequestUri.Scheme.ToLower()) };
                    break;
                case "ajax":
                   
                   return  new HttpResponseMessage()
                        {
                            Content =
                                new StringContent(this.Request.Headers.Contains("X-Requested-With") ? "ajax" : "not-ajax")
                        };
                    break;
                default:
                    return new HttpResponseMessage() { Content = new StringContent("Stupid WenAPI doesnt allow more than one Get hence this monstrosity") };
                    break;
            }
        }
    }
}