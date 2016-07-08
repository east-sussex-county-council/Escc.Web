# Returning common HTTP redirect and error responses

It's important to return the correct HTTP status code from any web request, because it informs user agents what to do with the request and whether they should repeat it. The `HttpStatus` class is a convenient way to set the correct properties of some common responses in a way which complies with HTTP 1.1 in [RFC2116](https://tools.ietf.org/html/rfc2616), handling issues like HEAD requests, redirect loops and whether to return a response body.

Each response requires either the `HttpResponse` used by ASP.NET WebForms or the `HttpResponseBase` used by ASP.NET.MVC, which are both available as the `Response` property in a code-behind, controller or view. 

	new HttpStatus().SeeOther(Request.Url, Request.HttpMethod, new Uri("https://www.example.org"), Response);

…or if you're happy to rely on `HttpContext.Current`:

	new HttpStatus().SeeOther(new Uri("https://www.example.org"));

## ASP.NET Web API

In ASP.NET Web API you can [return an `IHttpActionResult`](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/action-results), so you don't need `HttpStatus`. 