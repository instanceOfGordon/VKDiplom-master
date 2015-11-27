#include "stdafx.h"
#include "KnotsGenerator.h"

namespace splineknots
{
	const InterpolativeMathFunction& KnotsGenerator::Function() const
	{
		return function_;
	}

	KnotsGenerator::KnotsGenerator(MathFunction function)
		: function_(function)
	{
	}

	KnotsGenerator::KnotsGenerator(InterpolativeMathFunction function)
		: function_(function)
	{
	}
}
