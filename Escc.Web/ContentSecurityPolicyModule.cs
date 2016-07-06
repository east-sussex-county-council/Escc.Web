using System;
using System.Web;

namespace Escc.Web
{
    /// <summary>
    /// HTTP module to apply a default Content Security Policy, loaded from web.config. 
    /// </summary>
    public class ContentSecurityPolicyModule : IHttpModule
    {
        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            // Read the policy settings from web.config
            var config = new ContentSecurityPolicyFromConfig();
            var policies = config.ReadPolicies();
            if (policies == null) return;

            // Support excluding URLs from the policy
            var urlsToExclude = config.UrlsToExclude();
            var filter = new ContentSecurityPolicyUrlFilter(HttpContext.Current.Request.Url, urlsToExclude);
            if (!filter.ApplyPolicy()) return;

            var contentSecurity = new ContentSecurityPolicyHeaders(HttpContext.Current.Response.Headers);

            // Default to loading two policies, "Default" and "Local", but allow that to be overridden with a custom list
            var defaultPolicyNames = config.DefaultPoliciesToApply();
            if (defaultPolicyNames.Count > 0)
            {
                foreach (var policyName in defaultPolicyNames)
                {
                    contentSecurity.AppendPolicy(policies[policyName]);
                }
            }
            else
            {
                contentSecurity.AppendPolicy(policies["Default"]).AppendPolicy(policies["Local"]);
            }

            // Apply the policy
            contentSecurity.UpdateHeaders();
        }

        #endregion
    }
}
