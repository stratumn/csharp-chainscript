using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript
{
    /// <summary>
    /// A process is a collection of maps (process instances).
    /// A map is a collection of links that track the process' progress.
    /// </summary>
    public class Process
    {
        private string name;
        private string state;

        /// <param name="name"> </param>
        /// <param name="state"> </param>
        public Process(string name, string state) : base()
        {
            this.name = name;
            this.state = state != null ? state : "";
        }

        /// <returns> the name </returns>
        public string Name
        {
            get
            {
                return name;
            }
        }


        /// <returns> the state </returns>
        public string State
        {
            get
            {
                return state;
            }
        }

    }
}
