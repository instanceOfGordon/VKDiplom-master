#include "stdafx.h"
#include "KnotsGenerator.h"

namespace splineknots {
	/*double safeCall(MathFunction function, double x, double y)
	{
		auto value = function(x, y);
		if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
		value = function(x, y + double.Epsilon);
		if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
		value = function(x + double.Epsilon, y + double.Epsilon);
		return value;
	}*/

	KnotsGenerator::~KnotsGenerator()
	{
	}

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