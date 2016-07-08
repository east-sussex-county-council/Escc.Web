using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Gets configuration for the remote master page from web.config
    /// </summary>
    [Obsolete("For .NET 4.5.2 and above, use the Escc.Web NuGet package")]
    public class ConfigurationCorsAllowedOriginsProvider : ICorsAllowedOriginsProvider
    {
        /// <summary>
        /// Gets the allowed origins for CORS requests.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CorsAllowedOrigins()
        {
            var allowedOrigins = String.Empty;

            // Preferred configuration setting
            var config = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/Cors") as NameValueCollection;
            if (config != null && !String.IsNullOrEmpty(config["AllowedOrigins"]))
            {
                allowedOrigins = config["AllowedOrigins"];
            }

            // Backwards compatibility with original configuration setting, when this class was in a different assembly
            config = ConfigurationManager.GetSection("EsccWebTeam.EastSussexGovUK/RemoteMasterPage") as NameValueCollection;
            if (config != null && !String.IsNullOrEmpty(config["CorsAllowedOrigins"]))
            {
                allowedOrigins = config["CorsAllowedOrigins"];
            }

            if (!String.IsNullOrEmpty(allowedOrigins))
            {
                return new List<string>(allowedOrigins.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
            }

            return new string[0];
        }
    }
}