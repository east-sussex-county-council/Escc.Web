# Making cross-origin requests for client-side files (CORS)

If you use JavaScript to request certain restricted file types from other domains or subdomains (typically JSON data), by default that request is blocked because the other domain or subdomain is considered a separate origin, and cross-origin requests are not allowed. 

If the origin the files are being requested from is expecting the request, it can serve an HTTP header containing the name of the requesting domain, and the request is then allowed. See [Cross-origin resource sharing on WikiPedia](https://en.wikipedia.org/wiki/Cross-origin_resource_sharing) for more detail.

Using this project you can add a semi-colon-separated list of allowed domains in `web.config`:

	<configSections>
	  <sectionGroup name="EsccWebTeam.Data.Web">
	    <section name="Cors" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	  </sectionGroup>
	</configSections>
	
	<EsccWebTeam.Data.Web>
	  <Cors>
	    <add key="AllowedOrigins" value="https://subdomain1.example.org;https://subdomain2.example.org" />
	  </Cors>
	</EsccWebTeam.Data.Web>

Any code which returns a response which might be blocked by cross-origin rules can check easily whether the requesting domain is allowed and, if so, return the correct CORS header.

    var context = HttpContext.Current;
	var corsProvider = new ConfigurationCorsAllowedOriginsProvider();
	Cors.AllowCrossOriginRequest(context.Request, context.Response, corsProvider.CorsAllowedOrigins());

`ConfigurationCorsAllowedOriginsProvider` implements `ICorsAllowedOriginsProvider`, allowing you to swap it for another implementation that reads the allowed origins from somewhere else.
