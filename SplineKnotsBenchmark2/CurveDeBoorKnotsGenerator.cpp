#include "stdafx.h"
#include "CurveDeboorKnotsGenerator.h"
#include "SplineKnots.h"
#include "StopWatch.h"


splineknots::KnotVector splineknots::CurveDeboorKnotsGenerator::RightSide(const KnotVector& knots, double h, double dfirst, double dlast)
{
	int num_unknowns = knots.size() - 2;
	auto mu1 = 3 / h;
	KnotVector rhs(num_unknowns);
	for (int i = 0; i < num_unknowns; i++)
	{
		rhs[i] = mu1 * (knots[i + 2] - knots[i]);
	}
	rhs[0] = rhs[0] - dfirst;
	rhs[num_unknowns - 1] = rhs[num_unknowns - 1] - dlast;
	return rhs;
}

splineknots::CurveDeboorKnotsGenerator::CurveDeboorKnotsGenerator(const MathFunction function): function_(function)
{
}

splineknots::CurveDeboorKnotsGenerator::CurveDeboorKnotsGenerator(const InterpolativeMathFunction function)
	:function_(function)
{
}

void splineknots::CurveDeboorKnotsGenerator::InitializeKnots(const SurfaceDimension& dimension, KnotVector& knots)
{
	auto h = abs(dimension.max - dimension.min) / (dimension.knot_count - 1);
	auto x = dimension.min;
	for (auto i = 0; i < dimension.knot_count; i++ , x += h)
	{
		auto y = function_.Z()(x, 0);
		knots[i] = y;
	}
}

splineknots::KnotVector splineknots::CurveDeboorKnotsGenerator::GenerateKnots(const SurfaceDimension& dimension, double* calculation_time)
{
	StopWatch sw;
	sw.Start();
	KnotVector knots(dimension.knot_count);
	InitializeKnots(dimension, knots);
	auto dfirst = function_.Dx()(dimension.min, 0);
	auto dlast = function_.Dx()(dimension.max, 0);
	auto rhs = RightSide(knots, abs(dimension.max - dimension.min) / (dimension.knot_count - 1), dfirst, dlast);

	utils::SolveDeboorTridiagonalSystem(1, 4, 1, &rhs.front(), rhs.size());

	KnotVector result(knots.size());
	result[0] = dfirst;
	result[result.size() - 1] = dlast;
	memcpy(&result.front() + 1, &rhs.front(), rhs.size());
	sw.Stop();
	if (calculation_time != nullptr)
	{
		*calculation_time = sw.EllapsedTime();
	}
	return result;
}
