using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Testing.Tests
{
    public class GetDataController : ApiController
    {
        [ActionName("WEE")]
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

        [ActionName("POO")]
        public EchoModel PostBinderTHISISSHIT(EchoModel echoModel)
        {

            return echoModel;
        }

        public HttpResponseMessage Get()
        {
            //I would return return Request.RequestUri.Host; but by default API wont return text/plain

            return new HttpResponseMessage() { Content = new StringContent(Request.RequestUri.Host) };
        }

        public HttpResponseMessage Get(string id)
        {
            switch (id)
            {
                case "scheme":
                    return new HttpResponseMessage() { Content = new StringContent(Request.RequestUri.Scheme.ToLower()) };
                    break;
                case "cookie":
                    var response = new HttpResponseMessage() { Content = new StringContent("Cookies") };

                    response.Headers.AddCookies(Request.Headers.GetCookies());

                    return response;

                    break;
                case "ajax":

                    return new HttpResponseMessage()
                         {
                             Content =
                                 new StringContent(this.Request.Headers.Contains("X-Requested-With") ? "ajax" : "not-ajax")
                         };
                    break;
                default:
                    return new HttpResponseMessage() { Content = new StringContent("Stupid WebAPI doesnt allow more than one Get hence this monstrosity") };
                    break;
            }
        }
    }
}