namespace Fingerprinting.Runtime.InteropServices
{
    using System;
    using System.Runtime.InteropServices;

    public class HGlobalMemoryBuffer : IDisposable
    {
        private bool disposed;
        private IntPtr pointer;
        private int size;

        public HGlobalMemoryBuffer(int cb)
        {
            this.pointer = Marshal.AllocHGlobal(cb);
            this.size = cb;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.pointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(this.pointer);
                    this.pointer = IntPtr.Zero;
                    this.size = 0;
                }
                this.disposed = true;
            }
        }

        ~HGlobalMemoryBuffer()
        {
            this.Dispose(false);
        }

        public unsafe byte[] ToArray()
        {
            byte[] buffer = new byte[this.size];
            byte* numPtr = (byte*) this.Pointer.ToPointer();
            int index = 0;
            while (index < this.size)
            {
                buffer[index] = numPtr[0];
                index++;
                numPtr++;
            }
            return buffer;
        }

        public IntPtr EndPointer
        {
            get
            {
                return new IntPtr(this.pointer.ToInt64() + this.size);
            }
        }

        public IntPtr Pointer
        {
            get
            {
                return this.pointer;
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
        }
    }
}

