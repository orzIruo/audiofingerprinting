namespace Fingerprinting.Audio.FooIdFingerprinting
{
    using System;

    public class UnsupportedFooIdFingerprintException : Exception
    {
        public UnsupportedFooIdFingerprintException(string message) : base(message)
        {
        }
    }
}

