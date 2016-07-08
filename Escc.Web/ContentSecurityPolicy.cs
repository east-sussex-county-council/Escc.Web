using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Escc.Web
{
    /// <summary>
    /// A Content Security Policy restricts which resources a web page can load, protecting against risks such as cross-site scripting.
    /// </summary>
    public class ContentSecurityPolicy : IContentSecurityPolicy
    {
        private readonly Dictionary<string, IList> _parsedPolicy = new Dictionary<string, IList>();

        /// <summary>
        /// Appends a new Content Security Policy to the existing policy, and returns the updated policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public ContentSecurityPolicy AppendPolicy(ContentSecurityPolicy policy)
        {
            if (policy != null) this.ParsePolicy(policy.ToString());
            return this;
        }

        /// <summary>
        /// Appends a new Content Security Policy to the existing policy, and returns the updated policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public ContentSecurityPolicy AppendPolicy(string policy)
        {
            if (!String.IsNullOrEmpty(policy)) this.ParsePolicy(policy);
            return this;
        }

        /// <summary>
        /// Parses a Content Security Policy HTTP header value, and adds it to the existing policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        private void ParsePolicy(string policy)
        {
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
