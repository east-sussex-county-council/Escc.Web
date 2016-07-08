using System;

namespace Escc.Web
{
    /// <summary>
    /// Modify URLs for presentation to users, not for use as links
    /// </summary>
    public interface IUrlPresenter
    {
        /// <summary>
        /// Gets an abridged version of an absolute URL with a maximum of 60 characters, which may not work as a link
        /// </summary>
        /// <param name="urlToAbbreviate">The URL to abbreviate.</param>
        /// <returns></returns>
        string AbbreviateUrl(Uri urlToAbbreviate);

        /// <summary>
        /// Gets an abridged version of a URL with a maximum of 60 characters, which may not work as a link
        /// </summary>
        /// <param name="urlToAbbreviate">The URL to abbreviate.</param>
        /// <param name="baseUrl">The base URL, typically the URL of the current page.</param>
        /// <returns></returns>
        string AbbreviateUrl(Uri urlToAbbreviate, Uri baseUrl);

        /// <summary>
        /// Gets an abridged version of a URL, which may not work as a link
        /// </summary>
        /// <param name="urlToAbbreviate">The URL.</param>
        /// <param name="baseUrl">The base URL, typically the URL of the current page.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns></returns>
        string AbbreviateUrl(Uri urlToAbbreviate, Uri baseUrl, int maximumLength);
    }
}