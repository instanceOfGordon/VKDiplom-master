#include "stdafx.h"
#include <algorithm>
#include "SplineKnots.h"
#include "ReducedDeboorTridiagonal.h"
#include "Tridiagonal.h"


void splineknots::ReducedDeboorTridiagonal::Solve(size_t num_unknowns)
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
	if (tridiagonal_.IsBuffered())
	{
		utils::SolveDeboorTridiagonalSystemBuffered(LowerDiagonalValue(),
			MainDiagonalValue(), UpperDiagonalValue(), rightside, 
			num_equations,
			buffer, last_maindiag_value);
	}
	else
	{
		auto result = utils::SolveCsabaDeboorTridiagonalSystem(4, rightside,
			num_unknowns);
		memcpy(rightside, &result.front(), num_unknowns);
	}
}

splineknots::ReducedDeboorTridiagonal::ReducedDeboorTridiagonal(bool buffered)
	:tridiagonal_(1,-14,1,buffered)
{
}

void splineknots::ReducedDeboorTridiagonal::ResizeBuffers(size_t newsize, 
	bool shrinking_allowed)
{
	tridiagonal_.ResizeBuffers(newsize, shrinking_allowed);
}

double* splineknots::ReducedDeboorTridiagonal::RightSideBuffer()
{
	return tridiagonal_.RightSideBuffer();
}

size_t splineknots::ReducedDeboorTridiagonal::RightSideBufferSize() const
{
	return tridiagonal_.RightSideBufferSize();
}

void splineknots::ReducedDeboorTridiagonal::ResizeBuffer(size_t newsize, 
	bool shrinking_allowed)
{
	tridiagonal_.ResizeBuffer(newsize, shrinking_allowed);
}

void splineknots::ReducedDeboorTridiagonal::ResizeRightSide(size_t newsize, 
	bool shrinking_allowed)
{
	tridiagonal_.ResizeRightSide(newsize, shrinking_allowed);
}

double* splineknots::ReducedDeboorTridiagonal::ResetBufferAndGet()
{
	return tridiagonal_.ResetBufferAndGet();
}

double* splineknots::ReducedDeboorTridiagonal::Buffer()
{
	return tridiagonal_.Buffer();
}

size_t splineknots::ReducedDeboorTridiagonal::BufferSize() const
{
	return tridiagonal_.BufferSize();
}

const double& splineknots::ReducedDeboorTridiagonal::LowerDiagonalValue() const
{
	return tridiagonal_.LowerDiagonalValue();
}

const double& splineknots::ReducedDeboorTridiagonal::MainDiagonalValue() const
{
	return tridiagonal_.MainDiagonalValue();
}

const double& splineknots::ReducedDeboorTridiagonal::UpperDiagonalValue() const
{
	return tridiagonal_.UpperDiagonalValue();
}