/* ------------------------------------------------------------------

   libofa -- the Open Fingerprint Architecture library

   Copyright (C) 2006 Predixis Corporation
   All rights reserved.

-------------------------------------------------------------------*/
// FILE: "fftlibw_op.cpp"
// MODULE: Wrapper for MIT FFTW ver 2.0 library calls
// AUTHOR: Frode Holm
// DATE CREATED: 1/12/06

#include "fftlib_op.h"


void 
FFTLib_op::Initialize(int N, bool optimize)
{
	int flags;

	if (optimize)
		flags = FFTW_MEASURE;
	else
		flags = FFTW_ESTIMATE;

	PlanF = rfftw_create_plan(N, FFTW_REAL_TO_COMPLEX, flags);
}

void
FFTLib_op::Destroy()
{
	fftw_destroy_plan(PlanF);
}

void
FFTLib_op::SetSize(int N, bool optimize, double *in, double *out)
{
	int flags;

	if (optimize)
		flags = FFTW_MEASURE;
	else
		flags = FFTW_ESTIMATE;

	if (PlanF != 0)
		fftw_destroy_plan(PlanF);
	PlanF = rfftw_create_plan(N, FFTW_REAL_TO_COMPLEX, flags);
}

void 
FFTLib_op::ComputeFrame(int N, double *in, double *out)
{

	rfftw_one(PlanF, in, out);

}

