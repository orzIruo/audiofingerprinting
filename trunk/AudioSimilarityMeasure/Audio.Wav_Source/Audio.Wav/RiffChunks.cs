namespace Fingerprinting.Audio.Wav
{
    using System;

    public static class RiffChunks
    {
        private static readonly byte[] data = new byte[] { 100, 0x61, 0x74, 0x61 };
        private static readonly byte[] format = new byte[] { 0x66, 0x6d, 0x74, 0x20 };
        private static readonly byte[] riff = new byte[] { 0x52, 0x49, 70, 70 };

        public static byte[] Data
        {
            get
            {
                return data;
            }
        }

        public static byte[] Format
        {
            get
            {
                return format;
            }
        }

        public static byte[] Riff
        {
            get
            {
                return riff;
            }
        }
    }
}

