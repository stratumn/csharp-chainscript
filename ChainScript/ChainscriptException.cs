using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript
{
    /// <summary>
    ///*
    ///  Custom exception to hold the internal error messages 
    /// 
    /// </summary>
    public class ChainscriptException : Exception
    {

        /// 
        private const long serialVersionUID = 1L;

        Error error = Error.InternalError;
        public ChainscriptException() : base()
        {
        }

        
        public ChainscriptException(Error error) : base(error.ToString())
        {

            this.error = error;
        }
        

        public ChainscriptException(string message) : base(message)
        {
        }

        
        public ChainscriptException(Exception cause) : base(cause.ToString())
        {
        }
        

        public ChainscriptException(string message, Exception cause) : base(message, cause)
        {
        }

        public Error Error
        {
            get
            {
                return error;
            }
            set
            {
                this.error = value;
            }
        }




    }

}
