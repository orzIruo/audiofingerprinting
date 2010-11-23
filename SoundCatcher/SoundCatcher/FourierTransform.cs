/* Copyright (C) 2007 

   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;

namespace SoundCatcher
{
    public class FourierTransform
    {
        private static int n, nu;

        private static int BitReverse(int j)
        {
            int j2;
            int j1 = j;
            int k = 0;
            for (int i = 1; i <= nu; i++)
            {
                j2 = j1 / 2;
                k = 2 * k + j1 - 2 * j2;
                j1 = j2;
            }
            return k;
        }

        public static double[] FFT_3(ref double[] vector)
        {
            int n = vector.Length;
            double[] fin = new double[n * 2];
            double[] fout = new double[n * 2];
            GCHandle hin = GCHandle.Alloc(fin, GCHandleType.Pinned);
            GCHandle hout = GCHandle.Alloc(fout, GCHandleType.Pinned);
            for (int i = 0; i < n; i++)
            {
                fin[2 * i] = vector[i];
            }            
            IntPtr fplan = fftwf.dft_1d(n, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(),
                fftw_direction.Forward, fftw_flags.Measure);
            fftwf.execute(fplan);

            fftwf.destroy_plan(fplan);
            hin.Free();
            hout.Free();

            double[] decibels = new double[n];
            for (int i = 0; i < n; i++)
            {
                double x = fout[2 * i];
                double y = fout[2 * i + 1];
                decibels[i] = 10.0 * Math.Log10(Math.Sqrt(x * x + y * y));
            }
            return decibels;
        }

        public static double[] FFT_2(ref double[] vector)
        {
            int pow = (int)Math.Round(Math.Log(vector.Length, 2));
            double[] copy = new double[(int)Math.Pow(2, pow)];
            Array.Copy(vector, 0, copy, 0, Math.Min(vector.Length, copy.Length));

            double[] res, img;
            var cft = new MathNet.Numerics.Transformations.RealFourierTransformation();
            cft.TransformForward(copy, out res, out img);

            double[] decibels = new double[copy.Length];
            for (int i = 0; i < copy.Length; i++)
            { 
                double x = res[i];
                double y = img[i];                
                decibels[i] = 10.0 * Math.Log10(Math.Sqrt(x * x + y * y));
            }
            return decibels;
        }

        public static double[] FFT(ref double[] x)
        {
            
            int pow = (int)Math.Round(Math.Log(x.Length, 2));
            double[] copy = new double[(int)Math.Pow(2, pow)];
            Array.Copy(x, 0, copy, 0, Math.Min(x.Length, copy.Length));

            // Assume n is a power of 2 ?
            n = copy.Length;
            nu = (int)(Math.Log(n) / Math.Log(2));
            int n2 = n / 2;
            int nu1 = nu - 1;
            double[] xre = new double[n];
            double[] xim = new double[n];
            double[] magnitude = new double[n2];
            double[] decibel = new double[n2];
            double tr, ti, p, arg, c, s;
            for (int i = 0; i < n; i++)
            {
                xre[i] = copy[i];
                xim[i] = 0.0f;
            }
            int k = 0;
            for (int l = 1; l <= nu; l++)
            {
                while (k < n)
                {
                    for (int i = 1; i <= n2; i++)
                    {
                        p = BitReverse(k >> nu1);
                        arg = 2 * (double)Math.PI * p / n;
                        c = (double)Math.Cos(arg);
                        s = (double)Math.Sin(arg);
                        tr = xre[k + n2] * c + xim[k + n2] * s;
                        ti = xim[k + n2] * c - xre[k + n2] * s;
                        xre[k + n2] = xre[k] - tr;
                        xim[k + n2] = xim[k] - ti;
                        xre[k] += tr;
                        xim[k] += ti;
                        k++;
                    }
                    k += n2;
                }
                k = 0;
                nu1--;
                n2 = n2 / 2;
            }
            k = 0;
            int r;
            while (k < n)
            {
                r = BitReverse(k);
                if (r > k)
                {
                    tr = xre[k];
                    ti = xim[k];
                    xre[k] = xre[r];
                    xim[k] = xim[r];
                    xre[r] = tr;
                    xim[r] = ti;
                }
                k++;
            }
            for (int i = 0; i < n / 2; i++)
                //magnitude[i] = (float)(Math.Sqrt((xre[i] * xre[i]) + (xim[i] * xim[i])));
                decibel[i] = 10.0 * Math.Log10((float)(Math.Sqrt((xre[i] * xre[i]) + (xim[i] * xim[i]))));
            //return magnitude;
            return filter(decibel);
        }

        public static double[] filter(double[] data)
        {
            double fmin = 318;
            double fmax = 2000;
            int bins = 128;
            double basis = 1.11;
            bool log = true;

            double fs = 44100;
            int N = data.Length;
            
            double[] limits;
            if (log) limits = logSpace(fmin, fmax, bins);
            else limits = powSpace(basis, fmin, fmax, bins);

            double[] average = new double[bins];
            int[] count = new int[bins];
            int bin = 0;
            for (int i = (int)Math.Ceiling(fmin * N / fs); i * fs / N < fmax; i++)
            {
                while (i * fs / N > limits[bin + 1]) bin++;
                count[bin]++;
                average[bin] += data[i];
            } 
           
            for (int i = 0; i < bins; i++)
                if (count[i] > 0) average[i] /= count[i];

            return average;
        }

        public static double[] powSpace(double basis, double start, double end, int cnt)
        {
            double[] res = new double[cnt + 1];

            double factor = (end - start) * (basis - 1) / (Math.Pow(basis, cnt) - 1);
            double prev = start;
            for (int i = 0; i < cnt; i++)
            {
                res[i] = prev;
                prev += factor;
                factor *= basis;
            }
            res[cnt] = end;

            return res;
        }

        public static double[] logSpace(double start, double end, int cnt)
        {
            double[] res = new double[cnt + 1];

            double factor = Math.Log(end / start) / cnt;
            double expFactor = Math.Exp(factor);
            double prevFactor = 1.0;
            for (int i = 0; i < cnt; i++)
            {
                res[i] = start * prevFactor;
                prevFactor *= expFactor;
            }
            res[cnt] = end;

            return res;
        }
    }
}
