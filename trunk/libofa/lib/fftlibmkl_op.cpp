/* ------------------------------------------------------------------

   libofa -- the Open Fingerprint Architecture library

   Copyright (C) 2006 Predixis Corporation
   All rights reserved.

-------------------------------------------------------------------*/
// FILE: "ffflibmkl_op.cpp"
// MODULE: Wrapper for Intel MKL library calls
// AUTHOR: Frode Holm
// DATE CREATED: 1/12/06

extern "C"
void dzfft1dc(double* r, int m, int isign, double* ws);

#include "../config.h"
#include "fftlib_op.h"

void 
FFTLib_op::Initialize(int N, bool optimize)
{
//	WSave = new double[N*2+8];
//	double* in=0;
//	dzfft1dc(in, N, 0, WSave);
}

void
FFTLib_op::Destroy()
{
	if (WSave)
		delete[] WSave;
}

void
FFTLib_op::SetSize(int N, bool optimize, double *in, double *out)
{
	if (WSave)
		delete[] WSave;
	WSave = new double[N*2+8];
	double intmp[1];
	dzfft1dc(intmp, N, 0, WSave);
}


void 
FFTLib_op::ComputeFrame(int N, double *in, double *out)
{

	dzfft1dc(in, N, 1, WSave);

	// Convert to hermittian order
	int i, j;
	for (i=0; i<=N/2; i++)
		out[i] = in[i];
	for (i=N/2+1, j=N; i<N; i++, j--)
		out[i] = in[j];


}

