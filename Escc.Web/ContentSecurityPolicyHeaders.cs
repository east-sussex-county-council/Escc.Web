using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace Escc.Web
{
    /// <summary>
    /// Read and update the <c>Content-Security-Policy</c> header of an HTTP response
    /// </summary>
    public class ContentSecurityPolicyHeaders : IContentSecurityPolicyHeaders
    {
        private readonly NameValueCollection _responseHeadersCollection;
        private readonly HttpHeaders _responseHeadersObject;
        private ContentSecurityPolicy _policy = new ContentSecurityPolicy();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSecurityPolicyHeaders"/> class.
        /// </summary>
        /// <param name="responseHeaders">The response headers.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ContentSecurityPolicyHeaders(NameValueCollection responseHeaders)
        {
            if (responseHeaders == null) throw new ArgumentNullException(nameof(responseHeaders));
            _responseHeadersCollection = responseHeaders;
            ReadPolicyFromHeaders(_responseHeadersCollection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSecurityPolicyHeaders"/> class.
        /// </summary>
        /// <param name="responseHeaders">The response headers.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ContentSecurityPolicyHeaders(HttpResponseHeaders responseHeaders)
        {
            if (responseHeaders == null) throw new ArgumentNullException(nameof(responseHeaders));
            _responseHeadersObject = responseHeaders;
            ReadPolicyFromHeaders(_responseHeadersObject);
        }

        private void ReadPolicyFromHeaders(NameValueCollection responseHeaders)
        {
            if (responseHeaders != null && responseHeaders["Content-Security-Policy"] != null)
            {
                _policy.AppendPolicy(responseHeaders["Content-Security-Policy"]);
            }
        }

        private void ReadPolicyFromHeaders(HttpHeaders responseHeaders)
        {
            if (responseHeaders != null)
            {
                IEnumerable<string> headers;
                if (responseHeaders.TryGetValues("Content-Security-Policy", out headers))
                {
                    foreach (var header in headers)
                    {
                        _policy.AppendPolicy(header);
                    }
                }
            }
        }

        /// <summary>
        /// Appends a Content Security Policy to the existing policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public ContentSecurityPolicyHeaders AppendPolicy(string policy)
        {
            _policy.AppendPolicy(policy);
            return this;
        }

        /// <summary>
        /// Appends a Content Security Policy to the existing policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public ContentSecurityPolicyHeaders AppendPolicy(ContentSecurityPolicy policy)
        {
            _policy.AppendPolicy(policy.ToString());
            return this;
        }

        /// <summary>
        /// Replaces the existing Content Security Policy with a new one
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public ContentSecurityPolicyHeaders ReplacePolicy(string policy)
        {
            _policy = new ContentSecurityPolicy();
            return AppendPolicy(policy);
        }

        /// <summary>
        /// Replaces the existing Content Security Policy with a new one
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public ContentSecurityPolicyHeaders ReplacePolicy(ContentSecurityPolicy policy)
        {
            _policy = policy;
            return this;
        }

        /// <summary>
        /// Updates the HTTP response headers with the amended Content Security Policy.
        /// </summary>
        public void UpdateHeaders()
        {
            if (_responseHeadersCollection != null)
            {
                UpdateHeaders(_responseHeadersCollection, _policy);
            }
            else if (_responseHeadersObject != null)
            {
                UpdateHeaders(_responseHeadersObject, _policy);
            }
        }

        private static void UpdateHeaders(NameValueCollection responseHeaders, ContentSecurityPolicy policy)
        {
            responseHeaders.Remove("Content-Security-Policy");
            responseHeaders.Add("Content-Security-Policy", policy.ToString());
        }

        private static void UpdateHeaders(HttpHeaders responseHeaders, ContentSecurityPolicy policy)
        {
            responseHeaders.Remove("Content-Security-Policy");
            responseHeaders.Add("Content-Security-Policy", policy.ToString());
        }
    }
}
