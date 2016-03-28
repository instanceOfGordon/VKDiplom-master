#include "stdafx.h"
#include "Tridiagonal.h"
#include "utils.h"
#include "ReducedDeboorTridiagonal.h"
#include <algorithm>

splineknots::Tridiagonal* splineknots::Tridiagonal::Clone() const
{
	return new Tridiagonal(*this);
}

splineknots::Tridiagonal::Tridiagonal(double lower_value, double main_value, double upper_value)
	:lu_buffer_(),
	right_side_buffer_(),
	lower_diagonal_value(lower_value),
	main_diagonal_value(main_value),
	upper_diagonal_value(upper_value)

{
}


void splineknots::Tridiagonal::ResizeBuffers(size_t newsize, bool shrinking_allowed)
{
	ResizeBuffer(newsize, shrinking_allowed);
	ResizeRightSide(newsize, shrinking_allowed);
}

void splineknots::Tridiagonal::ResizeBuffer(size_t newsize, bool shrinking_allowed)
{
	auto oldsize = lu_buffer_.size();
	if (newsize > oldsize)
	{
		lu_buffer_.reserve(newsize);
	}
	else if (shrinking_allowed)
	{
		lu_buffer_.resize(newsize);
	}
	for (size_t i = oldsize; i < newsize; i++)
	{
		lu_buffer_.push_back(upper_diagonal_value);
	}
}

void splineknots::Tridiagonal::ResizeRightSide(size_t newsize, bool shrinking_allowed)
{
	auto oldsize = right_side_buffer_.size();
	if (newsize > oldsize || shrinking_allowed)
	{
		right_side_buffer_.resize(newsize);
	}

}

double* splineknots::Tridiagonal::ResetBufferAndGet()
{
	auto& buffer = lu_buffer_;
	std::fill(buffer.begin(), buffer.end(), upper_diagonal_value);
	return &buffer.front();
}

double* splineknots::Tridiagonal::ResultArray()
{
	return &lu_buffer_.front();
}

size_t splineknots::Tridiagonal::ResultArraySize() const
{
	return lu_buffer_.size();
}

void splineknots::Tridiagonal::Solve(size_t num_unknowns)
{
	auto resize = std::max(num_unknowns, right_side_buffer_.size());
	auto minsize = std::min(ResultArraySize(), RightSideBufferSize());
	if (resize > minsize)
		ResizeBuffers(resize);
	auto buffer = ResultArray();
	utils::SolveDeboorTridiagonalSystemBuffered(lower_diagonal_value, main_diagonal_value, upper_diagonal_value, &right_side_buffer_.front(), num_unknowns, buffer);
}

double* splineknots::Tridiagonal::RightSideBuffer()
{
	return &right_side_buffer_.front();
}

size_t splineknots::Tridiagonal::RightSideBufferSize() const
{
	return right_side_buffer_.size();
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