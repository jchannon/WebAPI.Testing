namespace WebAPI.Testing
{
    using System.Web.Http;

    public class ReusableBrowser : Browser
    {
        public ReusableBrowser() : base(false)
        {
        }

        public ReusableBrowser(HttpConfiguration httpConfiguration) : base(httpConfiguration, false)
        {
        }
    }
}