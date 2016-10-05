namespace WebAPI.Testing.Tests
{
    using System.Net;
    using System.Web.Http;
    using Xunit;

    public class ReusableBrowserFixture
    {
        private readonly ReusableBrowser _browser;

        public ReusableBrowserFixture()
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "ControllerAndActionfwefew",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            _browser = new ReusableBrowser(config);
        }

        [Fact]
        public void Can_Send_MultipleRequests_ToSingle_Instance()
        {
            var response = _browser.Get("/GetData/Get", (with) =>
            {
                with.Header("Accept", "application/json");
                with.HttpRequest();
            });

            Assert.Equal(response.StatusCode, HttpStatusCode.OK);

            var secondResponse = _browser.Get("/GetData/Get", (with) =>
            {
                with.Header("Accept", "application/json");
                with.HttpRequest();
            });

            Assert.Equal(secondResponse.StatusCode, HttpStatusCode.OK);
        }
    }
}