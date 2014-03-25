using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Net;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Ensures that the current request uses the correct protocol - either HTTP or HTTPS
    /// </summary>
    public class EnforceProtocolModule : IHttpModule
    {
        private List<string> errors = new List<string>();

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            this.errors.Clear();

            string folderName = GetCurrentFolder();

            if (String.IsNullOrEmpty(folderName)) folderName = "default";

            NameValueCollection securityConfig = ConfigurationManager.GetSection("EsccWebTeam.Data.Web/EnforceProtocolModule") as NameValueCollection;
            if (securityConfig == null) return;

            // If there's a specific setting for this folder
            if (securityConfig[folderName] != null)
            {
                this.errors.Add("using config setting for " + folderName + ", which is " + securityConfig[folderName].ToLowerInvariant());
                RedirectProtocol(securityConfig[folderName].ToLowerInvariant());
            }
            else
            {
                // Otherwise apply the default
                string defaultScheme = (securityConfig["default"] != null) ? securityConfig["default"].ToLowerInvariant() : Uri.UriSchemeHttp;
                this.errors.Add("using default scheme " + defaultScheme);
                RedirectProtocol(defaultScheme);
            }

        }

        private void ShowErrors()
        {
            if (HttpContext.Current.Request.QueryString["url"] != null)
            {
                string[] messages = new string[this.errors.Count];
                this.errors.CopyTo(messages);

                throw new HttpException(string.Join(Environment.NewLine, messages));
            }

        }

        /// <summary>
        /// Swops between HTTPS and HTTP if needed
        /// </summary>
        /// <param name="desiredProtocol">The desired protocol.</param>
        private void RedirectProtocol(string desiredProtocol)
        {
            // Really weird stuff happening here when coming via an ISA server

            // This gets the URL the user requested
            // string requestedUrl = HttpContext.Current.Request.Url.ToString();
            //
            // But running a requestedUrl.Substring(1) cuts a character off a completely
            // different URL; the one that ISA server requested from the web server
            // 
            // So code has to be very careful to keep working with the URL the user requested!

            this.errors.Add("Protocol should be " + desiredProtocol);


            this.errors.Add("User typed " + HttpContext.Current.Request.Url.ToString());
            this.errors.Add("Actual URL is " + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/" + HttpContext.Current.Request.Url.PathAndQuery);

            // Is it the right scheme?
            if (desiredProtocol != HttpContext.Current.Request.Url.Scheme)
            {
                this.errors.Add("Protocols don't match");
                string urlWithoutScheme = HttpContext.Current.Request.Url.ToString().Substring(HttpContext.Current.Request.Url.Scheme.Length);

                if (desiredProtocol == Uri.UriSchemeHttps)
                {
                    this.errors.Add("Redirect to https");
                    ShowErrors();
                    HttpContext.Current.Response.Redirect(Uri.UriSchemeHttps + urlWithoutScheme);
                    HttpContext.Current.Response.End();
                }
                else if (desiredProtocol == Uri.UriSchemeHttp)
                {
                    this.errors.Add("Redirect to http");
                    ShowErrors();
                    HttpContext.Current.Response.Redirect(Uri.UriSchemeHttp + urlWithoutScheme);
                    HttpContext.Current.Response.End();
                }

                this.errors.Add("No match, but no redirect - shouldn't get here");
                ShowErrors();
            }
            else
            {
                this.errors.Add("Protocols already match");
                ShowErrors();
            }
        }


        /// <summary>
        /// Gets normalised address of the current folder - lowercase, no filename, no surrounding slashes
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentFolder()
        {
            return HttpContext.Current.Request.Url.AbsolutePath.Substring(0, HttpContext.Current.Request.Url.AbsolutePath.Length - System.IO.Path.GetFileName(HttpContext.Current.Request.Url.AbsolutePath).Length).Trim('/').ToLower(CultureInfo.CurrentCulture);
        }
    }
}
