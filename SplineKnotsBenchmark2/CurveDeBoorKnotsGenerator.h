#pragma once
#include <vector>
#include "MathFunction.h"
#include "SurfaceDimension.h"
namespace splineknots
{
	typedef std::vector<double> KnotVector;
	class CurveDeboorKnotsGenerator final
	{
		InterpolativeMathFunction function_;
		KnotVector RightSide(const KnotVector& function_values, double h, double dfirst, double dlast);
		void InitializeKnots(const SurfaceDimension& dimension, KnotVector& knots);
	public:
		CurveDeboorKnotsGenerator(const MathFunction function);
		CurveDeboorKnotsGenerator(const InterpolativeMathFunction function);
		KnotVector GenerateKnots(const SurfaceDimension& dimension, double* calculation_time = nullptr);
	};
}
