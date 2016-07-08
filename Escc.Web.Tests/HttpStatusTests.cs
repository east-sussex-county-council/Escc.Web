using System;
using System.IO;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Escc.Web.Tests
{
    [TestClass]
    public class HttpStatusTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Redirect303ToCurrentUrlThrowsException()
        {
            var response = new HttpResponse(new StringWriter());
            var url = new Uri("https://www.example.org");

            new HttpStatus().SeeOther(url, "GET", url, response);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Redirect301ToCurrentUrlThrowsException()
        {
            var response = new HttpResponse(new StringWriter());
            var url = new Uri("https://www.example.org");

            new HttpStatus().MovedPermanently(url, "GET", url, response);
        }
    }
}
