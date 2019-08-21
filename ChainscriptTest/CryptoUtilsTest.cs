using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Utils;
using Org.BouncyCastle.Crypto.Parameters;

namespace Stratumn.Chainscript.ChainscriptTest
{
    [TestClass]
    public class CryptoUtilsTest
    {
       // static string publicKey = "-----BEGIN ED25519 PUBLIC KEY-----\nMCowBQYDK2VwAyEAewajeBYqSKxqcnJb209RSkH2CyaXgV3gotjq60DE4Is=\n-----END ED25519 PUBLIC KEY-----";
       // static string privateKey = "-----BEGIN ED25519 PRIVATE KEY-----\nMFACAQAwBwYDK2VwBQAEQgRACaNT4cup/ZQAq4IULZCrlPB7eR1QTCN9V3Qzct8S\nYp57BqN4FipIrGpyclvbT1FKQfYLJpeBXeCi2OrrQMTgiw==\n-----END ED25519 PRIVATE KEY-----\n";
        static string publicKey = "-----BEGIN ED25519 PUBLIC KEY-----\nMCowBQYDK2VwAyEAEIwjKUueKwu2s+ie5aFAsYBn8OEL7GHjEPML3JgxOEs=\n-----END ED25519 PUBLIC KEY-----";
        static string privateKey = "-----BEGIN ED25519 PRIVATE KEY-----\nMFACAQAwBwYDK2VwBQAEQgRAG4bBxUz5/UFzaCCxlhmpbKtZE313fsfY+hviGNRr\n5RYQjCMpS54rC7az6J7loUCxgGfw4QvsYeMQ8wvcmDE4Sw==\n-----END ED25519 PRIVATE KEY-----\n";
        string signature = "-----BEGIN MESSAGE-----\ncGEkdtv4MEZerv5pHS3fjDFk2ZX9vJwydFbQFUhcKsP/jp+6PueDcCokKU7CuxyB3F3QMJ0YfMxh7eg7MQmdBA==\n-----END MESSAGE-----\n";
        byte[] msg = Encoding.UTF8.GetBytes("This is a secret message");


        [TestMethod]
        public void TestVerify()
        {
            Ed25519PublicKeyParameters pub = CryptoUtils.DecodeEd25519PublicKey(publicKey);
            Assert.IsTrue(CryptoUtils.Verify(pub, msg, signature));
        }
 
        [TestMethod]
        public void TestSign()
        {
            try
            {
                Ed25519PrivateKeyParameters pri = CryptoUtils.DecodeEd25519PrivateKey(privateKey);
                String encodedSig = CryptoUtils.Sign(pri, msg);
                Assert.AreEqual(encodedSig.Trim(), signature.Trim(), "Encoded message does not match expected");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw e;
            }

        } 

        [TestMethod]
        public void TestGetPublicKeyFromPrivateKey()
        {
            try
            {
                Ed25519PrivateKeyParameters pri = CryptoUtils.DecodeEd25519PrivateKey(privateKey);
                Ed25519PublicKeyParameters pub = CryptoUtils.GetPublicKeyFromPrivateKey(pri);
                string publicPEM = CryptoUtils.EncodePublicKey(pub); 
                Assert.AreEqual(publicPEM, publicKey, "Extract public key from private key failed");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw e;
            }
        }


        [TestMethod]
        public void TestSignVerify()
        {
            try
            {
                Ed25519PrivateKeyParameters pri = CryptoUtils.DecodeEd25519PrivateKey(privateKey);
                Ed25519PublicKeyParameters pub = CryptoUtils.GetPublicKeyFromPrivateKey(pri);

                String encodedSig = CryptoUtils.Sign(pri, msg); 
                Assert.IsTrue(CryptoUtils.Verify(pub, msg, encodedSig),"Message Verification failed");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw e;
            }

        }
    }
}
