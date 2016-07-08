using System;

namespace Escc.Web
{
    /// <summary>
    /// A Content Security Policy restricts which resources a web page can load, protecting against risks such as cross-site scripting.
    /// </summary>
    public interface IContentSecurityPolicy
    {
        /// <summary>
        /// Appends a new Content Security Policy to the existing policy, and returns the updated policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        ContentSecurityPolicy AppendPolicy(ContentSecurityPolicy policy);

        /// <summary>
        /// Appends a new Content Security Policy to the existing policy, and returns the updated policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        ContentSecurityPolicy AppendPolicy(string policy);
    }
}