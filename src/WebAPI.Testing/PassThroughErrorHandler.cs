using System;
using System.Net;
using System.Web.Http.Controllers;

namespace Nancy.Testing
{
    public class PassThroughErrorHandler : IErrorHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, HttpControllerContext context)
        {
            if (!context.Items.ContainsKey(NancyEngine.ERROR_EXCEPTION))
            {
                return false;
            }

            var exception = context.Items[NancyEngine.ERROR_EXCEPTION] as Exception;

            return statusCode == HttpStatusCode.InternalServerError && exception != null;
        }

        public void Handle(HttpStatusCode statusCode, HttpControllerContext context)
        {
            throw new Exception("ConfigurableBootstrapper Exception", context.Items[NancyEngine.ERROR_EXCEPTION] as Exception);
        }
    }
}