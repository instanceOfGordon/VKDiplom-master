#pragma once
#include "stdafx.h"
#include <vector>


namespace splineknots
{
	class Tridiagonal final
	{
		
	public:	
		friend class ReducedDeboorTridiagonal;
		Tridiagonal(double lower_value, double main_value, double upper_value);

		void ResizeBuffers(size_t newsize, bool shrinking_allowed = false);
		
		void Solve(size_t num_unknowsns);
		double* RightSideBuffer();
		size_t RightSideBufferSize() const;
	private:
		std::vector<double> lu_buffer_;
		std::vector<double> right_side_buffer_;
		const double lower_diagonal_value;
		const double main_diagonal_value;
		const double upper_diagonal_value;
	public:
		void ResizeBuffer(size_t newsize, bool shrinking_allowed = false);
		void ResizeRightSide(size_t newsize, bool shrinking_allowed = false);
		double* ResetBufferAndGet();
		double* Buffer();
		size_t BufferSize() const;
		const double& LowerDiagonalValue() const;
		const double& MainDiagonalValue() const;
		const double& UpperDiagonalValue() const;	
	};
}
