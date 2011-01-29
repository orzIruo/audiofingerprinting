namespace Fingerprinting.Audio.Wav
{
    using System;
    using System.IO;
using Mp3Sharp;

    public class Wave
    {
        private WaveData data;
        private WaveFormat format;
        private static readonly byte[] waveRiffType = new byte[] { 0x57, 0x41, 0x56, 0x45 };

        public Wave(Mp3Stream stream)
        {
            this.Read(stream);
        }

        public Wave(Stream stream)
        {
            this.Read(stream);
        }

        private static bool Equals(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
            {
                return false;
            }
            int length = buffer1.Length;
            for (int i = 0; i < length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public TimeSpan GetDuration()
        {
            if (this.format == null)
            {
                throw new InvalidOperationException("Could not obtain the wave duration, because the wave format is not present.");
            }
            if (this.data == null)
            {
                throw new InvalidOperationException("Could not obtain the wave duration, because the wave data is not present.");
            }
            if (this.data.DataStream == null)
            {
                throw new InvalidOperationException("Could not obtain the wave duration, because the wave data stream is not present.");
            }
            double secs = ((double) this.data.DataStream.Length) / ((double) this.format.AverageBytesPerSecond);
            return new TimeSpan((long) (10000000.0 * secs));
        }

        public ulong GetNumberOfSamples()
        {
            if (this.format == null)
            {
                throw new InvalidOperationException("Could not obtain the number of samples, because the wave format is not present.");
            }
            if (this.data == null)
            {
                throw new InvalidOperationException("Could not obtain the number of samples, because the wave data is not present.");
            }
            if (this.data.DataStream == null)
            {
                throw new InvalidOperationException("Could not obtain the number of samples, because the wave data stream is not present.");
            }
            uint bytesPerSample = (uint) (this.format.BitsPerSample >> 3);
            return (ulong)((uint)((ulong)this.data.DataStream.Length / ((ulong)(this.format.NumberOfChannels * bytesPerSample))));
        }

        private void Read(Mp3Stream stream)
        {
            if (stream.Frequency < 0) stream.DecodeFrames(1);
            if (stream.Frequency > 0 && stream.ChannelCount > 0)
            {
                //var _waveFormat = new WaveFormat(mp3Stream.Frequency, 16, mp3Stream.ChannelCount);
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.data = new WaveData(stream);
            stream.Seek(0L, SeekOrigin.Begin);
        }

        private void Read(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            RiffChunk riffChunk = RiffChunk.Create(stream);
            if (!Equals(riffChunk.Id, RiffChunks.Riff))
            {
                throw new InvalidWaveException("RIFF chunk was not found in the stream.");
            }
            byte[] riffType = new byte[4];
            riffChunk.DataStream.Read(riffType, 0, 4);
            if (!Equals(riffType, waveRiffType))
            {
                throw new InvalidWaveException("WAVE chunk was not found in the stream.");
            }
            do
            {
                RiffChunk chunk = RiffChunk.Create(riffChunk.DataStream);
                if (Equals(chunk.Id, RiffChunks.Format))
                {
                    byte[] fmtData = new byte[chunk.DataSize];
                    chunk.DataStream.Read(fmtData, 0, (int) chunk.DataSize);
                    this.format = new WaveFormat(fmtData);
                }
                else if (Equals(chunk.Id, RiffChunks.Data))
                {
                    this.data = new WaveData(chunk.DataStream);
                    chunk.DataStream.Seek(0L, SeekOrigin.Begin);
                }
                else
                {
                    riffChunk.DataStream.Seek((long) chunk.DataSize, SeekOrigin.Current);
                }
            }
            while (riffChunk.DataStream.Position < riffChunk.DataStream.Length);
        }

        public void Write(Stream stream)
        {
            throw new NotImplementedException("The method is not implemented.");
        }

        public WaveData Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        public WaveFormat Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.format = value;
            }
        }
    }
}

