using System;
using System.Collections.Generic;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Cross-origin resource sharing (CORS) allows JavaScript on a web page to make XMLHttpRequests to another domain, not the domain the JavaScript originated from
    /// </summary>
    [Obsolete("For .NET 4.5.2 and above, use the Escc.Web NuGet package")]
    public static class Cors
    {
        /// <summary>
        /// A cross-origin request will only work if the CORS headers are right, and we only want to allow specific origins
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="allowedOrigins"></param>
        public static void AllowCrossOriginRequest(HttpRequest request, HttpResponse response, IEnumerable<string> allowedOrigins)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            // Not a CORS request - do nothing
            var requestOrigin = request.Headers["Origin"];
            if (String.IsNullOrEmpty(requestOrigin)) return;

            // Is the origin in the list of allowed origins?
            var allowedOrigin = new List<string>(allowedOrigins).Contains(requestOrigin.ToLowerInvariant());

            // If it is, echo back the origin as a CORS header
            if (allowedOrigin)
            {
                response.AddHeader("Access-Control-Allow-Origin", requestOrigin);
            }
        }
    }
}