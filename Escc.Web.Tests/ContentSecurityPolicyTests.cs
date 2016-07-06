using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Escc.Web.Tests
{
    [TestClass]
    public class ContentSecurityPolicyTests
    {
        [TestMethod]
        public void AmendedUpdateIgnoresDuplicates()
        {
            var policy = new ContentSecurityPolicy().AppendPolicy("img-src: https://www.example.org https://www.w3.org");

            policy.AppendPolicy("img-src: https://www.example.org https://example.org");

            Assert.AreEqual("img-src: https://www.example.org https://www.w3.org https://example.org", policy.ToString());
        }

        [TestMethod]
        public void ExistingHeaderIsUpdated()
        {
            var responseHeaders = new NameValueCollection
            {
                {"Content-Security-Policy", "img-src: https://www.example.org"}
            };

            new ContentSecurityPolicyHeaders(responseHeaders).AppendPolicy("img-src: https://example.org").UpdateHeaders();

            Assert.AreEqual("img-src: https://www.example.org https://example.org", responseHeaders["Content-Security-Policy"]);
        }

        [TestMethod]
        public void UrlIsIncluded()
        {
            var excludedUrls = new NameValueCollection()
            {
                {"None", "/example/excluded.html" }
            };

            var filter = new ContentSecurityPolicyUrlFilter(new Uri("https://www.example.org/example/included.html"), excludedUrls);

            Assert.IsTrue(filter.ApplyPolicy());
        }

        [TestMethod]
        public void UrlIsExcluded()
        {
            var excludedUrls = new NameValueCollection()
            {
                {"None", "/example/excluded.html" }
            };

            var filter = new ContentSecurityPolicyUrlFilter(new Uri("https://www.example.org/example/excluded.html"), excludedUrls);

            Assert.IsFalse(filter.ApplyPolicy());
        }
    }
}
