namespace Fingerprinting.Audio.FooIdFingerprinting
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Fingerprinting.Audio.Wav;
    using Fingerprinting.Runtime.InteropServices;
    using Mp3Sharp;

    public class FooIdFingerprint
    {
        private short avgDom;
        private short avgFit;
        private int[] dom;
        private int length;
        private int[,] r;
        private short version;

        public FooIdFingerprint()
        {
            this.r = new int[0x57, 0x10];
            this.dom = new int[0x57];
        }

        public FooIdFingerprint(Stream inputStream)
        {
            this.r = new int[0x57, 0x10];
            this.dom = new int[0x57];
            this.Read(inputStream);
        }

        public FooIdFingerprint(string fingerprintFile)
        {
            this.r = new int[0x57, 0x10];
            this.dom = new int[0x57];
            this.Read(fingerprintFile);
        }

        public FooIdFingerprint(Wave input)
        {
            this.r = new int[0x57, 0x10];
            this.dom = new int[0x57];
            this.Calculate(input);
        }

        public void Calculate(Wave input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (input.Format.BitsPerSample != 0x10)
            {
                throw new NotSupportedException("Only 16-bit signed WAVE streams are currently supported.");
            }
            IntPtr fid = FooId.Init((int) input.Format.SampleRate, 1);
            if (!(fid != IntPtr.Zero))
            {
                throw new Exception("Could not initialize FooID library.");
            }
            TimeSpan ts = input.GetDuration();
            float[] buffer = GetMonoArray(input, Math.Min((int)(Math.Round(ts.TotalSeconds)), 100));
            GCHandle gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            int result = FooId.FeedFloat(fid, buffer, buffer.Length);
            gchandle.Free();
            switch (result)
            {
                case 0:
                {
                    FooId.GetVersion(fid);
                    int centiseconds = (int) (input.GetDuration().Ticks / 0x186a0L);
                    using (HGlobalMemoryBuffer fpBuffer = new HGlobalMemoryBuffer(FooId.GetSize(fid)))
                    {
                        FooId.Calculate(fid, centiseconds, fpBuffer.Pointer);
                        using (MemoryStream mstream = new MemoryStream(fpBuffer.ToArray()))
                        {
                            this.Read(mstream);
                        }
                    }
                    FooId.Free(fid);
                    return;
                }
                case 1:
                    throw new Exception("Not enough data to calculate the audio fingerprint.");
            }
            if (result < 0)
            {
                throw new Exception("Could not calculate the audio fingerprint.");
            }
        }

        protected float FullMatch(FooIdFingerprint other)
        {
            int maxframe = (int) Math.Round((double) ((this.length * 0.9765625f) / 100f));
            maxframe = Math.Min(maxframe, 0x57);
            int[] rf = new int[4];
            int[] df = new int[64];
            int tdf = 0;
            int trf = 0;
            for (int f = 0; f < maxframe; f++)
            {
                for (int b = 0; b < 0x10; b++)
                {
                    int rdiff = Math.Abs((int) (this.r[f, b] - other.r[f, b]));
                    rf[rdiff]++;
                    trf += rdiff;
                }
                int ddiff = Math.Abs((int) (this.dom[f] - other.dom[f]));
                df[ddiff]++;
                tdf += ddiff;
            }
            int maxrflaw = 0x90 * maxframe;
            int maxdflaw = (0x3f * maxframe) / 4;
            int w_trf = (rf[1] + (rf[2] * 4)) + (rf[3] * 9);
            int w_tdf = tdf / 4;
            int totalflaws = w_trf + w_tdf;
            int maxflaws = maxrflaw + maxdflaw;
            float perc = ((float) totalflaws) / ((float) maxflaws);
            float conf = ((1f - perc) - 0.5f) * 2f;
            return Math.Max(Math.Min(conf, 1f), 0f);
        }

        private static float[] GetMonoArray(Wave input, int length)
        {
            int numberOfChannels = input.Format.NumberOfChannels;
            int numberOfSamples = (int) input.GetNumberOfSamples();
            int bytesPerSample = input.Format.BitsPerSample / 8;
            int bytesPerSecond = (int) (bytesPerSample * input.Format.SampleRate);
            int maxLength = length * bytesPerSecond;
            int floatBufferLength = Math.Min(maxLength, numberOfSamples);
            float[] floatBuffer = new float[floatBufferLength];
            byte[] bytes = ReadWaveStream(input, floatBufferLength);
            int offset = 0;
            for (int i = 0; i < floatBuffer.Length; i++)
            {
                int iValue = 0;
                for (int ch = 0; ch < numberOfChannels; ch++)
                {
                    iValue += BitConverter.ToInt16(bytes, offset);
                    offset += bytesPerSample;
                }
                floatBuffer[i] = ((float) iValue) / (32767f * numberOfChannels);
            }
            return floatBuffer;
        }

        public float Match(FooIdFingerprint other)
        {
            if (this.QuickMatch(other))
            {
                return this.FullMatch(other);
            }
            return 0f;
        }

        protected bool QuickMatch(FooIdFingerprint other)
        {
            if ((this.length + 0xbb8) < other.length)
            {
                return false;
            }
            if ((this.length - 0xbb8) > other.length)
            {
                return false;
            }
            if ((this.avgFit + 400) < other.avgFit)
            {
                return false;
            }
            if ((this.avgFit - 400) > other.avgFit)
            {
                return false;
            }
            if ((this.avgDom + 600) < other.avgDom)
            {
                return false;
            }
            if ((this.avgDom - 600) > other.avgDom)
            {
                return false;
            }
            return true;
        }

        public void Read(Stream inputStream)
        {
            using (BinaryReader reader = new BinaryReader(inputStream))
            {
                this.version = reader.ReadInt16();
                if (this.version != 0)
                {
                    throw new UnsupportedFooIdFingerprintException("Fingerprint version (" + this.version + ") is not supported.");
                }
                this.length = reader.ReadInt32();
                this.avgFit = reader.ReadInt16();
                this.avgDom = reader.ReadInt16();
                for (int i = 0; i < 0x57; i++)
                {
                    for (int j = 0; j < 0x10; j += 4)
                    {
                        byte value = reader.ReadByte();
                        this.r[i, j] = (value >> 6) & 3;
                        this.r[i, j + 1] = (value >> 4) & 3;
                        this.r[i, j + 2] = (value >> 2) & 3;
                        this.r[i, j + 3] = value & 3;
                    }
                }
                int dompos = 0;
                for (int i = 0; i < 0x16; i++)
                {
                    byte value1 = reader.ReadByte();
                    byte value2 = reader.ReadByte();
                    byte value3 = reader.ReadByte();
                    this.dom[dompos++] = (value1 >> 2) & 0x3f;
                    this.dom[dompos++] = ((value1 & 3) << 4) | ((value2 >> 4) & 15);
                    this.dom[dompos++] = ((value2 & 15) << 2) | ((value3 >> 6) & 3);
                    if (dompos < 0x57)
                    {
                        this.dom[dompos++] = value3 & 0x3f;
                    }
                }
            }
        }

        public void Read(string inputFile)
        {
            using (FileStream fileStream = new FileStream(inputFile, FileMode.Open))
            {
                this.Read(fileStream);
            }
        }

        public static int OffSet = 0;
        private static byte[] ReadWaveStream(Wave input, int length)
        {
            int maxLength = length * input.Format.BlockAlign;
            byte[] bytes = new byte[Math.Min(input.Data.DataStream.Length, (long) maxLength)];
            input.Data.DataStream.Read(bytes, OffSet, bytes.Length - OffSet);
            return bytes;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(this.version);
            writer.Write(this.length);
            writer.Write(this.avgFit);
            writer.Write(this.avgDom);
            for (int i = 0; i < 0x57; i++)
            {
                for (int j = 0; j < 0x10; j += 4)
                {
                    uint r0 = (uint) (this.r[i, j] & 3);
                    uint r1 = (uint) (this.r[i, j + 1] & 3);
                    uint r2 = (uint) (this.r[i, j + 2] & 3);
                    uint r3 = (uint) (this.r[i, j + 3] & 3);
                    byte value = (byte) ((((r0 << 6) | (r1 << 4)) | (r2 << 2)) | r3);
                    writer.Write(value);
                }
            }
            for (int i = 0; i < 0x57; i += 4)
            {
                uint dom0 = (uint) (this.dom[i] & 0x3f);
                uint dom1 = (uint) (this.dom[i + 1] & 0x3f);
                uint dom2 = (uint) (this.dom[i + 2] & 0x3f);
                uint dom3 = (i < 0x54) ? ((uint) (this.dom[i + 3] & 0x3f)) : 0;
                byte value0 = (byte) ((dom0 << 2) | ((dom1 & 0x30) >> 4));
                byte value1 = (byte) (((dom1 & 15) << 4) | ((dom2 & 60) >> 2));
                byte value2 = (byte) (((dom2 & 3) << 6) | (dom3 & 0x3f));
                writer.Write(value0);
                writer.Write(value1);
                writer.Write(value2);
            }
        }

        public void Write(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                this.Write(writer);
            }
        }

        public int AverageDom
        {
            get
            {
                return this.avgDom;
            }
        }

        public int AverageFit
        {
            get
            {
                return this.avgFit;
            }
        }

        public int Length
        {
            get
            {
                return this.length;
            }
        }

        public int Version
        {
            get
            {
                return this.version;
            }
        }

        private static class FooId
        {
            [DllImport("FooID.dll", EntryPoint="fp_calculate", CallingConvention=CallingConvention.Cdecl)]
            public static extern int Calculate(IntPtr fid, int songlen, IntPtr buff);
            [DllImport("FooID.dll", EntryPoint="fp_feed_float", CallingConvention=CallingConvention.Cdecl)]
            public static extern int FeedFloat(IntPtr fid, [In, Out] float[] data, int size);
            [DllImport("FooID.dll", EntryPoint="fp_feed_short", CallingConvention=CallingConvention.Cdecl)]
            public static extern int FeedShort(IntPtr fid, IntPtr data, int size);
            [DllImport("FooID.dll", EntryPoint="fp_free", CallingConvention=CallingConvention.Cdecl)]
            public static extern void Free(IntPtr fid);
            [DllImport("FooID.dll", EntryPoint="fp_getsize", CallingConvention=CallingConvention.Cdecl)]
            public static extern int GetSize(IntPtr fid);
            [DllImport("FooID.dll", EntryPoint="fp_getversion", CallingConvention=CallingConvention.Cdecl)]
            public static extern int GetVersion(IntPtr fid);
            [DllImport("FooID.dll", EntryPoint="fp_init", CallingConvention=CallingConvention.Cdecl)]
            public static extern IntPtr Init(int samplerate, int channels);
        }
    }
}

