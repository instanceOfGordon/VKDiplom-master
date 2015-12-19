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

		CurveKnotsGenerator(const CurveKnotsGenerator& other) = default;
		CurveKnotsGenerator(CurveKnotsGenerator&& other) = default;
		CurveKnotsGenerator& operator=(const CurveKnotsGenerator& other) = default;
		CurveKnotsGenerator& operator=(CurveKnotsGenerator&& other) = default;
		const InterpolativeMathFunction& Function() const;
		virtual KnotVector GenerateKnots(const SurfaceDimension& dimension) = 0;
	protected:
		CurveKnotsGenerator(MathFunction function);
		CurveKnotsGenerator(InterpolativeMathFunction function);
	};
}
