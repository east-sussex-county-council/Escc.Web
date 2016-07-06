using System;

namespace Escc.Web
{
    /// <summary>
    /// Protect query strings from being tampered with by including a hash of the original value
    /// </summary>
    public interface IUrlProtector
    {
        /// <summary>
        /// Checks a query string protected by <seealso cref="ProtectQueryString(Uri)"/> has not been tampered with.
        /// </summary>
        /// <param name="protectedUrl">The protected URL.</param>
        /// <returns></returns>
        bool CheckProtectedQueryString(Uri protectedUrl);

        /// <summary>
        /// Signs a query string so that you can check that it hasn't been changed.
        /// </summary>
        /// <param name="urlToProtect">The URL to protect.</param>
        /// <returns></returns>
        Uri ProtectQueryString(Uri urlToProtect);
    }
}