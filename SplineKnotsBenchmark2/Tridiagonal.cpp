#include "stdafx.h"
#include "Tridiagonal.h"
#include "utils.h"
#include <algorithm>


splineknots::Tridiagonal::Tridiagonal( double main_value,
	bool buffered)
	:lu_buffer_(),
	right_side_buffer_(),
	main_diagonal_value(main_value),
	buffered_(buffered)

{
}

void splineknots::Tridiagonal::ResizeBuffers(size_t newsize, bool
	shrinking_allowed)
{
	if (buffered_)
		ResizeBuffer(newsize, shrinking_allowed);
	ResizeRightSide(newsize, shrinking_allowed);
}

void splineknots::Tridiagonal::ResizeBuffer(size_t newsize,
	bool shrinking_allowed)
{
	if (!buffered_) return;
	auto oldsize = lu_buffer_.size();
	if (newsize > oldsize)
	{
		lu_buffer_.reserve(newsize);
	}
	else if (shrinking_allowed)
	{
		lu_buffer_.resize(newsize,1);
	}
	
}

void splineknots::Tridiagonal::ResizeRightSide(size_t newsize, bool
	shrinking_allowed)
{
	auto oldsize = right_side_buffer_.size();
	if (newsize > oldsize || shrinking_allowed)
	{
		right_side_buffer_.resize(newsize);
	}
}


double* splineknots::Tridiagonal::ResetBufferAndGet()
{
	if (!buffered_) return nullptr;
	auto& buffer = lu_buffer_;
	std::fill(buffer.begin(), buffer.end(), 1);
	return &buffer.front();
}

double* splineknots::Tridiagonal::Buffer()
{
	if (!buffered_) return nullptr;
	return &lu_buffer_.front();
}

size_t splineknots::Tridiagonal::BufferSize() const
{
	return lu_buffer_.size();
}

void splineknots::Tridiagonal::Solve(size_t num_unknowns)
{
	auto resize = std::max(num_unknowns, right_side_buffer_.size());
	auto minsize = std::min(BufferSize(), RightSideBufferSize());
	if (resize > minsize)
		ResizeBuffers(resize);
	auto buffer = Buffer();
	if (buffered_)
	{
		utils::SolveDeboorTridiagonalSystemBuffered(
			 main_diagonal_value, 
			&right_side_buffer_.front(), num_unknowns, buffer);
	}
	else
	{
		auto result = utils::SolveCsabaDeboorTridiagonalSystem(4,
			&right_side_buffer_.front(), num_unknowns);
		right_side_buffer_ = std::move(result);

	}
}

double* splineknots::Tridiagonal::RightSideBuffer()
{
	return &right_side_buffer_.front();
}

size_t splineknots::Tridiagonal::RightSideBufferSize() const
{
	return right_side_buffer_.size();
}

const double& splineknots::Tridiagonal::MainDiagonalValue() const
{
	return main_diagonal_value;
}

bool splineknots::Tridiagonal::IsBuffered() const
{
	return buffered_;
}