WebAPI.Testing
==============

Provides a way to test WebAPI projects.

    public class GetDataTests
    {
        [Fact]
        public void GetData_WhenRequested_ShouldReturnJSON()
        {
            var browser = new Browser();
            var response = browser.Get("/GetData", (with) =>
            {
                with.Header("Accept", "application/json");
                with.HttpRequest();
            });

            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void GetData_WhenRequested_ShouldReturnOKStatusCode()
        {
            var browser = new Browser();
            var response = browser.Get("/GetData", (with) =>
            {
                with.Header("Authorization", "Bearer johnsmith");
                with.Header("Accept", "application/json");
                with.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }

This project is a modified version of [Nancy.Testing][1] but built to work with ASP.Net WebAPI

  [1]: https://github.com/NancyFx/Nancy/tree/master/src/Nancy.Testing

---

To build nuget packages open a Developer Command Prompt and run the 

>buildandpack.cmd

command.