namespace Fingerprinting.Audio.Wav
{
    using System;
    using System.IO;

    public class WaveData
    {
        private Stream dataStream;

        public WaveData(Stream dataStream)
        {
            this.dataStream = dataStream;
        }

        public Stream DataStream
        {
            get
            {
                return this.dataStream;
            }
        }
    }
}

