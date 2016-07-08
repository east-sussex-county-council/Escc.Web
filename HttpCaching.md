# HTTP caching

[HTTP caching](https://www.keycdn.com/blog/http-cache-headers/) improves performance and reduces the load on your servers by telling clients when they can re-use pages and assets that they already have. Every web application should set its approach to HTTP caching, even if the right approach is to turn it off.

## HTTP caching ASP.NET WebForms and MVC

The `HttpCacheHeaders` class is a convenient way to set the right HTTP headers to cache responses in ASP.NET WebForms or MVC. 

For example, to cache a page for one day which is identical for every user:

	new HttpCacheHeaders().CacheUntil(Response.Cache, DateTime.Now.AddDays(1), true);

## HTTP caching ASP.NET Web API

For ASP.NET Web API there are good NuGet packages available, such as [CacheCow](https://github.com/aliostad/CacheCow/wiki) and [Web API CacheOutput](https://github.com/filipw/AspNetWebApi-OutputCache). 

## HTTP caching static files  

`HttpCacheHeaders` doesn't manage ETags to prevent unnecessary downloads of unchanged resources, so it's better to [configure HTTP caching of static files using IIS](https://www.iis.net/configreference/system.webserver/staticcontent/clientcache).
