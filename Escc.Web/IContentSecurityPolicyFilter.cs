namespace Escc.Web
{
    /// <summary>
    /// Determines whether to apply a Content Security Policy 
    /// </summary>
    public interface IContentSecurityPolicyFilter
    {
        /// <summary>
        /// Determines whether to apply the Content Security Policy 
        /// </summary>
        /// <returns></returns>
        bool ApplyPolicy();
    }
}