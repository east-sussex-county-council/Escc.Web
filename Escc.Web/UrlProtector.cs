using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Escc.Web
{
    /// <summary>
    /// Protect query strings from being tampered with by including a hash of the original value
    /// </summary>
    public class UrlProtector : IUrlProtector
    {
        private readonly string _salt;
        private readonly string _hashParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlProtector" /> class.
        /// </summary>
        /// <param name="salt">The unique salt used to protect this URL.</param>
        /// <param name="hashParameter">Name of the querystring parameter used to store the hash that prevents the time being modified.</param>
        public UrlProtector(string salt, string hashParameter="h")
        {
            if (String.IsNullOrEmpty(salt)) throw new ArgumentNullException(nameof(salt));

            _salt = salt;
            _hashParameter = hashParameter;
        }

        /// <summary>
        /// Signs a query string so that you can check that it hasn't been changed.
        /// </summary>
        /// <param name="urlToProtect">The URL to protect.</param>
        /// <returns></returns>
        public Uri ProtectQueryString(Uri urlToProtect)
        {
            if (urlToProtect == null) throw new ArgumentNullException("urlToProtect");
            if (!urlToProtect.IsAbsoluteUri) throw new ArgumentException("urlToProtect must be an absolute URI");

            // Build up current URL as a string ready to have extra query string parameters added
            var query = HttpUtility.ParseQueryString(urlToProtect.Query);
            query.Add(_hashParameter, CreateUrlHash(urlToProtect.Query, _salt));
            var protectedUrl = new Uri(urlToProtect.Scheme + "://" + urlToProtect.Authority + urlToProtect.AbsolutePath + "?" + query, UriKind.Absolute);


            // Hash the URI and add it as a parameter
            return protectedUrl;
        }


        /// <summary>
        /// Checks a query string protected by <seealso cref="ProtectQueryString(Uri)"/> has not been tampered with.
        /// </summary>
        /// <param name="protectedUrl">The protected URL.</param>
        /// <returns></returns>
        public bool CheckProtectedQueryString(Uri protectedUrl)
        {
            if (protectedUrl == null) throw new ArgumentNullException("protectedUrl");
            if (!protectedUrl.IsAbsoluteUri) throw new ArgumentException("protectedUrl must be an absolute URL", "protectedUrl");

            // Get the querystring in a usable form
            var queryString = HttpUtility.ParseQueryString(protectedUrl.Query);

            // Check we got a hash, otherwise we know straight away it's not valid
            if (String.IsNullOrEmpty(queryString[_hashParameter])) return false;

            // Make a new querystring without the protection parameter. This should be the original protected querystring.
            var protectedQueryString = new StringBuilder();
            foreach (string name in queryString.Keys)
            {
                if (name == _hashParameter) continue;
                if (protectedQueryString.Length > 0) protectedQueryString.Append("&");
                protectedQueryString.Append(name).Append("=").Append(queryString[name]);
            }

            // Determine what the hash SHOULD be
            var expectedHash = CreateUrlHash(protectedQueryString.ToString(), _salt);

            // Check that against what we actually got
            var receivedHash = queryString[_hashParameter];
            if (String.IsNullOrEmpty(receivedHash))
            {
                // No hash, so not valid
                return false;
            }

            // Now, see if the received and expected hashes match up
            return (expectedHash == receivedHash);
        }

        /// <summary>
        /// Creates the hash used to protect a URL.
        /// </summary>
        /// <param name="hashThis">The URL to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <returns></returns>
        private static string CreateUrlHash(string hashThis, string salt)
        {
            hashThis = salt + hashThis.TrimStart('?') + salt;

            var encoder = new System.Text.UTF8Encoding();
            using (var hasher = new SHA1CryptoServiceProvider())
            {
                var hashedBytes = hasher.ComputeHash(encoder.GetBytes(hashThis));

                // Base-64 encode the results and strip out any characters that might get URL encoded and cause hash not to match
                return Regex.Replace(Convert.ToBase64String(hashedBytes), "[^A-Za-z0-9]", String.Empty);
            }
        }
    }
}
