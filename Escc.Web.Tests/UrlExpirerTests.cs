using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Escc.Web.Tests
{
    [TestClass]
    public class UrlExpirerTests
    {
        [TestMethod]
        public void ValidUrlIsAllowed()
        {
            var url = new Uri("https://www.example.org/protect-me?id=1");
            var expirer = new UrlExpirer(new FakeProtector());

            url = expirer.ExpireUrl(url, new DateTime(2016,01,01,00,00,00));
            const int secondsInOneDay = 86400;
            var expired = expirer.HasUrlExpired(url, secondsInOneDay, new DateTime(2016, 01, 02, 00, 00, 00));

            Assert.IsFalse(expired);
        }

        [TestMethod]
        public void ExpiredUrlIsDisallowed()
        {
            var url = new Uri("https://www.example.org/protect-me?id=1");
            var expirer = new UrlExpirer(new FakeProtector());

            url = expirer.ExpireUrl(url, new DateTime(2016, 01, 01, 00, 00, 00));
            const int secondsInOneDay = 86400;
            var expired = expirer.HasUrlExpired(url, secondsInOneDay, new DateTime(2016, 01, 02, 00, 00, 01));

            Assert.IsTrue(expired);
        }
    }

    internal class FakeProtector : IUrlProtector
    {
        public bool CheckProtectedQueryString(Uri protectedUrl)
        {
            return true;
        }

        public Uri ProtectQueryString(Uri urlToProtect)
        {
            return urlToProtect;
        }
    }
}
