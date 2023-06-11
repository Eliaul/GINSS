using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.IO
{
    public abstract class FileStreamWriter : IDisposable
    {
        protected string? FilePath;

        protected FileStream? fileStream;

        protected StreamWriter? streamWriter;

        private bool disposed;

        protected FileStreamWriter(string filePath)
        {
            FilePath = filePath;
            fileStream = new(filePath, FileMode.Create, FileAccess.Write);
            streamWriter = new(fileStream);
            disposed = false;
        }

        public void WriteLine()
        {
            streamWriter?.WriteLine(Line());
        }

        protected abstract string Line();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileStreamWriter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                if (FilePath != null)
                    FilePath = null;
            }
            if (fileStream != null)
            {
                fileStream.Dispose();
                fileStream = null;
            }
            if (streamWriter != null)
            {
                streamWriter.Dispose();
                streamWriter = null;
            }
            disposed = true;
        }
    }
}
