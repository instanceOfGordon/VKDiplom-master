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

splineknots::ReducedDeBoorTridiagonal::~ReducedDeBoorTridiagonal()
{
}

void splineknots::ReducedDeBoorTridiagonal::Solve(size_t num_unknowns)
{
	auto even = num_unknowns % 2 == 0;
	auto num_equations = even ? num_unknowns / 2 - 1 : num_unknowns / 2;
	auto rightside = RightSideBuffer();
	auto resize = std::max(num_equations, RightSideBufferSize());
	auto minsize = std::min(BufferSize(), RightSideBufferSize());
	if (resize > minsize)
		ResizeBuffers(resize);
	double last_maindiag_value = even ? -15 : -14;
	auto buffer = Buffer();
	utils::SolveDeboorTridiagonalSystemBuffered(LowerDiagonalValue(), MainDiagonalValue(), UpperDiagonalValue(), rightside, num_equations, buffer, last_maindiag_value);
}