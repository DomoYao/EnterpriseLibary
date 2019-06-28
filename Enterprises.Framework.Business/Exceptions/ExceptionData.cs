using Enterprises.Framework.EventBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.Exceptions
{
    [Serializable]
    public class ExceptionData: EventData
    {
        /// <summary>
        /// Exception object.
        /// </summary>
        public Exception Exception { get; private set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">Exception object</param>
        public ExceptionData(Exception exception)
        {
            Exception = exception;
        }
    }
}