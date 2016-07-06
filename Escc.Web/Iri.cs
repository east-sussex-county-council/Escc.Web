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
    /// Methods for working with the Internationalized Resource Identifier (IRI) standard, which includes URIs and URLs
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iri")]
    public static class Iri
    {
        #region Convert URIs from one form to another
        /// <summary>
        /// Makes a relative URI absolute, by combining it with an absolute URI to which it is relative.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="baseUri">The absolute base URI to which the relative URI is compared.</param>
        /// <param name="ignoreBaseUriQueryString">if set to <c>true</c> ignore the base URI's query string.</param>
        /// <returns>An absolute URI</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Uri MakeAbsolute(Uri relativeUri, Uri baseUri, bool ignoreBaseUriQueryString)
        {
            // Check arguments
            if (relativeUri == null) throw new ArgumentNullException("relativeUri");
            if (baseUri == null) throw new ArgumentNullException("baseUri");
            if (!baseUri.IsAbsoluteUri) throw new ArgumentException("The base URI must be an absolute URI", "baseUri");

            // Check whether the URI is protocol-relative or absolute already
            relativeUri = HandleProtocolRelativeUrl(relativeUri, baseUri);
            if (relativeUri.IsAbsoluteUri) return relativeUri;

            // Split the relative URI into path and query, which the Uri object itself can't do.
            // The query string will include any fragment identifier as well, but it ends up in the right place so no need to separate it.
            string relativePath = relativeUri.OriginalString;
            string relativeQuery = String.Empty;
            int queryPos = relativePath.IndexOf("?", StringComparison.Ordinal);
            if (queryPos > -1)
            {
                relativeQuery = relativePath.Substring(queryPos + 1);
                relativePath = relativePath.Substring(0, queryPos);
            }

            // Take the absolute URI and add a relative path, making a new absolute URI
            var absoluteUri = new StringBuilder(baseUri.Scheme);
            absoluteUri.Append("://");
            absoluteUri.Append(baseUri.Host);
            if (!baseUri.IsDefaultPort) absoluteUri.Append(":").Append(baseUri.Port);
            if (relativePath.StartsWith("/", StringComparison.Ordinal))
            {
                absoluteUri.Append(relativePath);
            }
            else
            {
                absoluteUri.Append(VirtualPathUtility.Combine(baseUri.AbsolutePath, relativePath));
                // = baseUri.AbsolutePath.Substring(0, baseUri.AbsolutePath.Length - Path.GetFileName(baseUri.AbsolutePath).Length) + relativePath;
            }

            // If the relative URI had a querystring, need to combine that with the querystring of the absolute URI
            // unless specifically requested to ignore it
            bool hasQuery = false;
            if (!String.IsNullOrEmpty(baseUri.Query) && !ignoreBaseUriQueryString)
            {
                absoluteUri.Append("?").Append(baseUri.Query);
                hasQuery = true;
            }

            if (!String.IsNullOrEmpty(relativeQuery))
            {
                absoluteUri.Append(hasQuery ? "&" : "?");
                absoluteUri.Append(relativeQuery);
            }

            return new Uri(absoluteUri.ToString());
        }

        /// <summary>
        /// System.Uri gets protocol-relative URLs wrong. This looks for a mis-interpreted protocol-relative URL and converts it to absolute
        /// </summary>
        /// <param name="relativeUri">The protocol-relative URI.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>Fixed URL, or the original URL if not protocol-relative</returns>
        private static Uri HandleProtocolRelativeUrl(Uri relativeUri, Uri baseUri)
        {
            if (relativeUri == null) throw new ArgumentNullException("relativeUri");
            if (baseUri == null) throw new ArgumentNullException("baseUri");
            
            // System.Uri sees protocol-relative URLs as file URIs, so look for that pattern before taking action
            if (relativeUri.OriginalString.StartsWith(@"//", StringComparison.OrdinalIgnoreCase) && relativeUri.Scheme == "file")
            {
                relativeUri = new Uri(baseUri.Scheme + "://" + baseUri.Authority + relativeUri.ToString().Substring(6));
            }
            return relativeUri;
        }

        /// <summary>
        /// Makes a relative URI absolute, by combining it with an absolute URI to which it is relative.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="baseUri">The absolute base URI to which the relative URI is compared.</param>
        /// <returns>An absolute URI</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Uri MakeAbsolute(Uri relativeUri, Uri baseUri)
        {
            return MakeAbsolute(relativeUri, baseUri, true);
        }

        /// <summary>
        /// Makes a relative URI absolute, using the current HTTP request as a base URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="ignoreBaseUriQueryString">if set to <c>true</c> ignore the base URI's query string.</param>
        /// <returns>An absolute URI</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Uri MakeAbsolute(Uri relativeUri, bool ignoreBaseUriQueryString)
        {
            // If we can return without attempting to access HttpContext.Current then do so
            if (relativeUri == null) throw new ArgumentNullException("relativeUri");
            if (relativeUri.IsAbsoluteUri && !relativeUri.OriginalString.StartsWith(@"//", StringComparison.OrdinalIgnoreCase)) return relativeUri;
            return MakeAbsolute(relativeUri, HttpContext.Current.Request.Url, ignoreBaseUriQueryString);
        }

        /// <summary>
        /// Makes a relative URI absolute, using the current HTTP request as a base URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns>An absolute URI</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="System.Security.SecurityException">Thrown if this assembly is not granted <see cref="System.Web.AspNetHostingPermission"/> to access the current HTTP request</exception>
        public static Uri MakeAbsolute(Uri relativeUri)
        {
            // If we can return without attempting to access HttpContext.Current then do so
            if (relativeUri == null) throw new ArgumentNullException("relativeUri");
            if (relativeUri.IsAbsoluteUri && !relativeUri.OriginalString.StartsWith(@"//", StringComparison.OrdinalIgnoreCase)) return relativeUri;
            return MakeAbsolute(relativeUri, HttpContext.Current.Request.Url);
        }

        /// <summary>
        /// Gets an abridged version of a URL formatted for display, not for use as a link
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ShortenForDisplay(Uri url)
        {
            return ShortenForDisplay(url, 60);
        }

        /// <summary>
        /// Gets an abridged version of a URL formatted for display, not for use as a link
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns></returns>
        public static string ShortenForDisplay(Uri url, int maximumLength)
        {
            // Start by getting the host without the protocol
            url = MakeAbsolute(url, true);
            StringBuilder urlString = new StringBuilder();
            if (HttpContext.Current == null || url.Host != HttpContext.Current.Request.Url.Host) urlString.Append(url.Host);
            if (!url.IsDefaultPort) urlString.Append(":" + url.Port);

            // Alter maximumLength to reflect the maximum *remaining* length
            maximumLength = maximumLength - urlString.Length;

            // If it's too long, remove chunk by chunk from the start 
            var path = url.PathAndQuery.TrimEnd('/');
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

        #endregion

        #region Generic functions to work with query strings
        /// <summary>
        /// Splits a query string into its name and value pairs.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        public static Dictionary<string, string> SplitQueryString(string queryString)
        {
            var dictionary = new Dictionary<string, string>();

            if (queryString != null) queryString = queryString.TrimStart('?');

            if (!String.IsNullOrEmpty(queryString))
            {
                var pairs = queryString.Split('&');
                foreach (string pair in pairs)
                {
                    var nameValue = pair.Split(new char[] { '=' }, 2);
                    if (nameValue.Length == 2 && nameValue[0].Length > 0)
                    {
                        if (dictionary.ContainsKey(nameValue[0]))
                        {
                            dictionary[nameValue[0]] += ("," + nameValue[1]);
                        }
                        else
                        {
                            dictionary.Add(nameValue[0], nameValue[1]);
                        }
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Remove any existing parameter with a specified key from a given query string
        /// </summary>
        /// <param name="urlToEdit">The URL to edit.</param>
        /// <param name="parameterName">Parameter key</param>
        /// <returns>Updated URL</returns>
        public static Uri RemoveQueryStringParameter(Uri urlToEdit, string parameterName)
        {
            if (urlToEdit == null) throw new ArgumentNullException("urlToEdit");
            if (parameterName == null) throw new ArgumentNullException("parameterName");

            // Check there is a querystring, and split it
            Uri absoluteUrl = (urlToEdit.IsAbsoluteUri) ? urlToEdit : Iri.MakeAbsolute(urlToEdit);
            if (String.IsNullOrEmpty(absoluteUrl.Query)) return urlToEdit;
            var originalQuery = Iri.SplitQueryString(absoluteUrl.Query);

            // Rebuild query string without its parameter= value
            StringBuilder updatedQueryString = new StringBuilder();

            foreach (string parameter in originalQuery.Keys)
            {
                if (parameter == parameterName) continue;

                if (updatedQueryString.Length > 0) updatedQueryString.Append("&");
                updatedQueryString.Append(parameter).Append("=").Append(originalQuery[parameter]);
            }

            // Remove the original querystring from the original URL, and add this one
            string updatedUrl = urlToEdit.ToString();
            var fragment = String.Empty;
            int pos = updatedUrl.IndexOf("?", StringComparison.Ordinal);
            if (pos > -1)
            {
                // check for a fragment identifier before discarding
                var fragmentPos = updatedUrl.IndexOf("#", StringComparison.Ordinal);
                if (fragmentPos > -1 && fragmentPos > pos)
                {
                    fragment = updatedUrl.Substring(fragmentPos);
                }
                updatedUrl = updatedUrl.Substring(0, pos);
            }

            if (updatedQueryString.Length > 0) updatedUrl += "?" + updatedQueryString.ToString();
            if (fragment.Length > 0) updatedUrl += fragment;

            return new Uri(updatedUrl, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Prepares a URL for a new query string parameter to be added. Discards any fragment identifier.
        /// </summary>
        /// <param name="urlToPrepare">The URL to prepare.</param>
        /// <returns></returns>
        public static string PrepareUrlForNewQueryStringParameter(Uri urlToPrepare)
        {
            if (urlToPrepare == null) throw new ArgumentNullException("urlToPrepare");

            // Build up current URL as a string ready to have extra query string parameters added
            var preparedUrl = new StringBuilder();

            // If we can, rely on the built-in way to split up the URL
            var query = String.Empty;
            if (urlToPrepare.IsAbsoluteUri)
            {
                preparedUrl.Append(urlToPrepare.Scheme).Append("://").Append(urlToPrepare.Host);
                if (!urlToPrepare.IsDefaultPort) preparedUrl.Append(":").Append(urlToPrepare.Port);
                preparedUrl.Append(urlToPrepare.AbsolutePath);
                query = urlToPrepare.Query;
            }
            else
            {
                // Otherwise split it up ourselves
                var url = urlToPrepare.ToString();
                var pos = url.IndexOf("?", StringComparison.Ordinal);

                var path = String.Empty;
                if (pos > -1)
                {
                    // If there's a querystring...
                    path = url.Substring(0, pos);
                    query = url.Substring(pos);

                    // Is there also a fragment? Need to remove it. Can't save it because it should go after the
                    // query, and the point of this function is to have a querystring ready for appending to.
                    pos = query.IndexOf("#", StringComparison.Ordinal);
                    if (pos > -1)
                    {
                        query = query.Substring(0, pos);
                    }
                }
                else
                {
                    // If there's no querystring, still need to check for a fragment
                    pos = url.IndexOf("#", StringComparison.Ordinal);
                    if (pos > -1)
                    {
                        path = url.Substring(0, pos);
                    }
                }

                preparedUrl.Append(path);
            }

            if (String.IsNullOrEmpty(query) || query == "?")
            {
                preparedUrl.Append("?");
            }
            else
            {
                preparedUrl.Append(query).Append("&");
            }
            return preparedUrl.ToString();
        }
        #endregion

        #region Parse sections from URLs

        /// <summary>
        /// Lists files and folders in the path of an absolute URL or a URL relative to the site root. Trailing slashes are trimmed.
        /// </summary>
        /// <param name="urlToParse">The URL to parse.</param>
        /// <returns>List of paths, starting with the most specific and ending with only the root folder or filename</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IList<string> ListFilesAndFoldersInPath(Uri urlToParse)
        {
            if (urlToParse == null) throw new ArgumentNullException("urlToParse");

            // get path to process
            var path = String.Empty;
            if (urlToParse.IsAbsoluteUri)
            {
                path = urlToParse.AbsolutePath;
            }
            else if (urlToParse.ToString().StartsWith("/", StringComparison.Ordinal))
            {
                path = urlToParse.ToString();
                var querystring = path.IndexOf("?", StringComparison.Ordinal);
                if (querystring > -1) path = path.Substring(0, querystring);
            }
            else
            {
                throw new ArgumentException("urlToParse must be an absolute URL or a relative URL which begins with /", "urlToParse");
            }

            // Build up a list of paths, knocking off one segment at a time
            var paths = new List<string>();

            while (path.Length > 0)
            {
                paths.Add(path);
                var slashIndex = path.LastIndexOf("/", StringComparison.Ordinal);
                if (slashIndex == -1) break;
                path = path.Substring(0, slashIndex);
            }
            paths.Add("/"); // because we knock off the trailing slash, need to hard-code adding the root which is only a trailing slash

            return paths;
        }

        #endregion
    }
}
