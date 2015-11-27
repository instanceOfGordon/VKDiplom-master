#pragma once
#include "MathFunction.h"
#include "SurfaceDimension.h"
#include <vector>

namespace splineknots
{
	typedef std::vector<double> KnotVector;

	class CurveKnotsGenerator
	{
		InterpolativeMathFunction function_;
	public:
		virtual ~CurveKnotsGenerator() = default;
		const InterpolativeMathFunction& Function() const;
		virtual KnotVector GenerateKnots(const SurfaceDimension& dimension) = 0;
	protected:
		CurveKnotsGenerator(MathFunction function);
		CurveKnotsGenerator(InterpolativeMathFunction function);
	};
}
