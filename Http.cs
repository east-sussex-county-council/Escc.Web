﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Methods for working with elements of the HTTP web protocol
    /// </summary>
    public static class Http
    {
        #region Status codes

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
            var absoluteDestination = replacedByUrl.IsAbsoluteUri ? replacedByUrl : Iri.MakeAbsolute(replacedByUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == request.Url.ToString()) throw new ArgumentException("The request and destination URIs are the same", "replacedByUrl");

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
            var absoluteDestination = destinationUrl.IsAbsoluteUri ? destinationUrl : Iri.MakeAbsolute(destinationUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == request.Url.ToString()) throw new ArgumentException("The request and destination URIs are the same", "destinationUrl");

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

        #endregion

        #region Work with MIME types

        /// <summary>
        /// Returns the best MIME type for a response, based on the user agent's preferences expressed in the HTTP Accept header, and the MIME types the page is able to serve
        /// </summary>
        /// <param name="acceptTypesHeader">The HTTP Accept header, accessible using the <c>AcceptTypes</c> property of the current <seealso cref="System.Web.HttpRequest"/> (for example <c>System.Web.HttpContext.Current.Request.AcceptTypes</c>).</param>
        /// <param name="supportedMimeTypes">The MIME types the page is able to serve, in the order of our preference in case the user agent doesn't mind.</param>
        /// <param name="requireExplicitSupportFor">A subset of MIME types from the <c>mimeTypesAvailable</c> list which should only be used if the user-agent explicitly states support, not just using a wildcard.</param>
        /// <returns>Best MIME type to serve</returns>
        /// <exception cref="ArgumentNullException">Thrown if no available MIME types are specified</exception>
        /// <exception cref="ArgumentException">Thrown if only one available MIME types is specified (there's no point deciding if there's only one option)</exception>
        /// <remarks>The combination of <c>supportedMimeTypes</c> and <c>requireExplicitSupportFor</c> allows us to cope with user agents that say they'll accept anything,
        /// but are really better off with a fallback option rather than a newer standard they can't handle. For example, we might rather serve XHTML to Firefox because 
        /// it says explicitly that it supports XHTML, but we'd rather send HTML to IE8 because, even though it says it accepts anything, we know it doesn't support XHTML properly.</remarks>
        public static string PreferredMimeType(string[] acceptTypesHeader, string[] supportedMimeTypes, string[] requireExplicitSupportFor)
        {
            if (supportedMimeTypes == null) throw new ArgumentNullException("supportedMimeTypes", "You must specify at least two MIME types which can be served");
            if (supportedMimeTypes.Length < 2) throw new ArgumentException("You must specify at least two MIME types which can be served", "supportedMimeTypes");

            // Put the MIME types that we can offer into a dictionary, so we can track the user agent's preferences for those types
            Dictionary<string, decimal> candidateMimeTypes = new Dictionary<string, decimal>();
            foreach (string mimeType in supportedMimeTypes) candidateMimeTypes.Add(mimeType, 0);

            // And put the explicit-only types into a List so we have access to the Contains method
            List<string> explicitSupportOnly = new List<string>(requireExplicitSupportFor);

            // Check the accept header to work out the user agent's preferences for our available MIME types.
            // Ignore anything they want that we can't provide.
            if (acceptTypesHeader != null)
            {
                CultureInfo americanCulture = new CultureInfo("en-US"); // since HTTP is probably based on that

                foreach (string acceptedMimeType in acceptTypesHeader)
                {
                    if (String.IsNullOrEmpty(acceptedMimeType)) continue;

                    foreach (string supportedMimeType in supportedMimeTypes)
                    {
                        // Deal first with the simplest syntax, which is just the mime type
                        if (acceptedMimeType == supportedMimeType)
                        {
                            candidateMimeTypes[supportedMimeType] = 1;
                            continue;
                        }

                        // If not exact match, but it starts with a match and semi-colon, it has params
                        if (acceptedMimeType.StartsWith(supportedMimeType + ";", false, americanCulture))
                        {
                            string[] segments = acceptedMimeType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            if (segments[0] == supportedMimeType)
                            {
                                candidateMimeTypes[supportedMimeType] = 1;
                                for (int i = 1; i < segments.Length; i++)
                                {
                                    if (segments[i].Trim().StartsWith("q=", false, americanCulture)) candidateMimeTypes[supportedMimeType] = Decimal.Parse(segments[i].Trim().Substring(2), americanCulture);
                                }
                            }
                        }

                        // TODO: This won't cope with user agents saying wildcard types are acceptable. Should test for those here.
                    }
                }
            }

            // Look at the MIME types we want to serve. If all's equal choose our first preference. But if user prefers one of our backup options, choose that.
            // Note that this can lead to recommending a format the user can't handle, if they can't handle any of the formats we provide.

            // Start by setting preference to values that should definitely be beaten
            string preferredMimeType = String.Empty;
            decimal preferenceToBeat = -1;

            foreach (string mimeType in candidateMimeTypes.Keys)
            {
                if (candidateMimeTypes[mimeType] > preferenceToBeat)
                {
                    // preference of 0 indicates that the user agent didn't explicitly state support, 
                    // so if it's 0 check that we don't require explicit support for that MIME type
                    if (candidateMimeTypes[mimeType] > 0 || !explicitSupportOnly.Contains(mimeType))
                    {
                        preferredMimeType = mimeType;
                        preferenceToBeat = candidateMimeTypes[mimeType];
                    }
                }
            }

            return preferredMimeType;
        }

        #endregion // Work with MIME types

        #region HTTP caching

        /// <summary>
        /// Sets the current response to be cached for a given amount of time following the initial request, allowing both shared and private caches
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <param name="minute">The minute.</param>
        public static void CacheFor(int hours, int minute)
        {
            CacheFor(hours, minute, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets a response to be cached for a given amount of time following the initial request, allowing both shared and private caches
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="response">The response.</param>
        public static void CacheFor(int hours, int minute, HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            var freshUntil = DateTime.Now.AddHours(hours).AddMinutes(minute);
            response.Cache.SetExpires(freshUntil);
            response.Cache.SetMaxAge(freshUntil.Subtract(DateTime.Now));

            // Allow caching on shared proxies as well as users' own browsers
            response.Cache.SetCacheability(HttpCacheability.Public);
        }

        /// <summary>
        /// Sets the current response to be cached for up to one day until the specified time, allowing both shared and private caches
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        public static void CacheDaily(int hour, int minute)
        {
            CacheDaily(hour, minute, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets a response to be cached for up to one day until the specified time, allowing both shared and private caches
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="response">The response.</param>
        public static void CacheDaily(int hour, int minute, HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            // Are we before or after today's expiry time?
            var expiryToday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute, 0);
            if (DateTime.Now <= expiryToday)
            {
                response.Cache.SetExpires(expiryToday);
                response.Cache.SetMaxAge(expiryToday.Subtract(DateTime.Now));
            }
            else
            {
                var tomorrow = DateTime.Today.AddDays(1);
                var expiryTomorrow = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, hour, minute, 0);
                response.Cache.SetExpires(expiryTomorrow);
                response.Cache.SetMaxAge(expiryTomorrow.Subtract(DateTime.Now));
            }

            // Allow caching on shared proxies as well as users' own browsers
            response.Cache.SetCacheability(HttpCacheability.Public);
        }

        #endregion

    }
}