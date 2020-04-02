using System;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Xunit;
using Utils;

namespace Stratumn.Chainscript.ChainscriptTest
{
    public class CryptoUtilsTest
    {
        static string publicKey = "-----BEGIN ED25519 PUBLIC KEY-----\nMCowBQYDK2VwAyEAEIwjKUueKwu2s+ie5aFAsYBn8OEL7GHjEPML3JgxOEs=\n-----END ED25519 PUBLIC KEY-----\n";
        static string privateKey = "-----BEGIN ED25519 PRIVATE KEY-----\nMFACAQAwBwYDK2VwBQAEQgRAG4bBxUz5/UFzaCCxlhmpbKtZE313fsfY+hviGNRr\n5RYQjCMpS54rC7az6J7loUCxgGfw4QvsYeMQ8wvcmDE4Sw==\n-----END ED25519 PRIVATE KEY-----\n";
        string signature = "-----BEGIN MESSAGE-----\ncGEkdtv4MEZerv5pHS3fjDFk2ZX9vJwydFbQFUhcKsP/jp+6PueDcCokKU7CuxyB3F3QMJ0YfMxh7eg7MQmdBA==\n-----END MESSAGE-----\n";
        byte[] msg = Encoding.UTF8.GetBytes("This is a secret message");


        [Fact]
        public void TestVerify()
        {
            Ed25519PublicKeyParameters pub = CryptoUtils.DecodeEd25519PublicKey(publicKey);
            Assert.True(CryptoUtils.Verify(pub, msg, signature));
        }

        [Fact]
        public void TestSign()
        {
            Ed25519PrivateKeyParameters pri = CryptoUtils.DecodeEd25519PrivateKey(privateKey);
            String encodedSig = CryptoUtils.Sign(pri, msg);
            Assert.Equal(encodedSig.Trim(), signature.Trim());
        } 

        [Fact]
        public void TestGetPublicKeyFromPrivateKey()
        {
            Ed25519PrivateKeyParameters pri = CryptoUtils.DecodeEd25519PrivateKey(privateKey);
            Ed25519PublicKeyParameters pub = CryptoUtils.GetPublicKeyFromPrivateKey(pri);
            string publicPEM = CryptoUtils.EncodePublicKey(pub); 
            Assert.Equal(publicPEM, publicKey);
        }


        [Fact]
        public void TestSignVerify()
        {
            Ed25519PrivateKeyParameters pri = CryptoUtils.DecodeEd25519PrivateKey(privateKey);
            Ed25519PublicKeyParameters pub = CryptoUtils.GetPublicKeyFromPrivateKey(pri);
            String encodedSig = CryptoUtils.Sign(pri, msg); 
            Assert.True(CryptoUtils.Verify(pub, msg, encodedSig));
        }
    }
}
