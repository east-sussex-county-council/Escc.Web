using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// A Content Security Policy restricts which resources a web page can load, protecting against risks such as cross-site scripting.
    /// </summary>
    public class ContentSecurityPolicy
    {
        private readonly Dictionary<string, IList> _parsedPolicy = new Dictionary<string, IList>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSecurityPolicy" /> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public ContentSecurityPolicy(Uri url)
        {
            if (!IsExcludedUrl(url))
            {
                var contentSecurity = LoadConfigurationData();
                if (contentSecurity != null && !String.IsNullOrEmpty(contentSecurity["Apply"]))
                {
                    AppendFromConfig(contentSecurity["Apply"].Split(';'));
                }
                else
                {
                    AppendFromConfig("Default", "Dev");                    
                }
            }
        }

        private bool IsExcludedUrl(Uri url)
        {
            var contentSecurity = LoadConfigurationData();
            if (contentSecurity == null) return false;
            if (String.IsNullOrEmpty(contentSecurity["None"])) return false;

            var excluded = contentSecurity["None"].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var excludedUrl in excluded)
            {
                if (url.AbsolutePath.StartsWith(excludedUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static NameValueCollection LoadConfigurationData()
        {
            var contentSecurity = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/ContentSecurityPolicy") as NameValueCollection;
            return contentSecurity;
        }

        /// <summary>
        /// Parses a content security policy HTTP header value.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="replaceExisting">if set to <c>true</c> replace the existing policy; if <c>false</c> append the new settings to it.</param>
        public void ParsePolicy(string policy, bool replaceExisting = false)
        {
            if (String.IsNullOrEmpty(policy)) throw new ArgumentNullException(policy);

            if (replaceExisting) _parsedPolicy.Clear();

            var directives = policy.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string directive in directives)
            {
                ParseDirective(directive);
            }
        }

        private void ParseDirective(string directive)
        {
            var splitDirective = directive.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitDirective.Length > 1)
            {
                var sources = new List<string>(splitDirective);
                var directiveType = sources[0];
                sources.RemoveAt(0);

                ParseSources(directiveType, sources);
            }
        }

        private void ParseSources(string directiveType, IEnumerable<string> sources)
        {
            if (!_parsedPolicy.ContainsKey(directiveType)) _parsedPolicy.Add(directiveType, new List<string>());

            foreach (string source in sources)
            {
                if (!_parsedPolicy[directiveType].Contains(source)) _parsedPolicy[directiveType].Add(source);
            }
        }

        /// <summary>
        /// Gets a content security policy from the application configuration file, parses it and appends it to the current policy
        /// </summary>
        /// <param name="policyNames">The policy names to append, used as configuration keys.</param>
        public void AppendFromConfig(params string[] policyNames)
        {
            var contentSecurity = LoadConfigurationData();
            if (contentSecurity == null) return;

            foreach (var policyName in policyNames)
            {
                if (!String.IsNullOrEmpty(contentSecurity[policyName]))
                {
                    this.ParsePolicy(contentSecurity[policyName]);
                }
            }
        }

        /// <summary>
        /// Updates the headers for an HTTP response to reflect the current policy.
        /// </summary>
        /// <param name="response">The response.</param>
        public void UpdateHeader(HttpResponse response)
        {
            response.Headers.Remove("Content-Security-Policy");
            response.AddHeader("Content-Security-Policy", this.ToString());
        }

        /// <summary>
        /// Returns the current policy in a format suitable for a Content-Security-Policy HTTP header
        /// </summary>
        public override string ToString()
        {
            var policy = new StringBuilder();
            foreach (var directiveType in _parsedPolicy.Keys)
            {
                if (policy.Length > 0) policy.Append(";");
                policy.Append(directiveType);

                foreach (var source in _parsedPolicy[directiveType])
                {
                    policy.Append(" ").Append(source);
                }
            }
            return policy.ToString();
        }
    }
}
