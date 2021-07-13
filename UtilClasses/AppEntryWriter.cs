using System;
using System.IO;
using System.Text;

namespace UtilClasses
{
    public class AppEntryWriter : IDisposable
    {

        public StreamWriter Writer { get; }

        public AppEntryWriter(string file)
        {
            Writer = new StreamWriter(new FileStream(file, FileMode.Create),
                encoding: Encoding.UTF8);
        }

        ~AppEntryWriter()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            // уЕшьTssODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Writer?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
