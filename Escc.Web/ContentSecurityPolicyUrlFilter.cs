using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escc.Web
{
    /// <summary>
    /// Determines whether to apply a Content Security Policy by matching against a list of excluded URLs
    /// </summary>
    public class ContentSecurityPolicyUrlFilter : IContentSecurityPolicyFilter
    {
        private readonly Uri _targetUrl;
        private readonly IList<Uri> _urlsToExclude;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSecurityPolicyUrlFilter" /> class.
        /// </summary>
        /// <param name="targetUrl">The URL.</param>
        /// <param name="urlsToExclude">The urls to exclude.</param>
        public ContentSecurityPolicyUrlFilter(Uri targetUrl, IList<Uri> urlsToExclude)
        {
            _targetUrl = targetUrl;
            _urlsToExclude = urlsToExclude;
        }

        /// <summary>
        /// Determines whether to apply the Content Security Policy 
        /// </summary>
        /// <returns></returns>
        public bool ApplyPolicy()
        {
            if (_urlsToExclude == null) return true;

            foreach (var excludedUrl in _urlsToExclude)
            {
                if (_targetUrl.AbsolutePath.StartsWith(excludedUrl.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
