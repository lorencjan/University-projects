using System.Security.Authentication;
using NUnit.Framework;
using RockFests.DAL.StaticServices;

namespace RockFests.Specification.ServiceTests
{
    public class PasswordEncryptionTests : TestFixture
    {
        [TestCase("password")]
        [TestCase("verylonganddifficultpassword")]
        public void String_values_get_hashed(string str)
        {
            var hash = Encryption.HashPassword(str);
            Assert.That(!hash.ToLower().Contains(str.ToLower()));
            Assert.That(hash.Length == 44);
        }

        [TestCase("a")]
        [TestCase("password")]
        [TestCase("verylonganddifficultpassword")]
        public void Not_hashed_value_gets_successfully_verified_by_hash(string str)
        {
            var hashed = Encryption.HashPassword(str);
            Assert.DoesNotThrow(() => Encryption.VerifyPasswords(hashed, str));
        }

        [Test]
        public void Not_hashed_incorrect_value_gets_does_not_get_verified_by_hash()
        {
            const string toBeHashed = "hash";
            const string incorrect = "incorrect";

            var hashed = Encryption.HashPassword(toBeHashed);
            Assert.Throws<InvalidCredentialException>(() => Encryption.VerifyPasswords(hashed, incorrect));
        }
    }
}
