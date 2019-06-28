using Castle.Core.Logging;
using System.Globalization;

namespace Enterprises.Framework.BackgroundJobs
{
    /// <summary>
    /// Base class that can be used to implement <see cref="IBackgroundJob{TArgs}"/>.
    /// </summary>
    public abstract class BackgroundJob<TArgs> : IBackgroundJob<TArgs>
    {
        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { protected get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BackgroundJob()
        {
            Logger = NullLogger.Instance;
        }

        public abstract void Execute(TArgs args);

       
    }
}