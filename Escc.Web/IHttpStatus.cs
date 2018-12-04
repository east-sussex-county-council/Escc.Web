using System;
using System.Web;

namespace Escc.Web
{
      /// <summary>
    /// Configure common HTTP response statuses in a way which complies with HTTP 1.1 in RFC2116
    /// </summary>
    public interface IHttpStatus
      {
          /// <summary>
          /// Sets the current response status to '301 Moved Permanently' and redirects to the specified URL
          /// </summary>
          /// <param name="replacedByUrl">The replacement URL.</param>
          /// <remarks>See <seealso cref="MovedPermanently( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
          void MovedPermanently(string replacedByUrl);

          /// <summary>
          /// Sets the current response status to '301 Moved Permanently' and redirects to the specified URL
          /// </summary>
          /// <param name="replacedByUrl">The replacement URL.</param>
          /// <remarks>See <seealso cref="MovedPermanently( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
          void MovedPermanently(Uri replacedByUrl);

          /// <summary>
          /// Sets the supplied response status to '301 Moved Permanently' and redirects to the specified URL
          /// </summary>
          /// <param name="requestUrl">The request URL.</param>
          /// <param name="requestMethod">The request method.</param>
          /// <param name="replacedByUrl">The replacement URL.</param>
          /// <param name="response">The response.</param>
          /// <exception cref="System.ArgumentNullException">
          /// replacedByUrl
          /// or
          /// request
          /// or
          /// response
          /// </exception>
          /// <exception cref="System.ArgumentException">The request and destination URIs are the same;replacedByUrl</exception>
          /// <remarks>
          /// <para>Implements RFC2616:</para>
          /// <para>
          ///   <c>The requested resource has been assigned a new permanent URI and any future references to this resource
          /// SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references
          /// to the Request-URI to one or more of the new references returned by the server, where possible. This response is
          /// cacheable unless indicated otherwise.</c>
          /// </para>
          /// <para>
          ///   <c>The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD,
          /// the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
          /// </para>
          /// </remarks>
          void MovedPermanently(Uri requestUrl, string requestMethod, Uri replacedByUrl, HttpResponse response);

          /// <summary>
          /// Sets the supplied response status to '301 Moved Permanently' and redirects to the specified URL
          /// </summary>
          /// <param name="requestUrl">The request URL.</param>
          /// <param name="requestMethod">The request method.</param>
          /// <param name="replacedByUrl">The replacement URL.</param>
          /// <param name="response">The response.</param>
          /// <exception cref="System.ArgumentNullException">
          /// replacedByUrl
          /// or
          /// request
          /// or
          /// response
          /// </exception>
          /// <exception cref="System.ArgumentException">The request and destination URIs are the same;replacedByUrl</exception>
          /// <remarks>
          /// <para>Implements RFC2616:</para>
          /// <para>
          ///   <c>The requested resource has been assigned a new permanent URI and any future references to this resource
          /// SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references
          /// to the Request-URI to one or more of the new references returned by the server, where possible. This response is
          /// cacheable unless indicated otherwise.</c>
          /// </para>
          /// <para>
          ///   <c>The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD,
          /// the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
          /// </para>
          /// </remarks>
          void MovedPermanently(Uri requestUrl, string requestMethod, Uri replacedByUrl, HttpResponseBase response);

          /// <summary>
          /// Sets the current response status to '303 See Other' and redirects to the specified URL
          /// </summary>
          /// <param name="destinationUrl">The destination URL.</param>
          /// <remarks>See <seealso cref="SeeOther( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
          void SeeOther(string destinationUrl);

          /// <summary>
          /// Sets the current response status to '303 See Other' and redirects to the specified URL
          /// </summary>
          /// <param name="destinationUrl">The destination URL.</param>
          /// <remarks>See <seealso cref="SeeOther( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
          void SeeOther(Uri destinationUrl);

          /// <summary>
          /// Sets the supplied response status to '303 See Other' and redirects to the specified URL
          /// </summary>
          /// <param name="requestUrl">The request URL.</param>
          /// <param name="requestMethod">The request method.</param>
          /// <param name="destinationUrl">The destination URL.</param>
          /// <param name="response">The response.</param>
          /// <exception cref="System.ArgumentNullException">
          /// destinationUrl
          /// or
          /// request
          /// or
          /// response
          /// </exception>
          /// <exception cref="System.ArgumentException">The request and destination URIs are the same;destinationUrl</exception>
          /// <remarks>
          /// <para>Implements RFC2616:</para>
          /// <para>
          ///   <c>The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource.
          /// This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource.
          /// The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the
          /// response to the second (redirected) request might be cacheable.</c>
          /// </para>
          /// <para>
          ///   <c>The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of
          /// the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
          /// </para>
          /// </remarks>
          void SeeOther(Uri requestUrl, string requestMethod, Uri destinationUrl, HttpResponse response);

          /// <summary>
          /// Sets the supplied response status to '303 See Other' and redirects to the specified URL
          /// </summary>
          /// <param name="requestUrl">The request URL.</param>
          /// <param name="requestMethod">The request method.</param>
          /// <param name="destinationUrl">The destination URL.</param>
          /// <param name="response">The response.</param>
          /// <exception cref="System.ArgumentNullException">
          /// destinationUrl
          /// or
          /// request
          /// or
          /// response
          /// </exception>
          /// <exception cref="System.ArgumentException">The request and destination URIs are the same;destinationUrl</exception>
          /// <remarks>
          /// <para>Implements RFC2616:</para>
          /// <para>
          ///   <c>The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource.
          /// This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource.
          /// The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the
          /// response to the second (redirected) request might be cacheable.</c>
          /// </para>
          /// <para>
          ///   <c>The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of
          /// the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
          /// </para>
          /// </remarks>
          void SeeOther(Uri requestUrl, string requestMethod, Uri destinationUrl, HttpResponseBase response);

          /// <summary>
          /// Sets the current response status to '410 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
          /// </summary>
          /// <remarks>See <seealso cref="Gone(HttpResponse)"/> for more details.</remarks>
          void Gone();

          /// <summary>
          /// Sets the supplied response status to '410 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
          /// </summary>
          /// <param name="response">The response.</param>
          /// <remarks>
          /// 	<para>Implements RFC2616:</para>
          /// 	<para><c>The requested resource is no longer available at the server and no forwarding address is known. This condition is expected to be considered permanent. 
          /// 	Clients with link editing capabilities SHOULD delete references to the Request-URI after user approval. If the server does not know, or has no facility to determine, 
          /// 	whether or not the condition is permanent, the status code 404 (Not Found) SHOULD be used instead. This response is cacheable unless indicated otherwise.</c></para>
          /// </remarks>
          void Gone(HttpResponse response);

          /// <summary>
          /// Sets the supplied response status to '410 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
          /// </summary>
          /// <param name="response">The response.</param>
          /// <remarks>
          /// 	<para>Implements RFC2616:</para>
          /// 	<para><c>The requested resource is no longer available at the server and no forwarding address is known. This condition is expected to be considered permanent. 
          /// 	Clients with link editing capabilities SHOULD delete references to the Request-URI after user approval. If the server does not know, or has no facility to determine, 
          /// 	whether or not the condition is permanent, the status code 404 (Not Found) SHOULD be used instead. This response is cacheable unless indicated otherwise.</c></para>
          /// </remarks>
          void Gone(HttpResponseBase response);

          /// <summary>
          /// Sets the current response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
          /// </summary>
          /// <remarks>See <seealso cref="BadRequest(HttpResponse)"/> for more details.</remarks>
          void BadRequest();

          /// <summary>
          /// Sets the supplied response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
          /// </summary>
          /// <param name="response">The response.</param>
          /// <remarks>
          /// 	<para>Implements RFC2616:</para>
          /// 	<para><c>The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications.</c></para>
          /// </remarks>
          void BadRequest(HttpResponse response);

          /// <summary>
          /// Sets the supplied response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
          /// </summary>
          /// <param name="response">The response.</param>
          /// <remarks>
          /// 	<para>Implements RFC2616:</para>
          /// 	<para><c>The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications.</c></para>
          /// </remarks>
          void BadRequest(HttpResponseBase response);

          /// <summary>
          /// Sets the current response status to '404 Not Found' when the page is not found, or hidden for some reason.
          /// </summary>
          /// <remarks>See <seealso cref="NotFound(HttpResponse)"/> for more details.</remarks>
          void NotFound();

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
          void NotFound(HttpResponse response);

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
          void NotFound(HttpResponseBase response);

          /// <summary>
          /// Sets the current response status to '500 Internal Server Error' indicating an unexpected error.
          /// </summary>
          /// <remarks>See <seealso cref="BadRequest(HttpResponse)"/> for more details.</remarks>
          void InternalServerError();

          /// <summary>
          /// Sets the supplied response status to '500 Internal Server Error' indicating an unexpected error.
          /// </summary>
          /// <param name="response">The response.</param>
          /// <remarks>
          /// 	<para>Implements RFC2616:</para>
          /// 	<para><c>The server encountered an unexpected condition which prevented it from fulfilling the request.</c></para>
          /// </remarks>
          void InternalServerError(HttpResponse response);

          /// <summary>
          /// Sets the supplied response status to '500 Internal Server Error' indicating an unexpected error.
          /// </summary>
          /// <param name="response">The response.</param>
          /// <remarks>
          /// 	<para>Implements RFC2616:</para>
          /// 	<para><c>The server encountered an unexpected condition which prevented it from fulfilling the request.</c></para>
          /// </remarks>
          void InternalServerError(HttpResponseBase response);

          /// <summary>
          /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
          /// </summary>
          /// <remarks>
          ///     <para>Implements RFC2616:</para>
          ///     <para><c>The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request.</c></para>
          /// </remarks>
          /// <param name="response">The response.</param>
          void BadGateway(HttpResponse response);

          /// <summary>
          /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
          /// </summary>
          /// <remarks>
          ///     <para>Implements RFC2616:</para>
          ///     <para><c>The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request.</c></para>
          /// </remarks>
          /// <param name="response">The response.</param>
          void BadGateway(HttpResponseBase response);

          /// <summary>
          /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
          /// </summary>
          /// <remarks>See <seealso cref="BadGateway(HttpResponse)"/> for more details.</remarks>
          void BadGateway();
      }
}