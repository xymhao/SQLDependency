using Owin;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiController;

namespace WindowsServiceHost
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.SuppressDefaultHostAuthentication();
            WebApiConfig.Register(config);

            appBuilder.UseWebApi(config);
        }
    }
}
