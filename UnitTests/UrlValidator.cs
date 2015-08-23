using NUnit.Framework;
using textsmile.net;

namespace UnitTests {
   [TestFixture]
   public class UrlValidator {
      [Test]
      public void Is_main_url_valid() {
         Assert.That(UrlHelper.IsUrlValid(UrlHelper.MAIN), Is.True);
      }

      [Test]
      public void Is_releases_url_valid() {
         Assert.That(UrlHelper.IsUrlValid(UrlHelper.RELEASES), Is.True);
      }

      [Test]
      public void Ir_readme_url_valid() {
         Assert.That(UrlHelper.IsUrlValid(UrlHelper.README), Is.True);
      }
   }
}