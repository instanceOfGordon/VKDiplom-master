#pragma once

#include "CurveKnotsGenerator.h"

namespace splineknots
{
	class CurveDeboorKnotsGenerator : public CurveKnotsGenerator
	{
	public:
		CurveDeboorKnotsGenerator(const MathFunction function);
		CurveDeboorKnotsGenerator(const InterpolativeMathFunction function);
		KnotVector GenerateKnots(const SurfaceDimension& dimension) override;
	protected:
		virtual KnotVector RightSide(const KnotVector& function_values, double h, double dfirst, double dlast);
		void InitializeKnots(const SurfaceDimension& dimension, KnotVector& knots);
	};
}
