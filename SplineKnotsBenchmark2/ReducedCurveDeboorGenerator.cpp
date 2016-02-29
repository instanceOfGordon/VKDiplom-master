#include "stdafx.h"
#include "ReducedCurveDeboorGenerator.h"
#include "utils.h"
#include "StopWatch.h"


splineknots::ReducedCurveDeboorKnotsGenerator::ReducedCurveDeboorKnotsGenerator
(const splineknots::MathFunction& function, bool buffered)
	: function_(function), is_buffered_(buffered)
{
}

splineknots::ReducedCurveDeboorKnotsGenerator::ReducedCurveDeboorKnotsGenerator
(const splineknots::InterpolativeMathFunction& function, bool buffered) 
	: function_(function), is_buffered_(buffered)
{
}

splineknots::KnotVector splineknots::ReducedCurveDeboorKnotsGenerator::
RightSide(const splineknots::KnotVector& function_values, double h, 
	double dfirst, double dlast)
{
	int n1, n2, N = function_values.size();
	n1 = (N % 2 == 0) ? (N - 2) / 2 : (N - 3) / 2;
	n2 = (N % 2 == 0) ? (N - 2) : (N - 3);

	splineknots::KnotVector rhs(n1);
	double mu1 = 3.0 / h;
	double mu2 = 4.0 * mu1;
	int k = -1;
	for (int i = 2; i < n2; i+=2)
	{
		++k;
		rhs[k] = mu1 * (function_values[i + 1] - function_values[i - 2]) 
			- mu2 * (function_values[i + 1] - function_values[i - 1]);
	}
	rhs[0] -= dfirst;
	rhs[n1 - 1] -= dlast;
	return rhs;
}

void splineknots::ReducedCurveDeboorKnotsGenerator::InitializeKnots(
	const SurfaceDimension& dimension, KnotVector& knots)
{
	auto h = abs(dimension.max - dimension.min) / (dimension.knot_count - 1);
	auto x = dimension.min;
	for (auto i = 0; i < dimension.knot_count; i++, x += h)
	{
		auto y = function_.Z()(x, 0);
		knots[i] = y;
	}
}

splineknots::KnotVector splineknots::ReducedCurveDeboorKnotsGenerator::
GenerateKnots(const splineknots::SurfaceDimension& dimension, 
	double* calculation_time)
{
	StopWatch sw;
	sw.Start();
	splineknots::KnotVector knots(dimension.knot_count);
	InitializeKnots(dimension, knots);
	auto dfirst = function_.Dx()(dimension.min, 0);
	auto dlast = function_.Dx()(dimension.max, 0);
	auto h = abs(dimension.max - dimension.min) / (dimension.knot_count - 1);
	auto rhs = RightSide(knots, h, dfirst, dlast);
	if (is_buffered_)
	{
		utils::SolveDeboorTridiagonalSystem(1, -14, 1, &rhs.front(), 
			rhs.size(), -15);
	}
	else
	{
		auto res = utils::SolveCsabaDeboorTridiagonalSystem(-14, &rhs.front(),
			rhs.size(),-15);
		rhs = std::move(res);
	}
	
	KnotVector result(knots.size());
	result[0] = dfirst;
	result[result.size() - 1] = dlast;

	for (int i = 0; i < rhs.size(); i++)
	{
		result[2 * (i + 1)] = rhs[i];
	}
	auto mu1 = 3.0 / h;
	for (int i = 1; i < result.size(); i += 2)
	{
		result[i] = 0.25 * (mu1 * (knots[i + 1] - knots[i - 1]) - 
			result[i - 1]
			- result[i + 1]);
	}
	sw.Stop();
	if (calculation_time != nullptr)
	{
		*calculation_time = sw.EllapsedTime();
	}
	return result;
}
