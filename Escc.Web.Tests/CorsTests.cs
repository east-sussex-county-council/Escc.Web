using System;
using System.Collections.Specialized;
using System.Web.Cors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Escc.Web.Tests
{
    [TestClass]
    public class CorsTests
    {
        [TestMethod]
        public void MatchingOriginAddsAllowedHeader()
        {
            var request = new NameValueCollection()
            {
                { "Origin", "https://example.org" }
            };

            var response = new NameValueCollection();

            var policy = new CorsPolicy();
            policy.Origins.Add("https://example.org");

            new Cors().ApplyPolicy(request, response, policy);

            Assert.IsTrue(response["Access-Control-Allow-Origin"] == "https://example.org");
        }

        [TestMethod]
        public void UnmatchedOriginDoesNotAddAllowedHeader()
        {
            var request = new NameValueCollection()
            {
                { "Origin", "https://example.org" }
            };

            var response = new NameValueCollection();

            var policy = new CorsPolicy();
            policy.Origins.Add("https://www.example.org"); // different domain

            new Cors().ApplyPolicy(request, response, policy);

            Assert.IsNull(response["Access-Control-Allow-Origin"]);
        }
    }
}
