namespace Fingerprinting.Audio.Wav
{
    using System;

    public enum WaveCompression : ushort
    {
        Experimental = 0xffff,
        IMAADPCM = 0x11,
        ITUG711ALaw = 6,
        ITUG711uLaw = 7,
        MicrosoftADPCM = 2,
        MPEG = 80,
        PCM = 1,
        Unknown = 0
    }
}

