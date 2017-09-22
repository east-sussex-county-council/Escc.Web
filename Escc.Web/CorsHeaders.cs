using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Security;
using System.Text.RegularExpressions;
using System.Web.Cors;

namespace Escc.Web
{
    /// <summary>
    /// Cross-origin resource sharing (CORS) allows JavaScript on a web page to make XMLHttpRequests to another domain, not the domain the JavaScript originated from
    /// </summary>
    public class CorsHeaders : ICorsHeaders
    {
        private readonly NameValueCollection _requestHeadersCollection;
        private readonly NameValueCollection _responseHeadersCollection;
        private readonly HttpHeaders _requestHeadersObject;
        private readonly HttpHeaders _responseHeadersObject;
        private readonly CorsPolicy _corsPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsHeaders" /> class.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <param name="corsPolicy">The CORS policy.</param>
        /// <exception cref="System.ArgumentNullException">
        /// requestHeadersCollection
        /// or
        /// responseHeadersCollection
        /// or
        /// corsPolicy
        /// </exception>
        [SecuritySafeCritical]
        public CorsHeaders(NameValueCollection requestHeaders, NameValueCollection responseHeaders, CorsPolicy corsPolicy)
        {
            if (requestHeaders == null) throw new ArgumentNullException("requestHeaders");
            if (responseHeaders == null) throw new ArgumentNullException("responseHeaders");
            if (corsPolicy == null) throw new ArgumentNullException("corsPolicy");            
            _requestHeadersCollection = requestHeaders;
            _responseHeadersCollection = responseHeaders;
            _corsPolicy = corsPolicy;
       }

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsHeaders" /> class.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <param name="corsPolicy">The CORS policy.</param>
        /// <exception cref="System.ArgumentNullException">
        /// requestHeadersCollection
        /// or
        /// responseHeadersCollection
        /// or
        /// corsPolicy
        /// </exception>
        [SecuritySafeCritical]
        public CorsHeaders(HttpRequestHeaders requestHeaders, HttpResponseHeaders responseHeaders, CorsPolicy corsPolicy)
        {
            if (requestHeaders == null) throw new ArgumentNullException("requestHeaders");
            if (responseHeaders == null) throw new ArgumentNullException("responseHeaders");
            if (corsPolicy == null) throw new ArgumentNullException("corsPolicy");
            _requestHeadersObject = requestHeaders;
            _responseHeadersObject = responseHeaders;
            _corsPolicy = corsPolicy;
        }

        /// <summary>
        /// A cross-origin request will only work if the CORS headers are right, and we only want to allow specific origins
        /// </summary>
        [SecuritySafeCritical]
        public void UpdateHeaders()
        {
            if (_responseHeadersCollection != null)
            {
                UpdateHeaders(_requestHeadersCollection, _responseHeadersCollection, _corsPolicy);
            }
            else if (_responseHeadersObject != null)
            {
                UpdateHeaders(_requestHeadersObject, _responseHeadersObject, _corsPolicy);
            }
        }

        [SecuritySafeCritical]
        private static void UpdateHeaders(NameValueCollection requestHeaders, NameValueCollection responseHeaders, CorsPolicy corsPolicy)
        {
            // Reset any existing headers
            responseHeaders.Remove("Access-Control-Allow-Origin");

            // Not a CORS request - do nothing
            var requestOrigin = requestHeaders["Origin"];
            if (String.IsNullOrEmpty(requestOrigin)) return;

            // Is the origin in the list of allowed origins?
            var allowedOrigin = IsAllowedOrigin(corsPolicy.Origins, requestOrigin);

            // If it is, echo back the origin as a CORS header
            if (allowedOrigin)
            {
                responseHeaders.Add("Access-Control-Allow-Origin", requestOrigin);
            }
        }

        private static bool IsAllowedOrigin(IList<string> allowedOrigins, string requestOrigin)
        {
            requestOrigin = requestOrigin.ToLowerInvariant();
            var allowedOrigin = allowedOrigins.Contains(requestOrigin);
            if (!allowedOrigin)
            {
                // Allow a wildcard for the port number
                var match = Regex.Match(requestOrigin, ":[0-9]+$");
                if (match.Success)
                {
                    requestOrigin = requestOrigin.Substring(0, requestOrigin.Length - match.Length) + ":*";
                    allowedOrigin = allowedOrigins.Contains(requestOrigin);
                }
            }
            return allowedOrigin;
        }

        [SecuritySafeCritical]
        private static void UpdateHeaders(HttpHeaders requestHeaders, HttpHeaders responseHeaders, CorsPolicy corsPolicy)
        {
            // Reset any existing headers
            responseHeaders.Remove("Access-Control-Allow-Origin");

            // Not a CORS request - do nothing
            var requestOrigin = String.Empty;
            IEnumerable<string> requestOrigins;
            if (requestHeaders.TryGetValues("Origin", out requestOrigins))
            {
                requestOrigin = requestOrigins.FirstOrDefault();
            }
            if (String.IsNullOrEmpty(requestOrigin)) return;

            // Is the origin in the list of allowed origins?
            var allowedOrigin = IsAllowedOrigin(corsPolicy.Origins, requestOrigin);

            // If it is, echo back the origin as a CORS header
            if (allowedOrigin)
            {
                responseHeaders.Add("Access-Control-Allow-Origin", requestOrigin);
            }
        }
    }
}