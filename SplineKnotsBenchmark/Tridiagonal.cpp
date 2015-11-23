#include "stdafx.h"
#include "Tridiagonal.h"
#include "utils.h"

utils::Tridiagonal* utils::Tridiagonal::Clone() const
{
	return new Tridiagonal(*this);
}

utils::Tridiagonal::Tridiagonal(double lower_value, double main_value, double upper_value)
	:lu_buffer_(),
	lower_diagonal_value(lower_value),
	main_diagonal_value(main_value),
	upper_diagonal_value(upper_value)
	
{
}


utils::Tridiagonal::~Tridiagonal()
{
}

void utils::Tridiagonal::ResizeBuffer(size_t newsize)
{
	if (newsize > lu_buffer_.size()) {
		lu_buffer_.reserve(newsize);
		
	}
	for (size_t i = lu_buffer_.size(); i < newsize; i++)
	{
		lu_buffer_.push_back(upper_diagonal_value);
	}
	

}

double* utils::Tridiagonal::ResetBufferAndGet()
{
	auto& buffer = lu_buffer_;
	std::fill(buffer.begin(), buffer.end(), upper_diagonal_value);
	return &buffer.front();
}

double* utils::Tridiagonal::Buffer()
{
	return &lu_buffer_.front();
}

size_t utils::Tridiagonal::BufferElementCount() const
{
	return lu_buffer_.size();
}

void utils::Tridiagonal::Solve(size_t num_unknowns, double* right_side)
{
	//auto& buffer = *lu_buffer_;
	if (num_unknowns > BufferElementCount())
		ResizeBuffer(num_unknowns);
	auto buffer =Buffer();
	//utils::SolveTridiagonalSystem(&lower_diagonal_->front(), &main_diagonal_->front(), &upper_diagonal_->front(), right_side, num_unknowns);
	utils::SolveDeboorTridiagonalSystemBuffered(lower_diagonal_value, main_diagonal_value, upper_diagonal_value, right_side, num_unknowns,buffer);
}





const double& utils::Tridiagonal::LowerDiagonalValue() const
{
	return lower_diagonal_value;
}

const double& utils::Tridiagonal::MainDiagonalValue() const
{
	return main_diagonal_value;
}

const double& utils::Tridiagonal::UpperDiagonalValue() const
{
	return upper_diagonal_value;
}

utils::ReducedDeBoorTridiagonal* utils::ReducedDeBoorTridiagonal::Clone() const
{
	return new ReducedDeBoorTridiagonal(*this);
}

utils::ReducedDeBoorTridiagonal::ReducedDeBoorTridiagonal()
	: Tridiagonal(1,-14, 1)
{
}

utils::ReducedDeBoorTridiagonal::~ReducedDeBoorTridiagonal()
{
}

void utils::ReducedDeBoorTridiagonal::Solve(size_t num_unknowns, double* right_side)
{
	auto num_equations = (num_unknowns + 2) / 2 - 1;
	
	auto size = BufferElementCount();
	if (num_equations > size)
		ResizeBuffer(num_equations);
	double last_maindiag_value = num_unknowns % 2 == 0 ? -15 : -14;
	auto buffer = Buffer();
	utils::SolveDeboorTridiagonalSystemBuffered(LowerDiagonalValue(), MainDiagonalValue(), UpperDiagonalValue(), right_side, num_equations, buffer, last_maindiag_value);
}