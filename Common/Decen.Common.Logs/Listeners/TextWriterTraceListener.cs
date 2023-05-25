using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Decen.Common.Logs.Listeners
{
    /// <summary>
    ///将数据输出到TextWriter的简单TraceListener。
    /// </summary>
    public class TextWriterTraceListener : TraceListener, IDisposable
    {
        private TextWriter writer;

        private Mutex LockObject = new Mutex(false);

        private void LockOutput()
        {
            LockObject.WaitOne();
        }

        private void UnlockOutput()
        {
            LockObject.ReleaseMutex();
        }

        /// <summary>
        /// The writer that is used as the output.
        /// </summary>
        public TextWriter Writer
        {
            get
            {
                return writer;
            }
            set
            {
                if (writer != value)
                {
                    LockOutput();
                    try
                    {
                        writer = value;
                    }
                    finally
                    {
                        UnlockOutput();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new TextWriterTraceListener writing to the given filename.
        /// </summary>
        public TextWriterTraceListener(string filename)
            : this(new FileStream(filename, FileMode.Append))
        {
        }

        /// <summary>
        /// Creates a new TextWriterTraceListener writing to the given stream.
        /// </summary>
        public TextWriterTraceListener(Stream stream)
        {
            Writer = new StreamWriter(stream);
        }

        /// <summary>
        /// Writes a string to the current Writer.
        /// </summary>
        public override void Write(string message)
        {
            LockOutput();
            try
            {
                Writer.Write(message);
            }
            finally
            {
                UnlockOutput();
            }
        }

        /// <summary>
        /// Writes a string including a newline to the current Writer.
        /// </summary>
        public override void WriteLine(string message)
        {
            LockOutput();
            try
            {
                Writer.WriteLine(message);
            }
            finally
            {
                UnlockOutput();
            }
        }

        /// <summary>
        /// Flushes the log system and the current Writer.
        /// </summary>
        public override void Flush()
        {
            base.Flush();
            LockOutput();
            try
            {
                if (writer != null)
                    writer.Flush();
            }
            finally
            {
                UnlockOutput();
            }
        }

        /// <summary>
        /// Frees up the writer.
        /// </summary>
        public void Dispose()
        {
            LockOutput();
            try
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
            finally
            {
                UnlockOutput();
            }
        }
    }
}
