namespace WebAPI.Testing.Tests
{
    using System;
    using System.Net;
    using System.Web.Http;
    using Xunit;
    using RouteParameter = System.Web.Http.RouteParameter;

    public class BrowserExtensionsFixture
    {
        private readonly Browser _browser;

        public BrowserExtensionsFixture()
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "ControllerAndActionfwefew",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            _browser = new Browser(config);
        }

        [Fact]
        public void SouldBeAbleToPostJsonWithExtensions()
        {
            var response = _browser.PostAsJson("/json/post", new ADocument());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void PostingAsJsonCanStillSepcifyContextOptions()
        {
            string randomValue = Guid.NewGuid().ToString();
            var response = _browser.PostAsJson("/json/post", new ADocument(), with =>
            {
                with.Header("test", randomValue);
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(JsonController.TestHeaderValue, randomValue);
        }
    }
}