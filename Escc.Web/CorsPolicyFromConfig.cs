using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Web.Cors;

namespace Escc.Web
{
    /// <summary>
    /// Gets configuration for the domains allowed to make CORS requests from web.config or app.config
    /// </summary>
    [SecuritySafeCritical]
    public class CorsPolicyFromConfig 
    {
        /// <summary>
        /// Gets the CORS policy.
        /// </summary>
        /// <value>
        /// The CORS policy.
        /// </value>
        public CorsPolicy CorsPolicy { get; }
      
        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyFromConfig"/> class.
        /// </summary>
        public CorsPolicyFromConfig()
        {
            CorsPolicy = new CorsPolicy
            {
                AllowAnyMethod = false,
                AllowAnyHeader = false,
                AllowAnyOrigin = false
            };

            var allowedOrigins = CorsAllowedOrigins();
            foreach (var origin in allowedOrigins)
            {
                CorsPolicy.Origins.Add(origin);
            }
        }

        /// <summary>
        /// Gets the allowed origins for CORS requests.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> CorsAllowedOrigins()
        {
            // Preferred configuration setting
            var allowedOrigins = ReadSettingFromConfigSection("Escc.Web/Cors", "AllowedOrigins");

            // Backwards compatibility with old versions of this setting
            if (String.IsNullOrEmpty(allowedOrigins)) allowedOrigins = ReadSettingFromConfigSection("EsccWebTeam.Data.Web/Cors", "AllowedOrigins");
            if (String.IsNullOrEmpty(allowedOrigins)) allowedOrigins = ReadSettingFromConfigSection("EsccWebTeam.EastSussexGovUK/RemoteMasterPage", "CorsAllowedOrigins");

            if (!String.IsNullOrEmpty(allowedOrigins))
            {
                return new List<string>(allowedOrigins.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
            }

            return new string[0];
        }

        private static string ReadSettingFromConfigSection(string configSection, string configSetting)
        {
            var config = ConfigurationManager.GetSection(configSection) as NameValueCollection;
            if (config != null && !String.IsNullOrEmpty(config[configSetting]))
            {
                return config[configSetting];
            }
            return null;
        }
    }
}
