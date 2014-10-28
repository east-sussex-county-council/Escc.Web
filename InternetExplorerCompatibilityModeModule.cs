using System.Collections.Specialized;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Allow compatibility mode to be set for a site by matching problem URLs to specific modes
    /// </summary>
    public class InternetExplorerCompatibilityModeModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) =>
            {
                var settings = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/InternetExplorerCompatibilityMode") as NameValueCollection;
                if (settings == null) return;

                foreach (string urlPattern in settings)
                {
                    if (Regex.IsMatch(context.Request.Url.PathAndQuery, urlPattern, RegexOptions.IgnoreCase))
                    {
                        context.Response.AddHeader("X-UA-Compatible", settings[urlPattern]);
                        break;
                    }
                }
            };
        }

        #endregion
    }
}
