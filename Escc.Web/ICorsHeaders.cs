namespace Escc.Web
{
    /// <summary>
    /// Cross-origin resource sharing (CORS) allows JavaScript on a web page to make XMLHttpRequests to another domain, not the domain the JavaScript originated from
    /// </summary>
    public interface ICorsHeaders
    {
        /// <summary>
        /// A cross-origin request will only work if the CORS headers are right, and we only want to allow specific origins
        /// </summary>
        void UpdateHeaders();
    }
}