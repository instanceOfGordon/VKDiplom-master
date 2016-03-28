#pragma once
#include "stdafx.h"
#include <vector>
#include "Tridiagonal.h"


namespace splineknots
{
	class ReducedDeboorTridiagonal final
	{
		//friend class Tridiagonal;
		Tridiagonal tridiagonal_;
	public:

		ReducedDeboorTridiagonal(bool buffered = true);

		void ResizeBuffers(size_t newsize, bool shrinking_allowed = false);

		void Solve(size_t num_unknowsns);
		double* RightSideBuffer();
		size_t RightSideBufferSize() const;

	private:
		void ResizeBuffer(size_t newsize, bool shrinking_allowed = false);
		void ResizeRightSide(size_t newsize, bool shrinking_allowed = false);
	public:
		double* ResetBufferAndGet();
		double* Buffer();
		size_t BufferSize() const;
		const double& LowerDiagonalValue() const;
		const double& MainDiagonalValue() const;
		const double& UpperDiagonalValue() const;
	};
}