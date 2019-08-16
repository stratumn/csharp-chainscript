using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class CryptoUtils
    {
        private const string Ed25519 = "Ed25519";

        /// <summary>
        /// Encode the signature in base 64 with the begin and end message 
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>
        public static String EncodeSignature(byte[] sig)
        {
            return String.Format("-----BEGIN MESSAGE-----\n{0}\n-----END MESSAGE-----",
                  Convert.ToBase64String(sig));
        }

        /// <summary>
        /// Decode the signature 
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>

        public static byte[] DecodeSignature(String sig)
        {
            string s = sig.Replace("\n", "").Replace("-----BEGIN MESSAGE-----", "").Replace("-----END MESSAGE-----", "");
            return Convert.FromBase64String(s);
        }


        /// <summary>
        /// Signg the message and convert it ot base64
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Sign(Ed25519PrivateKeyParameters key, byte[] message)
        {
            ISigner signer = SignerUtilities.GetSigner(Ed25519);

            signer.Init(true, key);
            signer.BlockUpdate(message, 0, message.Length);
            var signature = signer.GenerateSignature();
            return CryptoUtils.EncodeSignature(signature);
        }


        /// <summary>
        /// Get the private key object from the raw key
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static Ed25519PrivateKeyParameters DecodeEd25519PrivateKey(string pem)
        {
            pem = pem.Replace("\n", "").Replace("-----BEGIN ED25519 PRIVATE KEY-----", "")
                .Replace("-----END ED25519 PRIVATE KEY-----", "");

            var privateKeyBase64 = Convert.FromBase64String(pem);
            byte[] seed = privateKeyBase64.Skip(17).Take(32).ToArray();   

            var keyParameters = new Ed25519PrivateKeyParameters(seed, 0);
            return (Ed25519PrivateKeyParameters)keyParameters;

        }


        /// <summary>
        /// Get the private key object from the raw key
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static Ed25519PrivateKeyParameters DecodeEd25519PrivateKey(byte[] keyBytes)
        { 
            var privateKey = (Ed25519PrivateKeyParameters)PrivateKeyFactory.CreateKey(keyBytes); 
            return privateKey; 
        }

        /// <summary>
        /// Get the private key object from the raw key
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static Ed25519PublicKeyParameters DecodeEd25519PublicKey(string pem)
        {
            pem = pem.Replace("\n", "").Replace("-----BEGIN ED25519 PUBLIC KEY-----", "")
                .Replace("-----END ED25519 PUBLIC KEY-----", "");

            var privateKeyBase64 = Convert.FromBase64String(pem);
            var publicKey = (Ed25519PublicKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(pem));
            return publicKey;

        }

        /// <summary>
        /// Todo :Need to be enhanced to return the correct public key but not used for now
        /// </summary>
        /// <param name="pem"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPublicKeyFromPrivateKey(Ed25519PrivateKeyParameters privateKey)
        {

            // get the public key from the private key 
            Ed25519PublicKeyParameters ed25519PublicKeyParam = privateKey.GeneratePublicKey();

            // convert it to X509
            var keyPair = new AsymmetricCipherKeyPair(ed25519PublicKeyParam, privateKey);
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);

            var serializedPublicBytes = publicKeyInfo.GetEncoded();
            var base64 = Convert.ToBase64String(serializedPublicBytes);
            base64 = string.Format("-----BEGIN ED25519 PUBLIC KEY-----\n{0}\n-----END ED25519 PUBLIC KEY-----", base64);
            return base64;

        }
        /// <summary>
        /// Verify the singature usign the public key and the raw message
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="encodedSig"></param>
        /// <returns></returns>
        public static bool Verify(Ed25519PublicKeyParameters key, byte[] message, string encodedSig)
        {
            byte[] sig = CryptoUtils.DecodeSignature(encodedSig);

            ISigner signer = SignerUtilities.GetSigner(Ed25519);

            signer.Init(false, key);
            signer.BlockUpdate(message, 0, message.Length);
            return signer.VerifySignature(sig);

        }
   


        /// <summary>
        /// provides a SHA256 hash of the the inputbytes
        /// </summary>
        public static byte[] Sha256(byte[] inputBytes)
        {
            Sha256Digest sha256 = new Sha256Digest();
            sha256.BlockUpdate(inputBytes, 0, inputBytes.Length);
            byte[] hash = new byte[sha256.GetDigestSize()];
            sha256.DoFinal(hash, 0);
            return hash;
        }

    }
}