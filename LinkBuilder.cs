using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript
{

    /// <summary>
    /// LinkBuilder makes it easy to create links that adhere to the ChainScript
    /// spec.
    /// It provides valid default values for required fields and allows the user
    /// to set fields to valid values.
    /// </summary>
    public class LinkBuilder : ILinkBuilder<LinkBuilder>
    {
        private Stratumn.Chainscript.Proto.Link link;
        private object linkData;
        private object linkMetadata;

        /// <exception cref="ChainscriptException"> 
        ///  </exception> 
        public LinkBuilder(string process, string mapId)
        {
            if (String.IsNullOrEmpty(process))
            {
                throw new ChainscriptException(Error.LinkProcessMissing);
            }

            if (String.IsNullOrEmpty(mapId))
            {
                throw new ChainscriptException(Error.LinkMapIdMissing);
            }

            Stratumn.Chainscript.Proto.Process processObj = new Stratumn.Chainscript.Proto.Process
            {
                Name = process
            };

            Stratumn.Chainscript.Proto.LinkMeta linkMeta = new Stratumn.Chainscript.Proto.LinkMeta
            {
                ClientId = Constants.ClientId,
                MapId = mapId,
                OutDegree = -1,
                Process = processObj

            };

            this.link = new Stratumn.Chainscript.Proto.Link
            {
                Version = Constants.LINK_VERSION,
                Meta = linkMeta
            };
        }

        public LinkBuilder WithAction(string action)
        {
            this.link.Meta.Action = action;
            return this;
        }

        public LinkBuilder WithData(object data)
        {
            this.linkData = data;
            return this;
        }

        public LinkBuilder WithDegree(int d)
        {
            this.link.Meta.OutDegree = d;
            return this;
        }

        public LinkBuilder WithMetadata(object data)
        {
            this.linkMetadata = data;
            return this;
        }


        public LinkBuilder WithParent(byte[] linkHash)
        {
            if (linkHash == null || linkHash.Length == 0)
            {
                throw new ChainscriptException(Error.LinkHashMissing);
            }
            this.link.Meta.PrevLinkHash = ByteString.CopyFrom(linkHash);
            return this;
        }

        public LinkBuilder WithPriority(double priority)
        {
            if (priority < 0)
            {
                throw new ChainscriptException(Error.LinkPriorityNotPositive);
            }
            this.link.Meta.Priority = priority;
            return this;
        }

        public LinkBuilder WithProcessState(string state)
        {
            this.link.Meta.Process.State = state;
            return this;
        }


        public LinkBuilder WithRefs(LinkReference[] refs)
        {
            if (refs != null)
            {

                for (int i = 0; i < refs.Length; i++)
                {
                    LinkReference @ref = refs[i];
                    if (String.IsNullOrEmpty(@ref.Process))
                    {
                        throw new ChainscriptException(Error.LinkProcessMissing);
                    }

                    if (@ref.LinkHash == null || @ref.LinkHash.Length == 0)
                    {
                        throw new ChainscriptException(Error.LinkHashMissing);
                    }
                    Stratumn.Chainscript.Proto.LinkReference reference = new Proto.LinkReference()
                    {
                        LinkHash = ByteString.CopyFrom(@ref.LinkHash),
                        Process = @ref.Process
                    };

                    this.link.Meta.Refs.Add(reference);
                }
            }
            return this;
        }

        public LinkBuilder WithStep(string step)
        { 
            this.link.Meta.Step = step;
            return this;
        }

        public LinkBuilder WithTags(string[] tags)
        { 
            List<string> listOfTags = tags.OfType<string>().ToList();
            List<string> filterTags = listOfTags.Where(tag => !String.IsNullOrEmpty(tag)).ToList();
            this.link.Meta.Tags.Add(filterTags); 
            return this;
        }
 
        public Link Build()
        {
            Link link = new Link(this.link);

            if (this.linkData != null)
            {
                link.Data = this.linkData;
            }
            if (this.linkMetadata != null)
            {
                link.Metadata = this.linkMetadata;
            }

            return link;
        }
    }

}
