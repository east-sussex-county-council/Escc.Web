using System;
using System.Text;

namespace Escc.Web
{
    /// <summary>
    /// Modify URLs for presentation to users, not for use as links
    /// </summary>
    public class UrlPresenter : IUrlPresenter
    {
        /// <summary>
        /// Gets an abridged version of an absolute URL with a maximum of 60 characters, which may not work as a link
        /// </summary>
        /// <param name="urlToAbbreviate">The URL to abbreviate.</param>
        /// <returns></returns>
        public string AbbreviateUrl(Uri urlToAbbreviate)
        {
            return AbbreviateUrl(urlToAbbreviate, null);
        }
        
        /// <summary>
        /// Gets an abridged version of a URL with a maximum of 60 characters, which may not work as a link
        /// </summary>
        /// <param name="urlToAbbreviate">The URL to abbreviate.</param>
        /// <param name="baseUrl">The base URL, typically the URL of the current page.</param>
        /// <returns></returns>
        public string AbbreviateUrl(Uri urlToAbbreviate, Uri baseUrl)
        {
            return AbbreviateUrl(urlToAbbreviate, baseUrl, 60);
        }

        /// <summary>
        /// Gets an abridged version of a URL, which may not work as a link
        /// </summary>
        /// <param name="urlToAbbreviate">The URL.</param>
        /// <param name="baseUrl">The base URL, typically the URL of the current page.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns></returns>
        public string AbbreviateUrl(Uri urlToAbbreviate, Uri baseUrl, int maximumLength)
        {
            if (urlToAbbreviate == null) throw new ArgumentNullException(nameof(urlToAbbreviate));
            if (urlToAbbreviate == null) throw new ArgumentNullException(nameof(baseUrl));

            // Start by getting the host without the protocol
            if (!urlToAbbreviate.IsAbsoluteUri)
            {
                if (!baseUrl.IsAbsoluteUri) throw new ArgumentException("when urlToAbbreviate is a relative URL, baseUrl must be an absolute URL");
                urlToAbbreviate = new Uri(baseUrl, urlToAbbreviate);
            }
            StringBuilder urlString = new StringBuilder();
            if (baseUrl == null || urlToAbbreviate.Host != baseUrl.Host) urlString.Append(urlToAbbreviate.Host);
            if (!urlToAbbreviate.IsDefaultPort) urlString.Append(":" + urlToAbbreviate.Port);

            // Alter maximumLength to reflect the maximum *remaining* length
            maximumLength = maximumLength - urlString.Length;

            // If it's too long, remove chunk by chunk from the start 
            var path = urlToAbbreviate.PathAndQuery.TrimEnd('/');
            var nextSlash = path.IndexOf("/", StringComparison.Ordinal);
            var shortened = false;
            while (path.Length > maximumLength && nextSlash > -1)
            {
                path = path.Substring(nextSlash + 1);
                nextSlash = path.IndexOf("/", StringComparison.Ordinal);

                // Note that the URL has been shortened, and will be prepended by a leading slash and ellipsis
                if (!shortened)
                {
                    maximumLength = maximumLength - 3;
                    shortened = true;
                }
            }
            urlString.Append(shortened ? "/…/" + path : path);

            // It's still too long, can we cut off the querystring?
            if (urlString.Length > maximumLength)
            {
                var cutQuery = urlString.ToString();
                var pos = cutQuery.IndexOf("?", StringComparison.Ordinal);
                if (pos > -1)
                {
                    urlString = new StringBuilder(cutQuery.Remove(pos) + "?…");
                }
            }

            return urlString.ToString();
        }
    }
}
