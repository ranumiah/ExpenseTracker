using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExpenseTracker.API
{
    public static class WebApiConfig
    {
        public static HttpConfiguration Register()
        {
            var config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultRouting",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
                );


            // clear the supported mediatypes of the xml formatter
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(
                new MediaTypeHeaderValue("application/json-patch+json"));


            // ... or ensure the json formatter accepts text/html

            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // results should come out
            // - with indentation for readability
            // - in camelCase

            JsonMediaTypeFormatter json = config.Formatters.JsonFormatter;
            json.SerializerSettings.Formatting = Formatting.Indented;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            // configure caching
            // This will create an ETag on indivisual resource.
            // If the header has below then it will not create another resource but use the caching one
            // If-None-Match: "f18b36d3af37405bb42b687010db3f73"
            config.MessageHandlers.Add(new CacheCow.Server.CachingHandler(config));

            return config;
        }
    }
}