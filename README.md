WebAPI.Testing
==============

Provides a way to test WebAPI projects.

    public class GetDataTests
    {
        private HttpConfiguration CreateConfiguration()
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}"
            );

                config.MessageHandlers.Add(new RequireAuthentication(new UserApiMapper()));

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            return config;
        }

        [Fact]
        public void GetData_WhenRequested_ShouldReturnJSON()
        {
            var config = CreateConfiguration();
            var browser = new Browser(config);
            var response = browser.Get("/GetData", (with) =>
            {
                with.Header("Authorization", "Bearer fred");
                with.Header("Accept", "application/json");
                with.HttpRequest();
            });

            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void GetData_WhenRequested_ShouldReturnOKStatusCode()
        {
            var config = CreateConfiguration();
            var browser = new Browser(config);
            var response = browser.Get("/GetData", (with) =>
            {
                with.Header("Authorization", "Bearer fred");
                with.Header("Accept", "application/json");
                with.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void GetData_NoAuth_ShouldReturnUnauthorizedStatusCode()
        {
            var config = CreateConfiguration();
            var browser = new Browser(config);
            var response = browser.Get("/GetData");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }