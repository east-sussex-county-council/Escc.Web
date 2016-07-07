using System;
using System.Security.Cryptography;
using System.Threading;
using System.Web;

namespace Escc.Web
{
    /// <summary>
    /// Methods for working with elements of the HTTP web protocol
    /// </summary>
    public static class Http
    {
        /// <summary>
        /// Sets the current response status to '301 Moved Permanently' and redirects to the specified URL
        /// </summary>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <remarks>See <seealso cref="Status301MovedPermanently(Uri, HttpRequest, HttpResponse)"/> for more details.</remarks>
        public static void Status301MovedPermanently(string replacedByUrl)
        {
            Status301MovedPermanently(new Uri(replacedByUrl, UriKind.RelativeOrAbsolute), HttpContext.Current.Request, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the current response status to '301 Moved Permanently' and redirects to the specified URL
        /// </summary>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <remarks>See <seealso cref="Status301MovedPermanently(Uri, HttpRequest, HttpResponse)"/> for more details.</remarks>
        public static void Status301MovedPermanently(Uri replacedByUrl)
        {
            Status301MovedPermanently(replacedByUrl, HttpContext.Current.Request, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '301 Moved Permanently' and redirects to the specified URL
        /// </summary>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The requested resource has been assigned a new permanent URI and any future references to this resource 
        /// 	SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references 
        /// 	to the Request-URI to one or more of the new references returned by the server, where possible. This response is 
        /// 	cacheable unless indicated otherwise.</c></para>
        /// 	<para><c>The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, 
        /// 	the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c></para>
        /// </remarks>
        public static void Status301MovedPermanently(Uri replacedByUrl, HttpRequest request, HttpResponse response)
        {
            if (replacedByUrl == null) throw new ArgumentNullException("replacedByUrl");
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            // RFC 2616 says the destination URL for the Location header must be absolute
            var absoluteDestination = replacedByUrl.IsAbsoluteUri ? replacedByUrl : new Uri(HttpContext.Current.Request.Url, replacedByUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == request.Url.ToString()) throw new ArgumentException("The request and destination URIs are the same", "replacedByUrl");

            try
            {
                response.Status = "301 Moved Permanently";
                response.StatusCode = 301;
                response.AddHeader("Location", absoluteDestination.ToString());

                if (request.HttpMethod != "HEAD")
                {
                    // Use a minimal HTML5 document for the hypertext response, since few people will ever see it.
                    response.Write("<!DOCTYPE html><title>This page has moved</title><h1>This page has moved</h1><p>Please see <a href=\"" + absoluteDestination.ToString() + "\">" + absoluteDestination.ToString() + "</a> instead.</p>");
                }

                response.End();
            }
            catch (ThreadAbortException)
            {
                // Just catch the expected exception. Don't call Thread.ResetAbort() because, 
                // in an Umbraco context, it causes problems with updating the cache.
            }
        }


        /// <summary>
        /// Sets the current response status to '303 See Other' and redirects to the specified URL
        /// </summary>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <remarks>See <seealso cref="Status303SeeOther(Uri, HttpRequest, HttpResponse)"/> for more details.</remarks>
        public static void Status303SeeOther(string destinationUrl)
        {
            Status303SeeOther(new Uri(destinationUrl, UriKind.RelativeOrAbsolute), HttpContext.Current.Request, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the current response status to '303 See Other' and redirects to the specified URL
        /// </summary>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <remarks>See <seealso cref="Status303SeeOther(Uri, HttpRequest, HttpResponse)"/> for more details.</remarks>
        public static void Status303SeeOther(Uri destinationUrl)
        {
            Status303SeeOther(destinationUrl, HttpContext.Current.Request, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '303 See Other' and redirects to the specified URL
        /// </summary>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource.
        /// This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource.
        /// The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the
        /// response to the second (redirected) request might be cacheable.</c></para>
        /// 	<para><c>The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of
        /// the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c></para>
        /// </remarks>
        public static void Status303SeeOther(Uri destinationUrl, HttpRequest request, HttpResponse response)
        {
            if (destinationUrl == null) throw new ArgumentNullException("destinationUrl");
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            // RFC 2616 says the destination URL for the Location header must be absolute
            var absoluteDestination = destinationUrl.IsAbsoluteUri ? destinationUrl : new Uri(HttpContext.Current.Request.Url, destinationUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == request.Url.ToString())
            {
                throw new ArgumentException("The request and destination URIs are the same", "destinationUrl");
            }

            try
            {
                response.Status = "303 See Other";
                response.StatusCode = 303;
                response.AddHeader("Location", absoluteDestination.ToString());

                if (request.HttpMethod != "HEAD")
                {
                    // Use a minimal HTML5 document for the hypertext response, since few people will ever see it.
                    response.Write("<!DOCTYPE html><title>See another page</title><h1>See another page</h1><p>Please see <a href=\"" + absoluteDestination.ToString() + "\">" + absoluteDestination.ToString() + "</a> instead.</p>");
                }

                response.End();
            }
            catch (ThreadAbortException)
            {
                // Just catch the expected exception. Don't call Thread.ResetAbort() because, 
                // in an Umbraco context, it causes problems with updating the cache.
            }
        }

        /// <summary>
        /// Sets the current response status to '310 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
        /// </summary>
        /// <remarks>See <seealso cref="Status310Gone(HttpResponse)"/> for more details.</remarks>
        public static void Status310Gone()
        {
            Status310Gone(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '310 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The requested resource is no longer available at the server and no forwarding address is known. This condition is expected to be considered permanent. 
        /// 	Clients with link editing capabilities SHOULD delete references to the Request-URI after user approval. If the server does not know, or has no facility to determine, 
        /// 	whether or not the condition is permanent, the status code 404 (Not Found) SHOULD be used instead. This response is cacheable unless indicated otherwise.</c></para>
        /// </remarks>
        public static void Status310Gone(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "310 Gone";
            response.StatusCode = 310;
        }

        /// <summary>
        /// Sets the current response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
        /// </summary>
        /// <remarks>See <seealso cref="Status400BadRequest(HttpResponse)"/> for more details.</remarks>
        public static void Status400BadRequest()
        {
            Status400BadRequest(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications.</c></para>
        /// </remarks>
        public static void Status400BadRequest(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "400 Bad Request";
            response.StatusCode = 400;
        }

        /// <summary>
        /// Sets the current response status to '404 Not Found' when the page is not found, or hidden for some reason.
        /// </summary>
        /// <remarks>See <seealso cref="Status404NotFound(HttpResponse)"/> for more details.</remarks>
        public static void Status404NotFound()
        {
            Status404NotFound(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '404 Not Found' when the page is not found, or hidden for some reason.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The server has not found anything matching the Request-URI. No indication is given of whether the condition is temporary or permanent.
        /// 	The 410 (Gone) status code SHOULD be used if the server knows, through some internally configurable mechanism, that an old resource is permanently 
        /// 	unavailable and has no forwarding address. This status code is commonly used when the server does not wish to reveal exactly why the request has 
        /// 	been refused, or when no other response is applicable.</c></para>
        /// </remarks>
        public static void Status404NotFound(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "404 Not Found";
            response.StatusCode = 404;
        }

        /// <summary>
        /// Sets the current response status to '500 Internal Server Error' indicating an unexpected error.
        /// </summary>
        /// <remarks>See <seealso cref="Status400BadRequest(HttpResponse)"/> for more details.</remarks>
        public static void Status500InternalServerError()
        {
            Status500InternalServerError(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '500 Internal Server Error' indicating an unexpected error.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The server encountered an unexpected condition which prevented it from fulfilling the request.</c></para>
        /// </remarks>
        public static void Status500InternalServerError(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "500 Internal Server Error";
            response.StatusCode = 500;

            RandomDelay();
        }

        /// <summary>
        /// Introduce a random delay, so defend against anyone trying to detect specific errors based on the time taken.
        /// <remarks>
        /// Code from <a href="http://weblogs.asp.net/scottgu/archive/2010/09/18/important-asp-net-security-vulnerability.aspx">http://weblogs.asp.net/scottgu/archive/2010/09/18/important-asp-net-security-vulnerability.aspx</a>
        /// </remarks>
        /// </summary>
        private static void RandomDelay()
        {
            byte[] delay = new byte[1];
            RandomNumberGenerator prng = new RNGCryptoServiceProvider();

            prng.GetBytes(delay);
            Thread.Sleep((int)delay[0]);

            IDisposable disposable = prng as IDisposable;
            if (disposable != null) { disposable.Dispose(); }
        }


        /// <summary>
        /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
        /// </summary>
        /// <remarks>
        ///     <para>Implements RFC2616:</para>
        ///     <para><c>The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request.</c></para>
        /// </remarks>
        /// <param name="response">The response.</param>
        public static void Status502BadGateway(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "502 Bad Gateway";
            response.StatusCode = 502;

            RandomDelay();
        }

        /// <summary>
        /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
        /// </summary>
        /// <remarks>See <seealso cref="Status502BadGateway(HttpResponse)"/> for more details.</remarks>
        public static void Status502BadGateway()
        {
            Status502BadGateway(HttpContext.Current.Response);
        }
    }
}
