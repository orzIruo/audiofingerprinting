    // Create a new instance of SoundTouch processor.
    //private static extern IntPtr soundtouch_createInstance();

    // Destroys a SoundTouch processor instance.
    //private static extern void soundtouch_destroyInstance(IntPtr h);

    // Get SoundTouch library version string
    //[return: MarshalAs(UnmanagedType.LPStr)]
    //private static extern String soundtouch_getVersionString();

    // Get SoundTouch library version Id
    //private static extern uint soundtouch_getVersionId();

    // Sets new rate control value. Normal rate = 1.0, smaller values
    // represent slower rate, larger faster rates.
    //private static extern void soundtouch_setRate(IntPtr h, float newRate);

    // Sets new tempo control value. Normal tempo = 1.0, smaller values
    // represent slower tempo, larger faster tempo.
    //private static extern void soundtouch_setTempo(IntPtr h, float newTempo);

    // Sets new rate control value as a difference in percents compared
    // to the original rate (-50 .. +100 %);
    //private static extern void soundtouch_setRateChange(IntPtr h, float newRate);

    // Sets new tempo control value as a difference in percents compared
    // to the original tempo (-50 .. +100 %);
    //private static extern void soundtouch_setTempoChange(IntPtr h, float newTempo);

    // Sets new pitch control value. Original pitch = 1.0, smaller values
    // represent lower pitches, larger values higher pitch.
    //private static extern void soundtouch_setPitch(IntPtr h, float newPitch);

    // Sets pitch change in octaves compared to the original pitch  
    // (-1.00 .. +1.00);
    //private static extern void soundtouch_setPitchOctaves(IntPtr h, float newPitch);

    // Sets pitch change in semi-tones compared to the original pitch
    // (-12 .. +12);
    //private static extern void soundtouch_setPitchSemiTones(IntPtr h, float newPitch);


    // Sets the number of channels, 1 = mono, 2 = stereo
    //private static extern void soundtouch_setChannels(IntPtr h, uint numChannels);

    // Sets sample rate.
    //private static extern void soundtouch_setSampleRate(IntPtr h, uint srate);

    // Flushes the last samples from the processing pipeline to the output.
    // Clears also the internal processing buffers.
    ////
    // Note: This function is meant for extracting the last samples of a sound
    // stream. This function may introduce additional blank samples in the end
    // of the sound stream, and thus it's not recommended to call this function
    // in the middle of a sound stream.
    //private static extern void soundtouch_flush(IntPtr h);

    // Adds 'numSamples' pcs of samples from the 'samples' memory position into
    // the input of the object. Notice that sample rate _has_to_ be set before
    // calling this function, otherwise throws a runtime_error exception.
    //private static extern void soundtouch_putSamples(IntPtr h,
    //   [MarshalAs(UnmanagedType.LPArray)] float[] samples,       ///< Pointer to sample buffer.
    //   uint numSamples      ///< Number of samples in buffer. Notice
    //                        ///< that in case of stereo-sound a single sample
    //                        ///< contains data for both channels.
    //   );

    // Clears all the samples in the object's output and internal processing
    // buffers.
    //private static extern void soundtouch_clear(IntPtr h);

    // Changes a setting controlling the processing system behaviour. See the
    // 'SETTING_...' defines for available setting ID's.
    // 
    // \return 'TRUE' if the setting was succesfully changed
    //private static extern bool soundtouch_setSetting(IntPtr h,
    //           int settingId,   ///< Setting ID number. see SETTING_... defines.
    //           int value        ///< New setting value.
    //           );

    // Reads a setting controlling the processing system behaviour. See the
    // 'SETTING_...' defines for available setting ID's.
    //
    // \return the setting value.
    //private static extern int soundtouch_getSetting(IntPtr h,
    //                     int settingId    ///< Setting ID number, see SETTING_... defines.
    //           );


    // Returns number of samples currently unprocessed.
    //private static extern uint soundtouch_numUnprocessedSamples(IntPtr h);

    // Adjusts book-keeping so that given number of samples are removed from beginning of the 
    // sample buffer without copying them anywhere. 
    //
    // Used to reduce the number of samples in the buffer when accessing the sample buffer directly
    // with 'ptrBegin' function.
    //private static extern uint soundtouch_receiveSamples(IntPtr h,
    //       [MarshalAs(UnmanagedType.LPArray)] float[] outBuffer,           ///< Buffer where to copy output samples.
    //       uint maxSamples     ///< How many samples to receive at max.
    //       );

    // Returns number of samples currently available.
    //private static extern uint soundtouch_numSamples(IntPtr h);

    // Returns nonzero if there aren't any samples available for outputting.
    //private static extern uint soundtouch_isEmpty(IntPtr h);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoundTouchNet;

namespace SoundCatcher
{
    public class SoundTouchTransforms
    {
        public static float[] ChangeTempo(byte[] input, int sampleRate, int nbChannels)
        {
            using (SoundStretcher stretcher = new SoundStretcher(sampleRate, 1))
            {

                Assert.IsNotNull(stretcher);
                Assert.IsTrue(stretcher.CanWrite);
                Assert.IsFalse(stretcher.CanRead);
                Assert.AreEqual(stretcher.AvaiableSamples, 0);
                
                stretcher.Tempo = 2;

                float[] samplesIn = new float[sampleRate];

                for (int i = 0; i < samplesIn.Length; i++)
                    samplesIn[i] = (float)input[i];

                stretcher.PutSamples(samplesIn, samplesIn.Length);

                stretcher.Flush();
                Assert.IsTrue(stretcher.CanRead);

                float[] samplesOut = new float[stretcher.AvaiableSamples];
                stretcher.ReceiveSamples(samplesOut, samplesOut.Length);

                Assert.AreEqual(stretcher.AvaiableSamples, 0);
                Assert.IsFalse(stretcher.CanRead);
                stretcher.Clear();

                return samplesOut;
            }
        }
    }
}
