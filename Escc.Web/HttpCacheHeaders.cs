using System;
using System.Web;

namespace Escc.Web
{
    /// <summary>
    /// Sets the common headers used to control HTTP caching in ASP.NET WebForms or ASP.NET MVC, including older versions for backwards-compatibility
    /// </summary>
    public class HttpCacheHeaders : IHttpCacheHeaders
    {
        /// <summary>
        /// Sets a response to be cached for a given amount of time following the initial request, allowing both shared and private caches
        /// </summary>
        /// <param name="cachePolicy">The response.</param>
        /// <param name="currentTime">The current time.</param>
        /// <param name="cacheExpiryTime">The cache expiry time.</param>
        /// <param name="responseIsIdenticalForEveryUser">if set to <c>true</c> one user may receive a response that was prepared for another user.</param>
        /// <exception cref="System.ArgumentNullException">cachePolicy</exception>
        /// <remarks>This method is for use with ASP.NET WebForms</remarks>
        public void CacheUntil(HttpCachePolicy cachePolicy, DateTime currentTime, DateTime cacheExpiryTime, bool responseIsIdenticalForEveryUser = true)
        {
            if (cachePolicy == null) throw new ArgumentNullException("cachePolicy");

            // Max-Age is the current standard, and Expires is the older one
            cachePolicy.SetMaxAge(cacheExpiryTime.Subtract(currentTime));
            cachePolicy.SetExpires(cacheExpiryTime);

            // Public allows caching on shared proxies as well as users' own browsers
            cachePolicy.SetCacheability(responseIsIdenticalForEveryUser ? HttpCacheability.Public : HttpCacheability.Private);
        }

        /// <summary>
        /// Sets a response to be cached for a given amount of time following the initial request, allowing both shared and private caches
        /// </summary>
        /// <param name="cachePolicy">The response.</param>
        /// <param name="currentTime">The current time.</param>
        /// <param name="cacheExpiryTime">The cache expiry time.</param>
        /// <param name="responseIsIdenticalForEveryUser">if set to <c>true</c> one user may receive a response that was prepared for another user.</param>
        /// <exception cref="System.ArgumentNullException">cachePolicy</exception>
        /// <remarks>This method is for use with ASP.NET MVC</remarks>
        public void CacheUntil(HttpCachePolicyBase cachePolicy, DateTime currentTime, DateTime cacheExpiryTime, bool responseIsIdenticalForEveryUser = true)
        {
            if (cachePolicy == null) throw new ArgumentNullException("cachePolicy");

            // Max-Age is the current standard, and Expires is the older one
            cachePolicy.SetMaxAge(cacheExpiryTime.Subtract(currentTime));
            cachePolicy.SetExpires(cacheExpiryTime);

            // Public allows caching on shared proxies as well as users' own browsers
            cachePolicy.SetCacheability(responseIsIdenticalForEveryUser ? HttpCacheability.Public : HttpCacheability.Private);
        }

        /// <summary>
        /// Sets a response to be cached for a given amount of time following the initial request, allowing both shared and private caches
        /// </summary>
        /// <param name="cachePolicy">The response.</param>
        /// <param name="cacheExpiryTime">The cache expiry time.</param>
        /// <param name="responseIsIdenticalForEveryUser">if set to <c>true</c> one user may receive a response that was prepared for another user.</param>
        /// <exception cref="System.ArgumentNullException">cachePolicy</exception>
        /// <remarks>This method is for use with ASP.NET WebForms</remarks>
        public void CacheUntil(HttpCachePolicy cachePolicy, DateTime cacheExpiryTime, bool responseIsIdenticalForEveryUser = true)
        {
            CacheUntil(cachePolicy, DateTime.Now, cacheExpiryTime, responseIsIdenticalForEveryUser);
        }

        /// <summary>
        /// Sets a response to be cached for a given amount of time following the initial request, allowing both shared and private caches
        /// </summary>
        /// <param name="cachePolicy">The response.</param>
        /// <param name="cacheExpiryTime">The cache expiry time.</param>
        /// <param name="responseIsIdenticalForEveryUser">if set to <c>true</c> one user may receive a response that was prepared for another user.</param>
        /// <exception cref="System.ArgumentNullException">cachePolicy</exception>
        /// <remarks>This method is for use with ASP.NET MVC</remarks>
        public void CacheUntil(HttpCachePolicyBase cachePolicy, DateTime cacheExpiryTime, bool responseIsIdenticalForEveryUser = true)
        {
            CacheUntil(cachePolicy, DateTime.Now, cacheExpiryTime, responseIsIdenticalForEveryUser);
        }
    }
}
