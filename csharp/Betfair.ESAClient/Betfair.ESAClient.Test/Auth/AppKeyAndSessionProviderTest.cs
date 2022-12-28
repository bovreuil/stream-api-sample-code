using System.IO;
using System.Security.Authentication;
using NUnit.Framework;

namespace Betfair.ESAClient.Test.Auth {
    [TestFixture]
    public class AppKeyAndSessionProviderTest : BaseTest {
        [Test]
        public void TestValidSession() {
            var session = ValidSessionProvider.GetOrCreateNewSession();
            Assert.IsNotNull(session);
        }

        [Test]
        public void TestInvalidHost() {
            Assert.Catch<IOException>(() =>
                InvalidHostSessionProvider.GetOrCreateNewSession()
            );
        }

        [Test]
        public void TestInvalidLogin() {
            Assert.Catch<InvalidCredentialException>(() =>
                InvalidLoginSessionProvider.GetOrCreateNewSession()
            );
        }
    }
}
