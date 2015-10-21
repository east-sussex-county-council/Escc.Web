using System.Collections.Generic;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Provides access to a list of allowed origins for CORS requests.
    /// </summary>
    public interface ICorsAllowedOriginsProvider
    {
        /// <summary>
        /// Gets the allowed origins for CORS requests.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> CorsAllowedOrigins();
    }
}