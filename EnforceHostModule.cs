using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Ensures that the current request uses the correct host
    /// </summary>
    public class EnforceHostModule : IHttpModule
    {
        /// <summary>
        /// Initialises the specified HTTP application.
        /// </summary>
        /// <param name="context">The HTTP application.</param>
        public void Init(HttpApplication context)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.PreRequestHandlerExecute += new EventHandler(this.OnPreRequestHandlerExecute);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do.
        }

        /// <summary>
        /// Called before the main request is executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            NameValueCollection config = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/EnforceHostModule") as NameValueCollection;
            if (config == null) return;

            // If there's a specific setting for this host
            Uri currentUrl = HttpContext.Current.Request.Url;
            string currentHost = currentUrl.Host.ToLower(CultureInfo.CurrentCulture);
            if (config[currentHost] != null)
            {
                // Change the host to the preferred alternative
                UriBuilder preferredUri = new UriBuilder(currentUrl);
                preferredUri.Host = config[currentHost];

                // ...and redirect
                HttpResponse response = HttpContext.Current.Response;
                response.Status = "301 Moved Permanently";
                response.AddHeader("Location", preferredUri.ToString());
                response.End();
            }
        }

    }
}
