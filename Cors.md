# Making cross-origin requests for client-side files (CORS)

If you use JavaScript to request certain restricted file types from other domains or subdomains (typically JSON data), by default that request is blocked because the other domain or subdomain is considered a separate origin, and cross-origin requests are not allowed. 

If the origin the files are being requested from is expecting the request, it can serve an HTTP header containing the name of the requesting domain, and the request is then allowed. See [Cross-origin resource sharing on WikiPedia](https://en.wikipedia.org/wiki/Cross-origin_resource_sharing) for more detail.

Using this project you can add a semi-colon-separated list of allowed domains in `web.config`. This can contain a wildcard for the port number:

	<configSections>
	  <sectionGroup name="Escc.Web">
	    <section name="Cors" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	  </sectionGroup>
	</configSections>
	
	<Escc.Web>
	  <Cors>
	    <add key="AllowedOrigins" value="https://subdomain1.example.org;https://subdomain2.example.org;https://localhost:*" />
	  </Cors>
	</Escc.Web>

Any code which returns a response which might be blocked by cross-origin rules can check easily whether the requesting domain is allowed and, if so, return the correct CORS header.

    var context = HttpContext.Current;
	var corsPolicy = new CorsPolicyFromConfig().CorsPolicy;
	new CorsHeaders(context.Request.Headers, context.Response.Headers, corsPolicy).UpdateHeaders();

In a Web API project the same syntax works, or you can enable CORS support in the `WebApiConfig` class:

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
			…
		}
	}

…and then apply the CORS policy using an attribute at either a class or method level:

	[CorsPolicyFromConfig]
    public class ExampleController : ApiController
    {
		…
	}
