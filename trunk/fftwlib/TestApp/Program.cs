using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            fftwtest ftest = new fftwtest();
            ftest.InitFFTW(3);
            ftest.TestAll();
            ftest.FreeFFTW();
        }
    }
    public class fftwtest
    {
        //pointers to unmanaged arrays
        IntPtr pin, pout;

        //managed arrays
        float[] fin, fout;

        //handles to managed arrays, keeps them pinned in memory
        GCHandle hin, hout;

        //pointers to the FFTW plan objects
        IntPtr fplan1, fplan2, fplan3;

        // Initializes FFTW and all arrays
        // n: Logical size of the transform
        public void InitFFTW(int n)
        {
            //create two unmanaged arrays, properly aligned
            pin = fftwf.malloc(n * 8);
            pout = fftwf.malloc(n * 8);

            //create two managed arrays, possibly misalinged
            //n*2 because we are dealing with complex numbers
            fin = new float[n * 2];
            fout = new float[n * 2];

            //get handles and pin arrays so the GC doesn't move them
            hin = GCHandle.Alloc(fin, GCHandleType.Pinned);
            hout = GCHandle.Alloc(fout, GCHandleType.Pinned);

            //fill our arrays with a sawtooth signal
            for (int i = 0; i < n * 2; i++)
                fin[i] = i % 50;
            for (int i = 0; i < n * 2; i++)
                fout[i] = i % 50;

            //copy managed arrays to unmanaged arrays
            Marshal.Copy(fin, 0, pin, n * 2);
            Marshal.Copy(fout, 0, pout, n * 2);

            //create a few test transforms
            fplan1 = fftwf.dft_1d(n, pin, pout, fftw_direction.Forward, fftw_flags.Estimate);
            fplan2 = fftwf.dft_1d(n, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(), fftw_direction.Forward, fftw_flags.Estimate);
            fplan3 = fftwf.dft_1d(n, hout.AddrOfPinnedObject(), pin, fftw_direction.Backward, fftw_flags.Measure);
        }

        public void TestAll()
        {
            TestPlan(fplan1);
            TestPlan(fplan2);
            TestPlan(fplan3);
        }

        // Tests a single plan, displaying results
        //plan: Pointer to plan to test
        public void TestPlan(IntPtr plan)
        {
            int start = System.Environment.TickCount;
            for (int i = 0; i < 10000; i++)
            {
                fftwf.execute(plan);
            }
            Console.WriteLine("Time: {0} ms",
                (System.Environment.TickCount - start));
            //a: adds, b: muls, c: fmas
            double a = 0, b = 0, c = 0;
            fftwf.flops(plan, ref a, ref b, ref c);
            Console.WriteLine("Approx. flops: {0}", (a + b + 2 * c));
        }

        // Releases all memory used by FFTW/C#
        public void FreeFFTW()
        {
            //it is essential that you call these after finishing
            //may want to put the initializers in the constructor
            //and these in the destructor
            fftwf.free(pin);
            fftwf.free(pout);
            fftwf.destroy_plan(fplan1);
            fftwf.destroy_plan(fplan2);
            fftwf.destroy_plan(fplan3);
            hin.Free();
            hout.Free();
        }
    }
}
