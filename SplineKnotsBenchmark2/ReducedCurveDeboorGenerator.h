#pragma once
#include "CurveDeboorKnotsGenerator.h"

namespace splineknots
{
	class ReducedCurveDeboorKnotsGenerator
	{
		InterpolativeMathFunction function_;
		KnotVector RightSide(const splineknots::KnotVector& function_values, double h, double dfirst, double dlast);
		void InitializeKnots(const SurfaceDimension& dimension, KnotVector& knots);
	public:
		KnotVector GenerateKnots(const splineknots::SurfaceDimension& dimension, double* calculation_time = nullptr);
		ReducedCurveDeboorKnotsGenerator(const splineknots::MathFunction& function);
		ReducedCurveDeboorKnotsGenerator(const splineknots::InterpolativeMathFunction& function);
	};
}
