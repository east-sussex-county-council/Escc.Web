using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Escc.Web
{
    /// <summary>
    /// Expire URLs after a given time or protect query strings from being tampered with 
    /// </summary>
    public class UrlExpirer : IUrlExpirer
    {
        private readonly IUrlProtector _urlProtector;
        private readonly string _timeParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="IUrlProtector" /> class.
        /// </summary>
        /// <param name="urlProtector">A URL protector ensures the expiration time cannot be changed</param>
        /// <param name="timeParameter">Name of the querystring parameter used to store when the link was created.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public UrlExpirer(IUrlProtector urlProtector, string timeParameter="t")
        {
            if (urlProtector == null) throw new ArgumentNullException(nameof(urlProtector));
            _urlProtector = urlProtector;
            _timeParameter = timeParameter;
        }

        /// <summary>
        /// Adds parameters to a URL which allow you to expire it after a set time. Set the time limit when you check if it's expired.
        /// </summary>
        /// <param name="urlToExpire">The URL to protect.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">urlToExpire</exception>
        /// <exception cref="System.ArgumentException">urlToExpire must be an absolute URI</exception>
        public Uri ExpireUrl(Uri urlToExpire)
        {
            return ExpireUrl(urlToExpire, DateTime.UtcNow);
        }

        /// <summary>
        /// Adds parameters to a URL which allow you to expire it after a set time. Set the time limit when you check if it's expired.
        /// </summary>
        /// <param name="urlToExpire">The URL to protect.</param>
        /// <param name="utcTimestamp">The UTC time from which to start the clock on expiry.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">urlToExpire</exception>
        /// <exception cref="System.ArgumentException">urlToExpire must be an absolute URI</exception>
        public Uri ExpireUrl(Uri urlToExpire, DateTime utcTimestamp)
        {
            if (urlToExpire == null) throw new ArgumentNullException("urlToExpire");
            if (!urlToExpire.IsAbsoluteUri) throw new ArgumentException("urlToExpire must be an absolute URI");
            if (utcTimestamp == null) throw new ArgumentNullException("urlToExpire");

            // Add current time, which can be used to expire the link
            var expiringUrl = new Uri(Iri.PrepareUrlForNewQueryStringParameter(urlToExpire) + _timeParameter + "=" + utcTimestamp.ToUniversalTime().ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture), UriKind.Absolute);

            // Protect the URI against tampering, otherwise expiry date can be circumvented
            return _urlProtector.ProtectQueryString(expiringUrl);
        }

        /// <summary>
        /// Determines whether a URL protected by <seealso cref="ExpireUrl(Uri)"/> has expired.
        /// </summary>
        /// <param name="urlToCheck">The URL to check.</param>
        /// <param name="validForSeconds">How many seconds the URL should be valid for.</param>
        /// <returns>
        /// 	<c>true</c> if the URL has expired; otherwise, <c>false</c>.
        /// </returns>
        public bool HasUrlExpired(Uri urlToCheck, int validForSeconds)
        {
            return HasUrlExpired(urlToCheck, validForSeconds, DateTime.UtcNow);
        }

        /// <summary>
        /// Determines whether a URL protected by <seealso cref="ExpireUrl(Uri)" /> has expired.
        /// </summary>
        /// <param name="urlToCheck">The URL to check.</param>
        /// <param name="validForSeconds">How many seconds the URL should be valid for.</param>
        /// <param name="currentUtcTime">The current UTC time.</param>
        /// <returns>
        ///   <c>true</c> if the URL has expired; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlToCheck</exception>
        /// <exception cref="System.ArgumentException">urlToCheck must be an absolute URI</exception>
        public bool HasUrlExpired(Uri urlToCheck, int validForSeconds, DateTime currentUtcTime)
        {
            if (urlToCheck == null) throw new ArgumentNullException("urlToCheck");
            if (!urlToCheck.IsAbsoluteUri) throw new ArgumentException("urlToCheck must be an absolute URI");

            // Check the querystring wasn't tampered with - if it was, it's expired
            if (!_urlProtector.CheckProtectedQueryString(urlToCheck)) return true;

            // Get the querystring in a usable form
            var queryString = Iri.SplitQueryString(urlToCheck.Query);

            // If time has been removed, expire link
            if (!queryString.ContainsKey(_timeParameter)) return true;

            var linkCreated = DateTime.SpecifyKind(DateTime.ParseExact(queryString[_timeParameter], "yyyyMMddHHmmss", CultureInfo.InvariantCulture), DateTimeKind.Utc);
            if (currentUtcTime.ToUniversalTime().Subtract(linkCreated).TotalSeconds > validForSeconds)
            {
                // It's been too long...
                return true;
            }

            return false;
        }
    }
}
