using System.Collections.Generic;

namespace Stratumn.Chainscript
{

    /// <summary>
    ///*
    /// Error codes thrown by the library
    /// </summary>
    public sealed class Error
    {

        /// <summary>
        ///*** Evidence errors **** </summary>
        public static readonly Error EvidenceVersionMissing = new Error("EvidenceVersionMissing", InnerEnum.EvidenceVersionMissing, "evidence version is missing");
        public static readonly Error EvidenceBackendMissing = new Error("EvidenceBackendMissing", InnerEnum.EvidenceBackendMissing, "evidence backend is missing");
        public static readonly Error EvidenceProviderMissing = new Error("EvidenceProviderMissing", InnerEnum.EvidenceProviderMissing, "evidence provider is missing");
        public static readonly Error EvidenceProofMissing = new Error("EvidenceProofMissing", InnerEnum.EvidenceProofMissing, "evidence proof is missing");
        public static readonly Error DuplicateEvidence = new Error("DuplicateEvidence", InnerEnum.DuplicateEvidence, "evidence already exists for the given backend and provider");

        /// <summary>
        ///*** Link errors **** </summary>

        public static readonly Error LinkClientIdUnkown = new Error("LinkClientIdUnkown", InnerEnum.LinkClientIdUnkown, "link was created with an unknown client: can't deserialize it");
        public static readonly Error LinkHashMissing = new Error("LinkHashMissing", InnerEnum.LinkHashMissing, "link hash is missing");
        public static readonly Error LinkMapIdMissing = new Error("LinkMapIdMissing", InnerEnum.LinkMapIdMissing, "link map id is missing");
        public static readonly Error LinkMetaMissing = new Error("LinkMetaMissing", InnerEnum.LinkMetaMissing, "link meta is missing");
        public static readonly Error LinkProcessMissing = new Error("LinkProcessMissing", InnerEnum.LinkProcessMissing, "link process is missing");
        public static readonly Error LinkVersionMissing = new Error("LinkVersionMissing", InnerEnum.LinkVersionMissing, "link vern is missing");
        public static readonly Error LinkVersionUnknown = new Error("LinkVersionUnknown", InnerEnum.LinkVersionUnknown, "unknown link version");
        public static readonly Error LinkPriorityNotPositive = new Error("LinkPriorityNotPositive", InnerEnum.LinkPriorityNotPositive, "priority needs to be positive");
        /// <summary>
        ///*** Segment errors **** </summary>

        public static readonly Error LinkHashMismatch = new Error("LinkHashMismatch", InnerEnum.LinkHashMismatch, "link hash mismatch");
        public static readonly Error LinkMissing = new Error("LinkMissing", InnerEnum.LinkMissing, "link is missing");
        public static readonly Error SegmentMetaMissing = new Error("SegmentMetaMissing", InnerEnum.SegmentMetaMissing, "segment meta is missing");

        /// <summary>
        ///*** Signature errors **** </summary>

        public static readonly Error SignatureInvalid = new Error("SignatureInvalid", InnerEnum.SignatureInvalid, "signature is invalid");
        public static readonly Error SignatureMissing = new Error("SignatureMissing", InnerEnum.SignatureMissing, "signature bytes are missing");
        public static readonly Error SignaturePublicKeyMissing = new Error("SignaturePublicKeyMissing", InnerEnum.SignaturePublicKeyMissing, "signature public key is missing");
        public static readonly Error SignatureVersionUnknown = new Error("SignatureVersionUnknown", InnerEnum.SignatureVersionUnknown, "unknown signature version");

        public static readonly Error InternalError = new Error("InternalError", InnerEnum.InternalError, "Internal Chainscript Error");

        private static readonly IList<Error> valueList = new List<Error>();

        static Error()
        {
            valueList.Add(EvidenceVersionMissing);
            valueList.Add(EvidenceBackendMissing);
            valueList.Add(EvidenceProviderMissing);
            valueList.Add(EvidenceProofMissing);
            valueList.Add(DuplicateEvidence);
            valueList.Add(LinkClientIdUnkown);
            valueList.Add(LinkHashMissing);
            valueList.Add(LinkMapIdMissing);
            valueList.Add(LinkMetaMissing);
            valueList.Add(LinkProcessMissing);
            valueList.Add(LinkVersionMissing);
            valueList.Add(LinkVersionUnknown);
            valueList.Add(LinkPriorityNotPositive);
            valueList.Add(LinkHashMismatch);
            valueList.Add(LinkMissing);
            valueList.Add(SegmentMetaMissing);
            valueList.Add(SignatureInvalid);
            valueList.Add(SignatureMissing);
            valueList.Add(SignaturePublicKeyMissing);
            valueList.Add(SignatureVersionUnknown);
            valueList.Add(InternalError);
        }

        public enum InnerEnum
        {
            EvidenceVersionMissing,
            EvidenceBackendMissing,
            EvidenceProviderMissing,
            EvidenceProofMissing,
            DuplicateEvidence,
            LinkClientIdUnkown,
            LinkHashMissing,
            LinkMapIdMissing,
            LinkMetaMissing,
            LinkProcessMissing,
            LinkVersionMissing,
            LinkVersionUnknown,
            LinkPriorityNotPositive,
            LinkHashMismatch,
            LinkMissing,
            SegmentMetaMissing,
            SignatureInvalid,
            SignatureMissing,
            SignaturePublicKeyMissing,
            SignatureVersionUnknown,
            InternalError
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;

        private string description;

        internal Error(string name, InnerEnum innerEnum, string description)
        {
            this.description = description;

            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        public override string ToString()
        {
            return this.description;
        }


        public static IList<Error> values()
        {
            return valueList;
        }

        public int ordinal()
        {
            return ordinalValue;
        }

        public static Error valueOf(string name)
        {
            foreach (Error enumInstance in Error.valueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }
}
