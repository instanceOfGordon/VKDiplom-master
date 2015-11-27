#pragma once
#include "CurveDeboorKnotsGenerator.h"

namespace splinkeknots
{
	class ReducedCurveDeboorGenerator : public splineknots::CurveDeboorKnotsGenerator
	{
	public:
		splineknots::KnotVector GenerateKnots(const splineknots::SurfaceDimension& dimension) override;
		~ReducedCurveDeboorGenerator() override;
		ReducedCurveDeboorGenerator(const splineknots::MathFunction& function);
		ReducedCurveDeboorGenerator(const splineknots::InterpolativeMathFunction& function);
	protected:
		splineknots::KnotVector RightSide(const splineknots::KnotVector& function_values, double h, double dfirst, double dlast) override;
	};
}
