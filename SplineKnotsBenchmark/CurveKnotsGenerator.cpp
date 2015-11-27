#include "stdafx.h"
#include "CurveKnotsGenerator.h"


const splineknots::InterpolativeMathFunction& splineknots::CurveKnotsGenerator::Function() const
{
	return function_;
}

splineknots::CurveKnotsGenerator::CurveKnotsGenerator(MathFunction function)
	: function_(function)
{
}

splineknots::CurveKnotsGenerator::CurveKnotsGenerator(InterpolativeMathFunction function)
	: function_(function)
{
}
