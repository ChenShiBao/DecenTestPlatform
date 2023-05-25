using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Enums
{
    public enum LogEventType
    {
        /// <summary>
        ///     Recoverable error.
        /// </summary>
        Error = 10,
        /// <summary>
        ///     Noncritical problem.
        /// </summary>
        Warning = 20,
        /// <summary>
        ///     Informational message.
        /// </summary>
        Information = 30,
        /// <summary>
        ///     Debugging trace.
        /// </summary>
        Debug = 40
    }
}
