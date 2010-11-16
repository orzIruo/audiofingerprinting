namespace Fingerprinting.Audio.Wav
{
    using System;
    using System.IO;

    public class RiffChunkStream : Stream
    {
        private Stream baseStream;
        private long baseStreamOffset;
        private long length;
        private long position;

        public RiffChunkStream(Stream baseStream, long offset, long length)
        {
            this.baseStream = baseStream;
            this.baseStreamOffset = offset;
            this.length = length;
            this.position = 0L;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.Seek(this.position, SeekOrigin.Begin);
            int result = this.baseStream.Read(buffer, offset, count);
            this.position = this.baseStream.Position - this.baseStreamOffset;
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.baseStream.Seek(offset + this.baseStreamOffset, SeekOrigin.Begin);
                    break;

                case SeekOrigin.Current:
                    this.baseStream.Seek(offset, SeekOrigin.Current);
                    break;

                case SeekOrigin.End:
                {
                    long fromEndPosition = ((this.baseStream.Length - this.length) - this.baseStreamOffset) + 1L;
                    this.baseStream.Seek(fromEndPosition, SeekOrigin.End);
                    break;
                }
            }
            this.position = this.baseStream.Position - this.baseStreamOffset;
            return this.position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get
            {
                return this.baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.baseStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.baseStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.length;
            }
        }

        public override long Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }
    }
}

