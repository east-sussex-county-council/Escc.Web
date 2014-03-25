using System;
using System.Collections.Generic;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Cross-origin resource sharing (CORS) allows JavaScript on a web page to make XMLHttpRequests to another domain, not the domain the JavaScript originated from
    /// </summary>
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

            // Not a cross-origin request - do nothing
            // (You could trigger this with a crafted request eg Origin: http://some-other-origin/?endswith=http://currenthost, but
            //  it doesn't result in any elevation of privilege - a cross-origin request is still denied - so that's fine.
            //  This is just about not having to remember to include the current host in the allowed hosts list.)
            if (requestOrigin.EndsWith("://" + request.Url.Host)) return;

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