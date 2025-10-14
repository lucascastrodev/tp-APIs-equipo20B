using System.Web.Http;

namespace api_producto
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
         
            config.MapHttpAttributeRoutes();

            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
