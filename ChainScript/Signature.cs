﻿
using Google.Protobuf;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Stratumn.Chainscript
{
    /// <summary>
    /// A signature of configurable parts of a link.
    /// Different signature types and versions are allowed to sign different
    /// encodings of the data, but we recommend signing a hash of the
    /// protobuf-encoded bytes.
    /// </summary>
    public class Signature
    {
       
        private Stratumn.Chainscript.Proto.Signature signature;

        /// <param name="s"> </param>
        public Signature(Stratumn.Chainscript.Proto.Signature signature)
        {
            this.signature = signature;
        }

     
        /// <summary>
        /// @returns the version of the signature scheme.
        /// </summary>
        public string Version()
        {
            return String.IsNullOrEmpty(this.signature.Version) ? "" : this.signature.Version;
        }

        /// <summary>
        /// @returns the algorithm used (for example, "EdDSA").
        /// </summary>
        public string Type()
        {
            return String.IsNullOrEmpty(this.signature.Type) ? "" : this.signature.Type;
        }

        /// <summary>
        /// @returns a description of the parts of the link that are signed.
        /// </summary>
        public string PayloadPath()
        {
            return String.IsNullOrEmpty(this.signature.PayloadPath) ? "" : this.signature.PayloadPath;
        }

        /// <summary>
        /// @returns the public key of the signer.
        /// </summary>
        public byte[] PublicKey()
        {
            return (this.signature.PublicKey != null ? this.signature.PublicKey : ByteString.Empty).ToByteArray();
        }

        /// <summary>
        /// @returns the signature bytes.
        /// </summary>
        public byte[] ByteSignature()
        {
            return (this.signature.Signature_ != null ? this.signature.Signature_ : ByteString.Empty).ToByteArray();
        }

        /// <summary>
        /// Validate the signature and throw an exception if invalid. </summary>
        /// <param name="link"> the link signed. </param>
        /// <exception cref="ChainscriptException">  </exception> 
        public virtual void Validate(Link link)
        {
            if (this.PublicKey() == null || this.PublicKey().Length == 0)
            {
                throw new ChainscriptException(Error.SignaturePublicKeyMissing);
            }

            if (this.ByteSignature() == null || this.ByteSignature().Length == 0)
            {
                throw new ChainscriptException(Error.SignatureMissing);
            }

            switch (this.Version())
            {
                case Constants.SIGNATURE_VERSION_1_0_0:
                    byte[] signed = link.SignedBytes(this.Version(), this.PayloadPath());
                     
                    try
                    {
                        var publicKey = CryptoUtils.DecodeEd25519PublicKey(this.PublicKey());
                        if (!CryptoUtils.Verify(publicKey, signed, Encoding.UTF8.GetString(this.ByteSignature())))
                        {
                            throw new ChainscriptException(Error.SignatureInvalid);
                        }
                    }
                    catch (Exception e)  
                    {
                        throw new ChainscriptException(e);
                    }
                    return;
                default:
                    throw new ChainscriptException(Error.SignatureVersionUnknown);
            }
        }


        /// <summary>
        /// Sign bytes with the current signature version. </summary>
        /// <param name="pemKey"> private key in PEM format (generated by @stratumn/js-crypto). </param>
        /// <param name="toSign"> bytes that should be signed. </param>
        /// <exception cref="Exception">  </exception>
        public static Signature Sign(byte[] key, byte[] toSign)
        {
            string signedMessage;
            Ed25519PublicKeyParameters publicKey  ;
            try
            {

               var privateKey = CryptoUtils.DecodeEd25519PrivateKey(key);
                signedMessage = CryptoUtils.Sign(privateKey, toSign);
                publicKey = CryptoUtils.GetPublicKeyFromPrivateKey(privateKey);
                 
            }
            catch (Exception e)  
            {
                throw new ChainscriptException("Could not create the private key / signature", e);
            }

            Stratumn.Chainscript.Proto.Signature sig = new Stratumn.Chainscript.Proto.Signature
            {
                Version = Constants.SIGNATURE_VERSION,
                Signature_ = ByteString.CopyFrom(Encoding.UTF8.GetBytes(signedMessage)),
                PublicKey = ByteString.CopyFrom(CryptoUtils.EncodePublicKeyX509(publicKey)),
            };
            return new Signature(sig);
        }

        /// <summary>
        /// Sign configurable parts of the given link with the current signature
        /// version.
        /// The payloadPath is used to select what parts of the link need to be signed
        /// with the given private key. If no payloadPath is provided, the whole link
        /// is signed. </summary>
        /// <param name="pemKey"> private key in PEM format (generated by @stratumn/js-crypto). </param>
        /// <param name="link"> that should be signed. </param>
        /// <param name="payloadPath"> link parts that should be signed. </param>
        /// <exception cref="Exception">  </exception>
        public static Signature SignLink(byte[] key, Link link, string payloadPath)
        {
            // We want to make it explicit when we're signing the whole link.
            if (String.IsNullOrEmpty(payloadPath))
            {
                payloadPath = "[version,data,meta]";
            }
            byte[] toSign = link.SignedBytes(Constants.SIGNATURE_VERSION, payloadPath);
            Signature signature = Sign(key, toSign);

            Stratumn.Chainscript.Proto.Signature sig = new Stratumn.Chainscript.Proto.Signature
            {
                Version = signature.Version(),
                PayloadPath = payloadPath,
                PublicKey = ByteString.CopyFrom(signature.PublicKey()),
                Signature_ = ByteString.CopyFrom(signature.ByteSignature())

            };
            return new Signature(sig);
        }

    }

}
