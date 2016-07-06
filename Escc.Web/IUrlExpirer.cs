using System;

namespace Escc.Web
{
    /// <summary>
     /// Expire URLs after a given time 
     /// </summary>
    public interface IUrlExpirer
    {
        /// <summary>
        /// Adds parameters to a URL which allow you to expire it after a set time. Set the time limit when you check if it's expired.
        /// </summary>
        /// <param name="urlToExpire">The URL to protect.</param>
        /// <returns></returns>
        Uri ExpireUrl(Uri urlToExpire);

        /// <summary>
        /// Determines whether a URL protected by <seealso cref="ExpireUrl"/> has expired.
        /// </summary>
        /// <param name="urlToCheck">The URL to check.</param>
        /// <param name="validForSeconds">How many seconds the URL should be valid for.</param>
        /// <returns>
        /// 	<c>true</c> if the URL has expired; otherwise, <c>false</c>.
        /// </returns>
        bool HasUrlExpired(Uri urlToCheck, int validForSeconds);
    }
}