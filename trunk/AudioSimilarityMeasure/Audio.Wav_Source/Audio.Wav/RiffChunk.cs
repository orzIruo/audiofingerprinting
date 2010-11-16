namespace Fingerprinting.Audio.Wav
{
    using System;
    using System.IO;

    public class RiffChunk
    {
        private uint dataSize;
        private RiffChunkStream dataStream;
        private byte[] id;

        public RiffChunk()
        {
        }

        public RiffChunk(byte[] id, uint dataSize)
        {
            this.id = id;
            this.dataSize = dataSize;
        }

        public static RiffChunk Create(Stream stream)
        {
            RiffChunk chunk = new RiffChunk();
            chunk.Read(stream);
            return chunk;
        }

        private void Read(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            this.id = reader.ReadBytes(4);
            this.dataSize = reader.ReadUInt32();
            this.dataStream = new RiffChunkStream(stream, stream.Position, (long) this.dataSize);
        }

        public uint DataSize
        {
            get
            {
                return this.dataSize;
            }
            set
            {
                this.dataSize = value;
            }
        }

        public Stream DataStream
        {
            get
            {
                return this.dataStream;
            }
        }

        public byte[] Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
    }
}

