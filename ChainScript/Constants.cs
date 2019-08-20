using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript
{
    /// 
    /// <summary>
    /// Common library constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// ClientID allows segment receivers to figure out how the segment was
        /// encoded and can be decoded.
        /// </summary>
        public const string ClientId = "github.com/stratumn/csharp-chainscript";

        /// <summary>
        /// LinkVersion_1_0_0 is the first version of the link encoding.
        /// In that version we encode interfaces (link.data and link.meta.data) with
        /// canonical JSON and hash the protobuf-encoded link bytes with SHA-256.
        /// </summary>
        public const string LINK_VERSION_1_0_0 = "1.0.0";
        /// <summary>
        /// The current link version. </summary>
        public const string LINK_VERSION = LINK_VERSION_1_0_0;

        /// <summary>
        /// SignatureVersion_1_0_0 is the first version of the link signature.
        /// In that version we use canonical JSON to encode the link parts.
        /// We use JMESPATH to select what parts of the link need to be signed.
        /// We use SHA-256 on the JSON-encoded bytes and sign the resulting hash.
        /// We use github.com/stratumn/js-crypto's 1.0.0 release to produce the
        /// signature (which uses PEM-encoded private keys).
        /// </summary>
        public const string SIGNATURE_VERSION_1_0_0 = "1.0.0";
        /// <summary>
        /// The current signature version. </summary>
        public const string SIGNATURE_VERSION = SIGNATURE_VERSION_1_0_0;
    }

}
