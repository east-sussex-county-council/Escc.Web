namespace Escc.Web
{
    /// <summary>
    /// Read and update the <c>Content-Security-Policy</c> header of an HTTP response
    /// </summary>
    public interface IContentSecurityPolicyHeaders
    {
        /// <summary>
        /// Appends a Content Security Policy to the existing policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        ContentSecurityPolicyHeaders AppendPolicy(string policy);
        
        /// <summary>
        /// Appends a Content Security Policy to the existing policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        ContentSecurityPolicyHeaders AppendPolicy(ContentSecurityPolicy policy);

        /// <summary>
        /// Replaces the existing Content Security Policy with a new one
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        ContentSecurityPolicyHeaders ReplacePolicy(string policy);

        /// <summary>
        /// Replaces the existing Content Security Policy with a new one
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        ContentSecurityPolicyHeaders ReplacePolicy(ContentSecurityPolicy policy);

        /// <summary>
        /// Updates the HTTP response headers with the amended Content Security Policy.
        /// </summary>
        void UpdateHeaders();
    }
}