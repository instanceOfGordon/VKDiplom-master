#pragma once
#include "stdafx.h"
#include <vector>


namespace splineknots
{
	class Tridiagonal final
	{
		std::vector<double> lu_buffer_;
		std::vector<double> right_side_buffer_;		
		const double main_diagonal_value;
		bool buffered_;
	public:
		friend class ReducedDeboorTridiagonal;
		Tridiagonal(double main_value, bool buffered = true);

		void ResizeBuffers(size_t newsize, bool shrinking_allowed = false);

		void Solve(size_t num_unknowsns);
		double* RightSideBuffer();
		size_t RightSideBufferSize() const;
		void ResizeBuffer(size_t newsize, bool shrinking_allowed = false);
		void ResizeRightSide(size_t newsize, bool shrinking_allowed = false);
		double* ResetBufferAndGet();
		double* Buffer();
		size_t BufferSize() const;
		const double& MainDiagonalValue() const;
		bool IsBuffered() const;
	};
}