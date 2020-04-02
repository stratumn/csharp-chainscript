using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratumn.Chainscript
{
    /// <summary>
    ///    A segment describes an atomic step in your process. 
    /// </summary>
    public class Segment
    {

        private Stratumn.Chainscript.Proto.Link pbLink;
        private Stratumn.Chainscript.Proto.Segment pbSegment;

        public Segment(Stratumn.Chainscript.Proto.Segment pbSegment)
        {
            if (pbSegment.Link == null)
            {
                throw new ChainscriptException(Error.LinkMissing);
            }

            this.pbLink = pbSegment.Link;
            this.pbSegment = pbSegment;

            if (pbSegment.Meta == null)
                this.pbSegment.Meta = new Stratumn.Chainscript.Proto.SegmentMeta();

            Link link = new Link(this.pbLink);
            this.pbSegment.Meta.LinkHash = ByteString.CopyFrom(link.Hash());

        }

        /// <summary>
        /// The segment can be enriched with evidence that the link was saved
        /// immutably somewhere. </summary>
        /// <param name="e"> evidence. </param>
        /// <exception cref="ChainscriptException">  </exception> 
        public virtual void AddEvidence(Evidence e)
        {
            e.Validate();

            if (this.GetEvidence(e.Backend, e.Provider) != null)
            {
                throw new ChainscriptException(Error.DuplicateEvidence);
            }

            Stratumn.Chainscript.Proto.Evidence pbEvidence = (new Stratumn.Chainscript.Proto.Evidence());
            pbEvidence.Version = e.Version;
            pbEvidence.Provider = e.Provider;
            pbEvidence.Backend = e.Backend;
            pbEvidence.Proof = ByteString.CopyFrom(e.Proof);
            this.pbSegment.Meta.Evidences.Add(pbEvidence);
        }

        /// <summary>
        /// Return all the evidences in this segment. </summary>
        /// <exception cref="Exception"> 
        /// @returns evidences. </exception> 
        public virtual Evidence[] Evidences()
        {
            IList<Stratumn.Chainscript.Proto.Evidence> evidences = this.pbSegment.Meta.Evidences;
            IList<Evidence> result = new List<Evidence>();
            foreach (Stratumn.Chainscript.Proto.Evidence evidence in evidences)
            {
                result.Add(Evidence.FromProto(evidence));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Return all the evidences of a specific backend. </summary>
        /// <param name="backend"> of the expected evidences. </param>
        /// <exception cref="Exception"> 
        /// @returns evidences. </exception> 
        public virtual Evidence[] FindEvidences(string backend)
        {
            IList<Stratumn.Chainscript.Proto.Evidence> evidences = this.pbSegment.Meta.Evidences;
            IList<Evidence> result = new List<Evidence>();
            foreach (Stratumn.Chainscript.Proto.Evidence evidence in evidences)
            {
                if (evidence.Backend.Equals(backend))
                {
                    result.Add(Evidence.FromProto(evidence));
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Retrieve the evidence for the given backend and provider (if one exists). </summary>
        /// <param name="backend"> evidence backend. </param>
        /// <param name="provider"> evidence backend instance. </param>
        /// <exception cref="ChainscriptException"> 
        /// @returns the evidence or null. </exception> 
        public virtual Evidence GetEvidence(string backend, string provider)
        {
            IList<Stratumn.Chainscript.Proto.Evidence> evidences = this.pbSegment.Meta.Evidences;
            foreach (Stratumn.Chainscript.Proto.Evidence evidence in evidences)
            {
                if (evidence.Backend.Equals(backend) && evidence.Provider.Equals(provider))
                {
                    return Evidence.FromProto(evidence);
                }
            }
            return null;
        }

        /// <summary>
        /// The segment's link is its immutable part.
        /// @returns the segment's link.
        /// </summary>
        public virtual Link Link()
        {
            return new Link(this.pbLink);
        }

        /// <summary>
        /// Get the hash of the segment's link.
        /// @returns the link's hash.
        /// </summary>
        public virtual byte[] LinkHash()
        {
            return (this.pbSegment.Meta.LinkHash.ToByteArray());
        }

        /// <summary>
        /// Serialize the segment.
        /// @returns segment bytes.
        /// </summary>
        public virtual byte[] Serialize()
        {
            return pbSegment.ToByteArray();
        }

        /// <summary>
        /// Validate checks for errors in a segment. </summary>
        /// <exception cref="ChainscriptException">  </exception> 
        public virtual void Validate()
        {
            if (this.pbSegment.Meta == null)
            {
                throw new ChainscriptException(Error.SegmentMetaMissing);
            }
            if (this.LinkHash() == null || this.LinkHash().Length == 0)
            {
                throw new ChainscriptException(Error.LinkHashMissing);
            }
            if (!Convert.ToBase64String(this.LinkHash()).Equals(Convert.ToBase64String(this.Link().Hash())))
            {
                throw new ChainscriptException(Error.LinkHashMismatch);
            }

            this.Link().Validate();
        }


        /// <summary>
        /// Deserialize a segment. </summary>
        /// <param name="segmentBytes"> encoded bytes. </param>
        /// <exception cref="Exception"> 
        /// @returns the deserialized segment. </exception> 
        public static Segment Deserialize(byte[] segmentBytes)
        {

            Stratumn.Chainscript.Proto.Segment segment = Stratumn.Chainscript.Proto.Segment.Parser.ParseFrom(segmentBytes);
            return new Segment(segment);
        }


    }
}
