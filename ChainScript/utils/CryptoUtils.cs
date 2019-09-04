using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters; 
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Linq;

namespace Utils
{
    public static class CryptoUtils
    {
        private const string Ed25519_ALGO = "Ed25519";

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
            ISigner signer = SignerUtilities.GetSigner(Ed25519_ALGO);

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
            TextReader textReader = new StringReader(pem);
            PemReader pemReader = new PemReader(textReader);
            var pemObject = pemReader.ReadPemObject();
            byte[] seed = pemObject.Content.Skip(18).Take(32).ToArray();
            var privateKey = DecodeEd25519PrivateKey(seed);
            return privateKey;
        }



        /// <summary>
        /// Get the private key object from the raw key
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static Ed25519PrivateKeyParameters DecodeEd25519PrivateKey(byte[] keyBytes)
        {
            Ed25519PrivateKeyParameters privateKey = new Ed25519PrivateKeyParameters(keyBytes, 0);
            return privateKey;
        }

        /// <summary>
        /// Get the private key object from the raw key
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        public static Ed25519PublicKeyParameters DecodeEd25519PublicKey(string pem)
        {
            TextReader textReader = new StringReader(pem);
            PemReader pemReader = new PemReader(textReader);
            var pemObject = pemReader.ReadPemObject();
            var keyParameters = new Ed25519PublicKeyParameters(pemObject.Content, 0);
            var publicKey = DecodeEd25519PublicKey(pemObject.Content);
           
            return publicKey;
        }

        public static Ed25519PublicKeyParameters DecodeEd25519PublicKey(byte[] keyBytes)
        {
            var publicKey = (Ed25519PublicKeyParameters)PublicKeyFactory.CreateKey(keyBytes);
            return publicKey;
        }

        /// <summary>
        /// Generates a KeyPair 
        /// </summary>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair GeneratePrivateKey()
        {
            Ed25519KeyPairGenerator d = new Ed25519KeyPairGenerator();
            d.Init(new Ed25519KeyGenerationParameters(SecureRandom.GetInstance(Ed25519_ALGO)));

            AsymmetricCipherKeyPair keyPair = d.GenerateKeyPair();
            return keyPair;
        }


        /// <summary>
        /// Todo :Need to be enhanced to return the correct public key but not used for now
        /// </summary>
        /// <param name="pem"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Ed25519PublicKeyParameters GetPublicKeyFromPrivateKey(Ed25519PrivateKeyParameters privateKey)
        {
            // get the public key from the private key 
            Ed25519PublicKeyParameters publicKey = privateKey.GeneratePublicKey();
            return publicKey;

        }


        /// <summary>
        /// Encodes private key to PEM formatted string
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string EncodePrivateKey(Ed25519PrivateKeyParameters privateKey)
        {
          
            var serializedPublicBytes = privateKey.GetEncoded();
            var base64 = Convert.ToBase64String(serializedPublicBytes);
            base64 = string.Format("-----BEGIN ED25519 PRIVATE KEY-----\n{0}\n-----END ED25519 PRIVATE KEY-----\n", base64);
            return base64;
        }

        /// <summary>
        /// Encodes public key to PEM formatted string
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string EncodePublicKey(Ed25519PublicKeyParameters publicKey)
        {
            // convert it to X509  
            string base64 = Convert.ToBase64String(EncodePublicKeyX509(publicKey)) ;
            base64 = string.Format("-----BEGIN ED25519 PUBLIC KEY-----\n{0}\n-----END ED25519 PUBLIC KEY-----\n", base64);
            return base64;
        }

        /// <summary>
        /// Hashes the Public key with X509
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static byte[] EncodePublicKeyX509(Ed25519PublicKeyParameters publicKey)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            var serializedPublicBytes = publicKeyInfo.GetEncoded();

            return serializedPublicBytes;
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

            ISigner signer = SignerUtilities.GetSigner(Ed25519_ALGO);

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