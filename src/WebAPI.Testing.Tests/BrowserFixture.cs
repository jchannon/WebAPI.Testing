namespace WebAPI.Testing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using Xunit;

    public class BrowserFixture
    {
        private readonly Browser _browser;

        public BrowserFixture()
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
        public void GetData_WhenRequested_ShouldReturnJSON()
        {
            var browser = new Browser();
            var response = browser.Get("/GetData/Get", (with) =>
                                                           {
                                                               with.Header("Accept", "application/json");
                                                               with.HttpRequest();
                                                           });

            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void Should_be_able_to_send_string_in_body()
        {
            const string thisIsMyRequestBody = "This is my request body";

            // When
            var result = _browser.Post("/GetData/WEE", with =>
            {
                with.HttpRequest();
                with.Body(thisIsMyRequestBody);

            });

            // Then

            Assert.Equal(thisIsMyRequestBody, result.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Should_be_able_to_set_user_host_address()
        {
            // Given
            const string userHostAddress = "127.0.0.1";

            // When
            var result = _browser.Get("/GetData/Get", with =>
                                                         {
                                                             with.HttpRequest();
                                                             with.UserHostAddress(userHostAddress);
                                                         });

            // Then
            Assert.Equal(userHostAddress, result.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Should_be_able_to_send_stream_in_body()
        {
            // Given
            const string thisIsMyRequestBody = "This is my request body";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(thisIsMyRequestBody);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            // When

            var result = _browser.Post("/GetData/WEE", with =>
                                           {
                                               with.HttpRequest();
                                               with.Body(stream, "text/plain");
                                           });

            // Then
            Assert.Equal(thisIsMyRequestBody, result.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Should_be_able_to_send_json_in_body()
        {
            // Given
            var model = new EchoModel { SomeString = "Some String", SomeInt = 29, SomeBoolean = true };

            // When
            var result = _browser.Post("/GetData/WEE", with =>
                                            {
                                                with.JsonBody(model);
                                            });


            // Then
            var actualModel = result.Content.DeserializeJson<EchoModel>();

            Assert.NotNull(actualModel);
            Assert.Equal(model.SomeString, actualModel.SomeString);
            Assert.Equal(model.SomeInt, actualModel.SomeInt);
            Assert.Equal(model.SomeBoolean, actualModel.SomeBoolean);
        }

        [Fact]
        public void Should_be_able_to_send_form_values()
        {
            var result = _browser.Post("/GetData/POO", with =>
                                                      {
                                                          with.FormValue("SomeString", "Some String");
                                                          with.FormValue("SomeInt", "29");
                                                          with.FormValue("SomeBoolean", "true");
                                                      });


            var actualModel = result.Content.DeserializeJson<EchoModel>();

            Assert.NotNull(actualModel);
            Assert.Equal("Some String", actualModel.SomeString);
            Assert.Equal(29, actualModel.SomeInt);
            Assert.Equal(true, actualModel.SomeBoolean);
        }

        [Fact]
        public void Should_add_basic_authentication_credentials_to_the_headers_of_the_request()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.BasicAuth("username", "password");

            // Then
            IBrowserContextValues values = context;

            var credentials = string.Format("{0}:{1}", "username", "password");
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            Assert.Equal(1, values.Headers.Count);
            Assert.Equal("Basic " + encodedCredentials, values.Headers["Authorization"].First());
        }

        [Fact]
        public void Should_add_cookies_to_the_request()
        {
            // Given
            var context = new BrowserContext();

            var cookies =
                new Dictionary<string, string>
                    {
                        {"CookieName", "CookieValue"},
                        {"SomeCookieName", "SomeCookieValue"}
                    };

            // When
            context.Cookie(cookies);

            // Then
            IBrowserContextValues values = context;

            var cookieString = cookies.Aggregate(string.Empty, (current, cookie) => current + string.Format("{0}={1};", HttpUtility.UrlEncode(cookie.Key), HttpUtility.UrlEncode(cookie.Value)));

            Assert.Equal(1, values.Headers["Cookie"].Count());
            Assert.Equal(cookieString, values.Headers["Cookie"].First());

        }

        [Fact]
        public void Should_add_cookie_to_the_request()
        {
            // Given
            var context = new BrowserContext();

            var cookies =
                new Dictionary<string, string>
                    {
                        {"CookieName", "CookieValue"},
                        {"SomeCookieName", "SomeCookieValue"}
                    };

            // When
            foreach (var cookie in cookies)
            {
                context.Cookie(cookie.Key, cookie.Value);
            }

            // Then
            IBrowserContextValues values = context;

            var cookieString = cookies.Aggregate(string.Empty, (current, cookie) => current + string.Format("{0}={1};", HttpUtility.UrlEncode(cookie.Key), HttpUtility.UrlEncode(cookie.Value)));

            Assert.Equal(1, values.Headers["Cookie"].Count());
            Assert.Equal(cookieString, values.Headers["Cookie"].First());

        }

        [Fact]
        public void Should_add_cookies_to_the_request_and_get_cookies_in_response()
        {
            // Given
            var cookies =
                new Dictionary<string, string>
                    {
                        {"CookieName", "CookieValue"},
                        {"SomeCookieName", "SomeCookieValue"}
                    };

            // When
            var result = _browser.Get("/GetData/Get/cookie", with =>
            {
                with.Cookie(cookies);
            });

            // Then
            var values = result.Headers.Single(x => x.Key == "Set-Cookie").Value.First().Split(';');

            Assert.Equal(2, values.Count());

            Assert.Equal(cookies.First().Key, values[0].Substring(0, values[0].IndexOf("=")));
            Assert.Equal(cookies.First().Value, values[0].Substring(values[0].IndexOf("=") + 1));

            Assert.Equal(cookies.Last().Key, values[1].Substring(0, values[1].IndexOf("=")).Trim());  //Have to trim as they are seperated by a whitespace
            Assert.Equal(cookies.Last().Value, values[1].Substring(values[1].IndexOf("=") + 1).Trim());

        }

        [Fact]
        public void Should_add_a_cookie_to_the_request_and_get_a_cookie_in_response()
        {
            // Given, When
            var result = _browser.Get("/GetData/Get/cookie", with => with.Cookie("CookieName", "CookieValue"));

            // Then

            var values = result.Headers.Single(x => x.Key == "Set-Cookie").Value.First().Split(';');

            Assert.Equal(1, values.Count());
            Assert.Equal("CookieName", values[0].Substring(0, values[0].IndexOf("=")));
            Assert.Equal("CookieValue", values[0].Substring(values[0].IndexOf("=") + 1));
        }

        //[Fact]
        //public void Should_be_able_to_continue_with_another_request()
        //{
        //    // Given
        //    const string FirstRequestBody = "This is my first request body";
        //    const string SecondRequestBody = "This is my second request body";
        //    var firstRequestStream = new MemoryStream();
        //    var firstRequestWriter = new StreamWriter(firstRequestStream);
        //    firstRequestWriter.Write(FirstRequestBody);
        //    firstRequestWriter.Flush();
        //    var secondRequestStream = new MemoryStream();
        //    var secondRequestWriter = new StreamWriter(secondRequestStream);
        //    secondRequestWriter.Write(SecondRequestBody);
        //    secondRequestWriter.Flush();

        //    // When
        //    var result = browser.Post("/", with =>
        //    {
        //        with.HttpRequest();
        //        with.Body(firstRequestStream, "text/plain");
        //    }).Then.Post("/", with =>
        //    {
        //        with.HttpRequest();
        //        with.Body(secondRequestStream, "text/plain");
        //    });

        //    // Then
        //    result.Body.AsString().ShouldEqual(SecondRequestBody);
        //}

        //    [Fact]
        //    public void Should_maintain_cookies_when_chaining_requests()
        //    {
        //        // Given
        //        // When
        //        var result = browser.Get(
        //                "/session",
        //                with => with.HttpRequest())
        //            .Then
        //            .Get(
        //                "/session",
        //                with => with.HttpRequest());

        //        result.Body.AsString().ShouldEqual("Current session value is: I've created a session!");
        //    }

        //    [Fact]
        //    public void Should_maintain_cookies_even_if_not_set_on_directly_preceding_request()
        //    {
        //        // Given
        //        // When
        //        var result = browser.Get(
        //                "/session",
        //                with => with.HttpRequest())
        //            .Then
        //            .Get(
        //                "/nothing",
        //                with => with.HttpRequest())
        //            .Then
        //            .Get(
        //                "/session",
        //                with => with.HttpRequest());

        //        result.Body.AsString().ShouldEqual("Current session value is: I've created a session!");
        //    }

        [Fact]
        public void Should_be_able_to_not_specify_delegate_for_basic_http_request()
        {
            var result = _browser.Get("/GetData/Get/scheme");

            Assert.Equal("http", result.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Should_add_ajax_header()
        {
            var result = _browser.Get("/GetData/Get/ajax", with => with.AjaxRequest());

            Assert.Equal("ajax", result.Content.ReadAsStringAsync().Result);
        }

        //    [Fact]
        //    public void Should_add_forms_authentication_cookie_to_the_request()
        //    {
        //        var userId = A.Dummy<Guid>();

        //        var formsAuthConfig = new FormsAuthenticationConfiguration()
        //        {
        //            RedirectUrl = "/login",
        //            UserMapper = A.Fake<IUserMapper>(),
        //        };

        //        var encryptedId = formsAuthConfig.CryptographyConfiguration.EncryptionProvider.Encrypt(userId.ToString());
        //        var hmacBytes = formsAuthConfig.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedId);
        //        var hmacString = Convert.ToBase64String(hmacBytes);
        //        var cookieContents = String.Format("{1}{0}", encryptedId, hmacString);

        //        var response = browser.Get("/cookie", (with) =>
        //        {
        //            with.HttpRequest();
        //            with.FormsAuth(userId, formsAuthConfig);
        //        });

        //        var cookie = response.Cookies.Single(c => c.Name == FormsAuthentication.FormsAuthenticationCookieName);
        //        var cookieValue = HttpUtility.UrlDecode(cookie.Value);
        //        cookieValue.ShouldEqual(cookieContents);
        //    }
    }
}
