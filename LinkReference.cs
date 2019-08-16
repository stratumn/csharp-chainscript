using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript
{   /// <summary>
/// A reference to a link that can be in another process.
/// </summary>
    public class LinkReference
    {
        private byte[] linkHash;
        private string process;

        /// <param name="linkHash"> </param>
        /// <param name="process"> </param>
        /// <exception cref="ChainscriptException"> </exception>
        /// <exception cref="Exception">  </exception>
        public LinkReference(byte[] linkHash, string process)
        {
            if (String.IsNullOrEmpty(process))
            {
                throw new ChainscriptException(Error.LinkProcessMissing);
            }
            if (linkHash == null || linkHash.Length == 0)
            {
                throw new ChainscriptException(Error.LinkHashMissing);
            }

            this.linkHash = linkHash;
            this.process = process;
        }

        /// <returns> the linkHash </returns>
        public virtual byte[] LinkHash
        {
            get
            {
                return linkHash;
            }
            set
            {
                this.linkHash = value;
            }
        }


        /// <returns> the process </returns>
        public virtual string Process
        {
            get
            {
                return process;
            }
            set
            {
                this.process = value;
            }
        }

    }
}
