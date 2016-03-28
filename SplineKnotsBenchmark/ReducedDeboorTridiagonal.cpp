#include "stdafx.h"
#include "SplineKnots.h"
#include "ReducedDeboorTridiagonal.h"
#include <algorithm>

splineknots::ReducedDeBoorTridiagonal* splineknots::ReducedDeBoorTridiagonal::Clone() const
{
	return new ReducedDeBoorTridiagonal(*this);
}

splineknots::ReducedDeBoorTridiagonal::ReducedDeBoorTridiagonal()
	: Tridiagonal(1, -14, 1)
{
}

void splineknots::ReducedDeBoorTridiagonal::Solve(size_t num_unknowns)
{
	auto even = num_unknowns % 2 == 0;
	auto num_equations = even ? num_unknowns / 2 - 1 : num_unknowns / 2;
	auto rightside = RightSideBuffer();
	auto resize = std::max(num_equations, RightSideBufferSize());
	auto minsize = std::min(ResultArraySize(), RightSideBufferSize());
	if (resize > minsize)
		ResizeBuffers(resize);
	double last_maindiag_value = even ? -15 : -14;
	auto buffer = ResultArray();
	utils::SolveDeboorTridiagonalSystemBuffered(LowerDiagonalValue(), MainDiagonalValue(), UpperDiagonalValue(), rightside, num_equations, buffer, last_maindiag_value);
}