#pragma once
#include "CurveDeboorKnotsGenerator.h"

namespace splineknots
{
	class ReducedCurveDeboorKnotsGenerator : public splineknots::CurveDeboorKnotsGenerator
	{
	public:
		splineknots::KnotVector GenerateKnots(const splineknots::SurfaceDimension& dimension) override;
		~ReducedCurveDeboorKnotsGenerator() override;
		ReducedCurveDeboorKnotsGenerator(const splineknots::MathFunction& function);
		ReducedCurveDeboorKnotsGenerator(const splineknots::InterpolativeMathFunction& function);
	protected:
		splineknots::KnotVector RightSide(const splineknots::KnotVector& function_values, double h, double dfirst, double dlast) override;
	};
}
