namespace Fingerprinting.Audio.Wav
{
    using System;

    public class InvalidWaveException : Exception
    {
        public InvalidWaveException()
        {
        }

        public InvalidWaveException(string message) : base(message)
        {
        }

        public InvalidWaveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

