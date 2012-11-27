using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.Testing.Tests
{
    public class FakeHandler : DelegatingHandler
    {
        public HttpResponseMessage Response { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Response == null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            return Task.Factory.StartNew(() => Response);
        }
    }
}