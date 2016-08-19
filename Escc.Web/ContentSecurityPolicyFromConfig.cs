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
        private NameValueCollection _policies;
        private IList<string> _defaultPoliciesToApply;
        private IList<Uri> _urlsToExclude;

        /// <summary>
        /// Content Security Policies which can be combined to create a composite policy
        /// </summary>
        /// <returns></returns>
        public NameValueCollection Policies
        {
            get
            {
                if (_policies == null) _policies = ReadPolicies();
                return _policies;
            }
        }

        /// <summary>
        /// Gets the policy names to apply by default.
        /// </summary>
        /// <returns></returns>
        public IList<string> DefaultPoliciesToApply
        {
            get
            {
                if (_defaultPoliciesToApply == null) _defaultPoliciesToApply = ReadDefaultPoliciesToApply();
                return _defaultPoliciesToApply;
            }
        }

        /// <summary>
        /// Gets a list of URLs which should not have a Content Security Policy applied
        /// </summary>
        /// <returns></returns>
        public IList<Uri> UrlsToExclude
        {
            get
            {
                if (_urlsToExclude == null) _urlsToExclude = ReadUrlsToExclude();
                return _urlsToExclude;
            }
        } 


        private NameValueCollection ReadPolicies()
        {
            // Support current and backwards-compatible configuration sections
            var contentSecurity = ConfigurationManager.GetSection("Escc.Web/ContentSecurityPolicies") as NameValueCollection;
            if (contentSecurity == null) contentSecurity = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/ContentSecurityPolicy") as NameValueCollection;
            return contentSecurity;
        }

        private NameValueCollection ReadSettings()
        {
            // Support current and backwards-compatible configuration sections
            var contentSecurity = ConfigurationManager.GetSection("Escc.Web/ContentSecurityPolicySettings") as NameValueCollection;
            if (contentSecurity == null) contentSecurity = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/ContentSecurityPolicy") as NameValueCollection;
            return contentSecurity;
        }

        private IList<string> ReadDefaultPoliciesToApply()
        {
            // Try current and backwards-compatible setting names
            var settings = ReadSettings();
            if (settings == null) return new string[0];

            var policyNames = settings["PoliciesToApply"];
            if (String.IsNullOrEmpty(policyNames)) policyNames = settings["Apply"];

            if (!String.IsNullOrEmpty(policyNames))
            {
                return policyNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return new string[0];
        }

       private IList<Uri> ReadUrlsToExclude()
        {
            // Try current and backwards-compatible setting names
            var settings = ReadSettings();
            if (settings == null) return new Uri[0];

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
