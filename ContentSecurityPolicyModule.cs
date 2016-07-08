using System;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// HTTP module to apply a default content security policy, loaded from web.config. See <see cref="ContentSecurityPolicy" /> for details.
    /// </summary>
    [Obsolete("For .NET 4.5.2 and above, use the Escc.Web NuGet package")]
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
            var contentSecurity = new ContentSecurityPolicy(HttpContext.Current.Request.Url);
            contentSecurity.UpdateHeader(HttpContext.Current.Response);
        }

        #endregion
    }
}
