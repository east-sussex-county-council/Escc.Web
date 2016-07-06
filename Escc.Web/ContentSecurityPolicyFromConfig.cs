using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escc.Web
{
    /// <summary>
    /// Reads Content Security Policy settings from web.config
    /// </summary>
    public class ContentSecurityPolicyFromConfig 
    {
        /// <summary>
        /// Reads the policies from web.config.
        /// </summary>
        /// <returns></returns>
        public NameValueCollection ReadPolicies()
        {
            // Support current and backwards-compatible configuration sections
            var contentSecurity = ConfigurationManager.GetSection("Escc.Web/ContentSecurityPolicies") as NameValueCollection;
            if (contentSecurity == null) contentSecurity = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/ContentSecurityPolicy") as NameValueCollection;
            return contentSecurity;
        }

        /// <summary>
        /// Reads settings from web.config.
        /// </summary>
        /// <returns></returns>
        private NameValueCollection ReadSettings()
        {
            // Support current and backwards-compatible configuration sections
            var contentSecurity = ConfigurationManager.GetSection("Escc.Web/ContentSecurityPolicySettings") as NameValueCollection;
            if (contentSecurity == null) contentSecurity = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/ContentSecurityPolicy") as NameValueCollection;
            return contentSecurity;
        }

        /// <summary>
        /// Gets the policy names to apply by default.
        /// </summary>
        /// <returns></returns>
        public IList<string> DefaultPoliciesToApply()
        {
            // Try current and backwards-compatible setting names
            var settings = ReadSettings();
            var policyNames = settings["PoliciesToApply"];
            if (String.IsNullOrEmpty(policyNames)) policyNames = settings["Apply"];

            if (!String.IsNullOrEmpty(policyNames))
            {
                return policyNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return new string[0];
        }

        /// <summary>
        /// Gets a list of URLs which should not have a Content Security Policy applied
        /// </summary>
        /// <returns></returns>
        public IList<Uri> UrlsToExclude()
        {
            // Try current and backwards-compatible setting names
            var settings = ReadSettings();
            var urlsToExclude = settings["UrlsToExclude"];
            if (String.IsNullOrEmpty(urlsToExclude)) urlsToExclude = settings["None"];

            if (!String.IsNullOrEmpty(urlsToExclude))
            {
                return urlsToExclude.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(url => new Uri(url, UriKind.RelativeOrAbsolute)).ToArray();
            }
            return new Uri[0];
        } 
    }
}
