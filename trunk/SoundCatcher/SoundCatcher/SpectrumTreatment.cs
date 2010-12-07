using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundCatcher
{
    public class SpectrumTreatment
    {

        /*
            transform frequency to Bark
        */
        public static float toBARK(float f)
        {
            float z = ((26.81f * f) / (1960.0f + f)) - 0.53f;
            //z = 13f * (float)Math.Atan(0.00076f * f) + 3.5f * (float)Math.Atan(Math.Pow((f / 7500f), 2));

            if (z < 2.0f)
            {
                z = z + 0.15f * (2.0f - z);
            }
            else if (z > 20.1f)
            {
                z = z + 0.22f * (z - 20.1f);
            }

            return z;
        }

        public static float CriticalBandwidthHZ(float bark)
        {
            return 52548f / (bark * bark - 52.56f * bark + 690.39f);
        }

    }
}
