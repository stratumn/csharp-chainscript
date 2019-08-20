using System;
using Google.Protobuf;

namespace Stratumn.Chainscript
{

    /// <summary>
	/// Evidences can be used to externally verify a link's existence at a given
	/// moment in time.
	/// An evidence can be a proof of inclusion in a public blockchain, a timestamp
	/// signed by a trusted authority or anything that you trust to provide an
	/// immutable ordering of your process' steps.
	/// </summary>
    public class Evidence
    {
        /// <summary>
        /// Evidence version. Useful to correctly deserialize proof bytes. </summary>
        private string version;
        /// <summary>
        /// Identifier of the evidence type. </summary>
        private string backend;
        /// <summary>
        /// Instance of the backend used. </summary>
        private string provider;
        /// <summary>
        /// Serialized proof. </summary>
        private byte[] proof;

        /// <param name="version"> </param>
        /// <param name="backend"> </param>
        /// <param name="provider"> </param>
        /// <param name="proof"> </param>
        /// <exception cref="ChainscriptException">  </exception> 
        public Evidence(string version, string backend, string provider, byte[] proof)
        {
            this.version = version;
            this.backend = backend;
            this.provider = provider;
            this.proof = proof;
            this.Validate();
        }

        /// <summary>
        /// Validate that the evidence is well-formed.
        /// The proof is opaque bytes so it isn't validated here. </summary>
        /// <exception cref="ChainscriptException">  </exception>

        public virtual void Validate()
        {
            if (String.IsNullOrEmpty(this.version))
            {
                throw new ChainscriptException(Error.EvidenceVersionMissing);
            }

            if (String.IsNullOrEmpty(this.backend))
            {
                throw new ChainscriptException(Error.EvidenceBackendMissing);

            }

            if (String.IsNullOrEmpty(this.provider))
            {
                throw new ChainscriptException(Error.EvidenceProviderMissing);
            }

            if (this.proof == null || this.proof.Length == 0)
            {
                throw new ChainscriptException(Error.EvidenceProofMissing);
            }
        }

        /// <summary>
        /// Serialize the evidence.
        /// @returns evidence bytes.
        /// </summary>
        public byte[] Serialize()
        {
            return (new Stratumn.Chainscript.Proto.Evidence
            {
                Version = this.version,
                Backend = this.backend,
                Provider = this.provider,
                Proof = ByteString.CopyFrom(this.Proof)
            }
                    ).ToByteArray();
        }

        /// <returns> the version </returns>
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                this.version = value;
            }
        }


        /// <returns> the backend </returns>
        public string Backend
        {
            get
            {
                return backend;
            }
            set
            {
                this.backend = value;
            }
        }


        /// <returns> the provider </returns>
        public string Provider
        {
            get
            {
                return provider;
            }
            set
            {
                this.provider = value;
            }
        }


        /// <returns> the proof </returns>
        public byte[] Proof
        {
            get
            {
                return proof;
            }
            set
            {
                this.proof = value;
            }
        }



        /// <summary>
        /// Create an evidence from a protobuf object. </summary>
        /// <param name="e"> protobuf evidence. </param>
        /// <exception cref="ChainscriptException">  </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public static Evidence fromProto(stratumn.chainscript.Chainscript.Evidence object) throws ChainscriptException
        public static Evidence FromProto(Stratumn.Chainscript.Proto.Evidence @object)
        {

            string version = String.IsNullOrEmpty(@object.Version) ? "" : @object.Version;
            string backend = String.IsNullOrEmpty(@object.Backend) ? "" : @object.Backend;
            string provider = String.IsNullOrEmpty(@object.Provider) ? "" : @object.Provider;
            byte[] proof = @object.Proof != null ? (@object.Proof).ToByteArray() : new byte[0];

            return new Evidence(version, backend, provider, proof);
        }



        /// <summary>
        /// Deserialize an evidence. </summary>
        /// <param name="evidenceBytes"> encoded bytes. </param>
        /// <exception cref="ChainscriptException">  
        /// @returns the deserialized evidence. </exception>
        public static Evidence Deserialize(byte[] evidenceBytes)
        {
            Stratumn.Chainscript.Proto.Evidence pbEvidence;
            try
            {
                pbEvidence = Stratumn.Chainscript.Proto.Evidence.Parser.ParseFrom(evidenceBytes);
            }
            catch (InvalidProtocolBufferException e)
            {
                throw new ChainscriptException(e);
            }
            return FromProto(pbEvidence);
        }

    }

}

