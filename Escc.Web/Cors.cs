using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using System.Web.Cors;

namespace Escc.Web
{
    /// <summary>
    /// Cross-origin resource sharing (CORS) allows JavaScript on a web page to make XMLHttpRequests to another domain, not the domain the JavaScript originated from
    /// </summary>
    public class Cors
    {
        /// <summary>
        /// A cross-origin request will only work if the CORS headers are right, and we only want to allow specific origins
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <param name="corsPolicy">The CORS policy.</param>
        /// <exception cref="System.ArgumentNullException">
        /// requestHeaders
        /// or
        /// responseHeaders
        /// or
        /// corsPolicy
        /// </exception>
        [SecuritySafeCritical]
        public void ApplyPolicy(NameValueCollection requestHeaders, NameValueCollection responseHeaders, CorsPolicy corsPolicy)
        {
            if (requestHeaders == null) throw new ArgumentNullException("requestHeaders");
            if (responseHeaders == null) throw new ArgumentNullException("responseHeaders");
            if (corsPolicy == null) throw new ArgumentNullException("corsPolicy");

            // Not a CORS request - do nothing
            var requestOrigin = requestHeaders["Origin"];
            if (String.IsNullOrEmpty(requestOrigin)) return;

            // Is the origin in the list of allowed origins?
            var allowedOrigin = corsPolicy.Origins.Contains(requestOrigin.ToLowerInvariant());

            // If it is, echo back the origin as a CORS header
            if (allowedOrigin)
            {
                responseHeaders.Add("Access-Control-Allow-Origin", requestOrigin);
            }
        }
    }
}