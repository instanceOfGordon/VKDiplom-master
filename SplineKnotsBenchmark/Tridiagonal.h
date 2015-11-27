#pragma once
#include "stdafx.h"
#include <vector>
#include "Cloneable.h"

namespace splineknots
{
	class Tridiagonal : public Cloneable
	{
	public:
		Tridiagonal* Clone() const override;
		Tridiagonal(double lower_value, double main_value, double upper_value);
		virtual ~Tridiagonal();
		void ResizeBuffer(size_t newsize);
		virtual void Solve(size_t num_unknowns, double* right_side);
	private:
		std::vector<double> lu_buffer_;
		const double lower_diagonal_value;
		const double main_diagonal_value;
		const double upper_diagonal_value;
	protected:
		double* ResetBufferAndGet();
		double* Buffer();
		size_t BufferElementCount() const;
		const double& LowerDiagonalValue() const;
		const double& MainDiagonalValue() const;
		const double& UpperDiagonalValue() const;
	};
}
