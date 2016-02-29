#pragma once
#include "CurveDeboorKnotsGenerator.h"

namespace splineknots
{
	class ReducedCurveDeboorKnotsGenerator
	{
		InterpolativeMathFunction function_;
		bool is_buffered_;
		KnotVector RightSide(const splineknots::KnotVector& function_values, 
			double h, double dfirst, double dlast);
		void InitializeKnots(const SurfaceDimension& dimension,
			KnotVector& knots);
	public:
		KnotVector GenerateKnots(const SurfaceDimension& dimension, 
			double* calculation_time = nullptr);
		ReducedCurveDeboorKnotsGenerator(const MathFunction& function, 
			bool buffered = true);
		ReducedCurveDeboorKnotsGenerator
			(const InterpolativeMathFunction& function, bool buffered = true);
	};
}
