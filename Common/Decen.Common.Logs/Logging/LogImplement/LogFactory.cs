using Decen.Common.Logs.Logging.LogInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogImplement
{
    /// <summary>
    /// ILogContext接口的实现的工厂类。
    /// </summary>
    public static class LogFactory
    {
        public static ILogContext CreateContext()
        {
            return new LogContext();
        }
    }
}
