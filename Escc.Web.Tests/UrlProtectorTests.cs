using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Escc.Web.Tests
{
    [TestClass]
    public class UrlProtectorTests
    {
        [TestMethod]
        public void UnalteredUrlIsAllowed()
        {
            var url = new Uri("https://www.example.org/protect-me?id=1");
            var salt = Guid.NewGuid().ToString();
            var protector = new UrlProtector(salt);

            url = protector.ProtectQueryString(url);

            Assert.IsTrue(protector.CheckProtectedQueryString(url));
        }

        [TestMethod]
        public void AlteredUrlIsDisallowed()
        {
            var url = new Uri("https://www.example.org/protect-me?id=1");
            var salt = Guid.NewGuid().ToString();
            var protector = new UrlProtector(salt);

            url = protector.ProtectQueryString(url);
            url = new Uri(url + "tampered-value");

            Assert.IsFalse(protector.CheckProtectedQueryString(url));
        }
    }
}
