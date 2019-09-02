
using DevLab.JmesPath;
using Google.Protobuf;
using Newtonsoft.Json;
using Stratumn.CanonicalJson;
using Stratumn.Chainscript;
using Stratumn.Chainscript.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Stratumn.Chainscript
{
    /// <summary>
    /// A link is the immutable part of a segment.
    /// A link contains all the data that represents a process' step.
    /// </summary>
    public class Link
    {
        public Stratumn.Chainscript.Proto.Link ALink { get; set; }

        /// <param name="link"> </param>
        public Link(Stratumn.Chainscript.Proto.Link link)
        {
            this.ALink = link;
        }

        /// <summary>
        /// A link is usually created as a result of an action. </summary>
        /// <exception cref="Exception"> 
        /// @returns the link's action. </exception>
        public string Action()
        {
            return this.LinkMeta.Action ?? "";
        }

        /// <summary>
        /// Add a signature to the link.
        /// This will validate the signature before adding it. </summary>
        /// <param name="signature"> link signature. </param>
        /// <exception cref="Exception">  </exception>
        public void AddSignature(Signature signature)
        {
            signature.Validate(this);

            Stratumn.Chainscript.Proto.Signature sig = new Stratumn.Chainscript.Proto.Signature()
            {
                Version = signature.Version(),
                PayloadPath = signature.PayloadPath(),
                PublicKey = ByteString.CopyFrom(signature.PublicKey()),
                Signature_ = ByteString.CopyFrom(signature.ByteSignature()),

            };

            this.ALink.Signatures.Add(sig);
        }

        /// <summary>
        /// The client id allows segment receivers to figure out how the segment was
        /// encoded and can be decoded. </summary>
        /// <exception cref="Exception"> 
        /// @returns the link's client id. </exception>
        public virtual string ClientId()
        {
            return LinkMeta.ClientId ?? "";
        }

        /// <summary>
        /// gets the Data
        /// </summary>
        /// <returns></returns>
        public object Data()
        {
            /// <summary>
            /// The link data (business logic details about the execution of a process step). </summary>
            /// <exception cref="Exception"> 
            /// @returns the object containing the link details. </exception>

            this.VerifyCompatibility();

            if (this.ALink.Data == null || this.ALink.Data.IsEmpty)
            {
                return null;
            }
            switch (this.Version())
            {
                case Constants.LINK_VERSION_1_0_0:
                    return Canonicalizer.Parse(this.ALink.Data.ToStringUtf8());
                default:
                    throw new ChainscriptException(Error.LinkVersionUnknown);
            }
        }

        /// <summary>
        ///  Set the given object as the link's data. 
        /// </summary>
        /// <param name="value"></param>
        public void SetData(object value)
        {
            this.VerifyCompatibility();

            switch (this.Version())
            {
                case Constants.LINK_VERSION_1_0_0:
                    try
                    {
                        string canonicalData = Canonicalizer.Stringify(value);
                        this.ALink.Data = ByteString.CopyFrom(Encoding.UTF8.GetBytes(canonicalData));
                        return;
                    }
                    catch (Exception e)
                    {
                        throw new ChainscriptException(e);
                    }
                default:
                    throw new ChainscriptException(Error.LinkVersionUnknown);
            }
        }

        /// <summary>
        /// Serialize the link and compute a hash of the resulting bytes.
        /// The serialization and hashing algorithm used depend on the link version. </summary>
        /// <exception cref="Exception"> 
        /// @returns the hash bytes. </exception>
        public byte[] Hash()
        {
            switch (this.Version())
            {
                case Constants.LINK_VERSION_1_0_0:

                    byte[] linkBytes = this.ALink.ToByteArray();
                    return CryptoUtils.Sha256(linkBytes);
                default:
                    throw new ChainscriptException(Error.LinkVersionUnknown);
            }
        }

        /// <summary>
        /// A link always belongs to a specific process map. </summary>
        /// <exception cref="Exception"> 
        /// @returns the link's map id. </exception>
        public string MapId()
        {

            Proto.LinkMeta meta = LinkMeta;
            return string.IsNullOrEmpty(meta.MapId) ? "" : meta.MapId;

        }


        /// <summary>
        /// Set the given object as the link's metadata. </summary>
        /// <param name="data"> custom data to save with the link metadata. </param>
        /// <exception cref="ChainscriptException"> </exception>
        /// <exception cref="Exception">  </exception> 
        /// 
        public object Metadata()
        {

            this.VerifyCompatibility();
            object result = null;
            ByteString linkMetadata = LinkMeta.Data;
            if (linkMetadata == null || linkMetadata.IsEmpty)
            {
                return result;
            }
            switch (this.Version())
            {
                case Constants.LINK_VERSION_1_0_0:
                    return Canonicalizer.Parse(linkMetadata.ToStringUtf8());

                default:
                    throw new ChainscriptException(Error.LinkVersionUnknown);
            }
        }


        public void SetMetadata(object value)
        {
            this.VerifyCompatibility();

            switch (this.Version())
            {
                case Constants.LINK_VERSION_1_0_0:
                    try
                    {
                        string canonicalData = Canonicalizer.Stringify(value);
                        LinkMeta.Data = ByteString.CopyFrom(Encoding.UTF8.GetBytes(canonicalData));

                        Proto.LinkMeta meta = LinkMeta;
                        this.ALink.Meta = meta;
                        return;
                    }
                    catch (Exception e)
                    {
                        throw new ChainscriptException(e);
                    }
                default:
                    throw new ChainscriptException(Error.LinkVersionUnknown);
            }

        }





        /// <summary>
        /// Maximum number of children a link is allowed to have.
        /// This is set to -1 if the link is allowed to have as many children as it
        /// wants. </summary>
        /// <exception cref="Exception"> 
        /// @returns the maximum number of children allowed. </exception>
        public int OutDegree()
        {
            return LinkMeta.OutDegree;
        }

        /// <summary>
        /// A link can have a parent, referenced by its link hash. </summary>
        /// <exception cref="Exception"> 
        /// @returns the parent link hash. </exception>
        public byte[] PrevLinkHash()
        {
            if (LinkMeta.PrevLinkHash == null)
            {
                return new byte[0];
            }
            return LinkMeta.PrevLinkHash.ToByteArray();
        }

        /// <summary>
        /// The priority can be used to order links. </summary>
        /// <exception cref="Exception"> 
        /// @returns the link's priority. </exception>
        public double Priority()
        {
            return LinkMeta.Priority;
        }

        /// <summary>
        /// A link always belong to an instance of a process. </summary>
        /// <exception cref="ChainscriptException"> 
        /// @returns the link's process name. </exception>
        public virtual Process Process()
        {
            Stratumn.Chainscript.Proto.Process process = LinkMeta.Process;
            if (process == null)
            {
                throw new ChainscriptException(Error.LinkProcessMissing);
            }
            return new Process(string.IsNullOrEmpty(process.Name) ? ""
                          : process.Name, string.IsNullOrEmpty(process.State) ? ""
                                   : process.State);
        }

        /// <summary>
        /// A link can contain references to other links. </summary>
        /// <exception cref="ChainscriptException"> 
        /// @returns referenced links. </exception>
        public LinkReference[] Refs()
        {
            IList<Stratumn.Chainscript.Proto.LinkReference> refList = LinkMeta.Refs;

            if (refList == null)
            {
                return new LinkReference[0];
            }

            IList<LinkReference> refListOut = new List<LinkReference>();
            foreach (Stratumn.Chainscript.Proto.LinkReference @ref in refList)
            {
                LinkReference linkReference;
                linkReference = new LinkReference(@ref.LinkHash != null ? @ref.LinkHash.ToByteArray() : new byte[0], @ref.Process ?? "");
                refListOut.Add(linkReference);
            }
            return refListOut.ToArray();
        }

        /// <summary>
        /// Create a segment from the link. </summary>
        /// <exception cref="Exception"> 
        /// @returns the segment wrapping the link. </exception>
        public virtual Segment Segmentify()
        {
            Stratumn.Chainscript.Proto.Segment segment = new Stratumn.Chainscript.Proto.Segment()
            {
                Link = this.ALink
            };
            return new Segment(segment);
        }

        /// <summary>
        /// Serialize the link.
        /// @returns link bytes.
        /// </summary>
        public virtual byte[] Serialize()
        {
            return this.ALink.ToByteArray();
        }




        /// <summary>
        /// Sign configurable parts of the link with the current signature version.
        /// The payloadPath is used to select what parts of the link need to be signed
        /// with the given private key. If no payloadPath is provided, the whole link
        /// is signed.
        /// The signature is added to the link's signature list. </summary>
        /// <param name="pemKey"> private key in PEM format (generated by @stratumn/js-crypto). </param>
        /// <param name="payloadPath"> link parts that should be signed. </param>
        /// <exception cref="Exception">  </exception>
        public void Sign(byte[] key, string payloadPath)
        {
            Signature signature = Signature.SignLink(key, this, payloadPath);

            Stratumn.Chainscript.Proto.Signature sig = new Stratumn.Chainscript.Proto.Signature()
            {
                Version = signature.Version(),
                PayloadPath = signature.PayloadPath(),
                PublicKey = ByteString.CopyFrom(signature.PublicKey()),
                Signature_ = ByteString.CopyFrom(signature.ByteSignature())
            };
            this.ALink.Signatures.Add(sig);
        }

        /// <summary>
        /// @returns the link's signatures (if any).
        /// </summary>
        public Signature[] Signatures()
        {
            Signature[] signatures = new Signature[this.ALink.Signatures.Count];
            for (int i = 0; i < this.ALink.Signatures.Count; i++)
            {
                Signature signature = new Signature(this.ALink.Signatures[i]);
                signatures[i] = signature;
            }
            return signatures;
        }

        /// <summary>
        /// Compute the bytes that should be signed. </summary>
        /// <exception cref="Exception"> 
        /// @argument version impacts how those bytes are computed.
        /// @argument payloadPath parts of the link that should be signed.
        /// @returns bytes to be signed. </exception>
        public byte[] SignedBytes(string version, string payloadPath)
        {
            byte[] hashedResultBytes = null;
            switch (version)
            {
                case Constants.SIGNATURE_VERSION_1_0_0:
                    if (string.IsNullOrEmpty(payloadPath))
                    {
                        payloadPath = "[version,data,meta]";
                    }

                    string input = null;
                    try
                    {

                        input = Google.Protobuf.JsonFormatter.ToDiagnosticString(this.ALink);
                        var jmes = new JmesPath();
                        string result = jmes.Transform(input, payloadPath);

                        string canonicalResult = Canonicalizer.Canonizalize(result.ToString());
                        byte[] payloadBytes = Encoding.UTF8.GetBytes(canonicalResult);
                        hashedResultBytes = CryptoUtils.Sha256(payloadBytes);
                    }
                    catch (IOException e1)
                    {
                        throw new ChainscriptException(e1);
                    }
                    break;
                default:
                    throw new ChainscriptException(Error.SignatureVersionUnknown);
            }
            return hashedResultBytes;
        }

        /// <summary>
        /// (Optional) A link can be interpreted as a step in a process. </summary>
        /// <exception cref="Exception"> 
        /// @returns the corresponding process step. </exception>
        public virtual string Step()
        {
            return string.IsNullOrEmpty(LinkMeta.Step) ? "" : LinkMeta.Step;
        }

        /// <summary>
        /// (Optional) A link can be tagged.
        /// Tags are useful to filter link search results. </summary>
        /// <exception cref="Exception"> 
        /// @returns link tags. </exception>
        public string[] Tags()
        {

            string[] result = LinkMeta.Tags != null ? LinkMeta.Tags.ToArray() : new string[0];

            return result;
        }

        /// <summary>
        /// Validate checks for errors in a link. </summary>
        /// <exception cref="ChainscriptException">  </exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(this.ALink.Version))
            {
                throw new ChainscriptException(Error.LinkVersionMissing);
            }

            Proto.LinkMeta meta = LinkMeta;

            if (string.IsNullOrEmpty(meta.MapId))
            {
                throw new ChainscriptException(Error.LinkMapIdMissing);
            }

            if (meta.Process == null || string.IsNullOrEmpty(meta.Process.Name))
            {
                throw new ChainscriptException(Error.LinkProcessMissing);
            }

            this.VerifyCompatibility();
            foreach (LinkReference @ref in this.Refs())
            {
                if (string.IsNullOrEmpty(@ref.Process))
                {
                    throw new ChainscriptException(Error.LinkProcessMissing);

                }
                if (@ref.LinkHash == null || @ref.LinkHash.Length == 0)
                {
                    throw new ChainscriptException(Error.LinkHashMissing);
                }
            }

            foreach (Signature sig in this.Signatures())
            {
                sig.Validate(this);
            }
        }

        /// <summary>
        /// The link version is used to properly serialize and deserialize it.
        /// @returns the link version.
        /// </summary>
        public virtual string Version()
        {
            return this.ALink.Version;
        }

        /// <summary>
        /// Check if the link is compatible with the current library.
        /// If not compatible, will throw an Error. </summary>
        /// <exception cref="Exception">  </exception>

        private void VerifyCompatibility()
        {

            if (string.IsNullOrEmpty(LinkMeta.ClientId))
            {
                throw new ChainscriptException(Error.LinkClientIdUnkown);
            }

            string[] compatibleClients = new string[] { Constants.ClientId, "github.com/stratumn/java-chainscript", "github.com/stratumn/go-chainscript", "github.com/stratumn/js-chainscript" };
            if (!compatibleClients.Contains(LinkMeta.ClientId))
            {
                throw new ChainscriptException(Error.LinkClientIdUnkown);
            }
        }

        /// <returns> the link </returns>
        public virtual Stratumn.Chainscript.Proto.Link GetLink()
        {
            return ALink;
        }


        /// <summary>
        ///*
        /// Validates Link MetaData before returning it.
        /// @return </summary>
        /// <exception cref="ChainscriptException"> </exception>
        private Proto.LinkMeta LinkMeta
        {
            get
            {
                if (this.ALink.Meta == null)
                {
                    throw new ChainscriptException(Error.LinkMetaMissing);
                }
                return this.ALink.Meta;
            }
        }

        /// <summary>
        /// Deserialize a link. </summary>
        /// <param name="linkBytes"> encoded bytes. </param>
        /// <exception cref="ChainscriptException"> 
        /// @returns the deserialized link. </exception>
        public static Link Deserialize(byte[] linkBytes)
        {
            try
            {
                return new Link(Stratumn.Chainscript.Proto.Link.Parser.ParseFrom(linkBytes));
            }
            catch (InvalidProtocolBufferException e)
            {
                throw new ChainscriptException(e);
            }
        }

        public String ToObject()
        {
            try
            {
                return JsonConvert.SerializeObject(this.ALink);
            }
            catch (Exception e)
            {
                throw new ChainscriptException(e);
            }
        }

   
        /// <summary>
        ///*
        /// Convert a   json object to a link. </summary>
        /// <param name="jsonObject">
        /// @return </param>
        public static Link FromObject(string jsonObject)
        {
            return new Link(JsonHelper.FromJson<Stratumn.Chainscript.Proto.Link>(jsonObject));
        }


    }


}


