namespace Fingerprinting.Audio.Wav
{
    using System;

    public class WaveFormat
    {
        private uint averageBytesPerSecond;
        private ushort bitsPerSample;
        private ushort blockAlign;
        private WaveCompression compression;
        private byte[] extraFormatBytes;
        private ushort numberOfChannels;
        private uint sampleRate;

        public WaveFormat()
        {
        }

        public WaveFormat(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length < 0x10)
            {
                throw new ArgumentException("The input byte array must be at least 16 bytes long.");
            }
            ushort compressionCode = BitConverter.ToUInt16(bytes, 0);
            this.compression = (WaveCompression) compressionCode;
            this.numberOfChannels = BitConverter.ToUInt16(bytes, 2);
            this.sampleRate = BitConverter.ToUInt32(bytes, 4);
            this.averageBytesPerSecond = BitConverter.ToUInt32(bytes, 8);
            this.blockAlign = BitConverter.ToUInt16(bytes, 12);
            this.bitsPerSample = BitConverter.ToUInt16(bytes, 14);
            if ((bytes.Length > 0x10) && (this.compression != WaveCompression.PCM))
            {
                ushort extraByteCount = BitConverter.ToUInt16(bytes, 0x10);
                Array.Copy(bytes, 0x12, this.extraFormatBytes, 0, extraByteCount);
            }
        }

        public uint AverageBytesPerSecond
        {
            get
            {
                return this.averageBytesPerSecond;
            }
            set
            {
                this.averageBytesPerSecond = value;
            }
        }

        public ushort BitsPerSample
        {
            get
            {
                return this.bitsPerSample;
            }
            set
            {
                this.bitsPerSample = value;
            }
        }

        public ushort BlockAlign
        {
            get
            {
                return this.blockAlign;
            }
            set
            {
                this.blockAlign = value;
            }
        }

        public WaveCompression Compression
        {
            get
            {
                return this.compression;
            }
            set
            {
                this.compression = value;
            }
        }

        public byte[] ExtraFormatBytes
        {
            get
            {
                return this.extraFormatBytes;
            }
            set
            {
                this.extraFormatBytes = value;
            }
        }

        public ushort NumberOfChannels
        {
            get
            {
                return this.numberOfChannels;
            }
            set
            {
                this.numberOfChannels = value;
            }
        }

        public uint SampleRate
        {
            get
            {
                return this.sampleRate;
            }
            set
            {
                this.sampleRate = value;
            }
        }
    }
}

