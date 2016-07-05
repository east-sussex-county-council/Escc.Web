using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace Escc.Web
{
    /// <summary>
    /// Apply <see cref="CorsPolicyFromConfig"/> as an attribute on a Web API class or method
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="System.Web.Http.Cors.ICorsPolicyProvider" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    [SecurityCritical]
    public class CorsPolicyFromConfigAttribute : Attribute, ICorsPolicyProvider
    {
        private readonly CorsPolicy _policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyFromConfigAttribute"/> class.
        /// </summary>
        public CorsPolicyFromConfigAttribute()
        {
            // Create a CORS policy.
            _policy = new CorsPolicyFromConfig().CorsPolicy;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Cors.CorsPolicy" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="T:System.Web.Cors.CorsPolicy" />.
        /// </returns>
        [SecurityCritical]
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_policy);
        }
    }
}
