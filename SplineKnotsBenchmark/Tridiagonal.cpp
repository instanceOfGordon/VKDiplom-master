#include "stdafx.h"
#include "Tridiagonal.h"
#include "utils.h"
#include "ReducedDeboorTridiagonal.h"

splineknots::Tridiagonal* splineknots::Tridiagonal::Clone() const
{
	return new Tridiagonal(*this);
}

splineknots::Tridiagonal::Tridiagonal(double lower_value, double main_value, double upper_value)
	:lu_buffer_(),
	 lower_diagonal_value(lower_value),
	 main_diagonal_value(main_value),
	 upper_diagonal_value(upper_value)

{
}


splineknots::Tridiagonal::~Tridiagonal()
{
}

void splineknots::Tridiagonal::ResizeBuffer(size_t newsize)
{
	if (newsize > lu_buffer_.size())
	{
		lu_buffer_.reserve(newsize);
	}
	for (size_t i = lu_buffer_.size(); i < newsize; i++)
	{
		lu_buffer_.push_back(upper_diagonal_value);
	}
}

double* splineknots::Tridiagonal::ResetBufferAndGet()
{
	auto& buffer = lu_buffer_;
	std::fill(buffer.begin(), buffer.end(), upper_diagonal_value);
	return &buffer.front();
}

double* splineknots::Tridiagonal::Buffer()
{
	return &lu_buffer_.front();
}

size_t splineknots::Tridiagonal::BufferElementCount() const
{
	return lu_buffer_.size();
}

void splineknots::Tridiagonal::Solve(size_t num_unknowns, double* right_side)
{
	if (num_unknowns > BufferElementCount())
		ResizeBuffer(num_unknowns);
	auto buffer = Buffer();
	utils::SolveDeboorTridiagonalSystemBuffered(lower_diagonal_value, main_diagonal_value, upper_diagonal_value, right_side, num_unknowns, buffer);
}


const double& splineknots::Tridiagonal::LowerDiagonalValue() const
{
	return lower_diagonal_value;
}

const double& splineknots::Tridiagonal::MainDiagonalValue() const
{
	return main_diagonal_value;
}

const double& splineknots::Tridiagonal::UpperDiagonalValue() const
{
	return upper_diagonal_value;
}
