#include "stdafx.h"
#include "Tridiagonal.h"
#include "utils.h"


//Tridiagonal& Tridiagonal::Instance()
//{
//	static Tridiagonal instance;
//	return instance;
//}

utils::Tridiagonal::Tridiagonal(double lower_value, double main_value, double upper_value)
	:lower_diagonal_(std::make_unique<std::vector<double>>(kInitCount,lower_value)),
	main_diagonal_(std::make_unique<std::vector<double>>(kInitCount, main_value)),
	upper_diagonal_(std::make_unique<std::vector<double>>(kInitCount, upper_value)),
	lu_buffer_(std::make_unique<std::vector<double>>(kInitCount, upper_value)),
	lower_diagonal_value(lower_value),
	main_diagonal_value(main_value),
	upper_diagonal_value(upper_value)
	
{
}


utils::Tridiagonal::~Tridiagonal()
{
}

void utils::Tridiagonal::Resize(size_t newsize)
{
	if (newsize > main_diagonal_->size()) {
		lower_diagonal_->reserve(newsize);
		main_diagonal_->reserve(newsize);
		upper_diagonal_->reserve(newsize);
	}
	for (size_t i = lower_diagonal_->size(); i < newsize; i++)
	{
		lower_diagonal_->push_back(lower_diagonal_value);
		main_diagonal_->push_back(main_diagonal_value);
		upper_diagonal_->push_back(upper_diagonal_value);
	}
	/*if(newsize>main_diagonal_->size())
	{
		main_diagonal_->push_back(main_diagonal_value);
	}*/

}



void utils::Tridiagonal::Solve(size_t num_unknowns, double* right_side)
{
	if (num_unknowns > main_diagonal_->size())
		Resize(num_unknowns);
	utils::SolveTridiagonalSystem(&lower_diagonal_->front(), &main_diagonal_->front(), &upper_diagonal_->front(), right_side, num_unknowns);

}

const std::vector<double>& utils::Tridiagonal::LowerDiagonal() const
{
	return *lower_diagonal_;
}

const std::vector<double>& utils::Tridiagonal::Buffer()
{
	memcpy(&lu_buffer_->front(), &upper_diagonal_->front(), lu_buffer_->size());
	return *lu_buffer_;
}

const std::vector<double>& utils::Tridiagonal::MainDiagonal() const
{
	return *main_diagonal_;
}

const std::vector<double>& utils::Tridiagonal::UpperDiagonal() const
{
	return *upper_diagonal_;
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
	
	//auto size = 
	if (num_equations > MainDiagonal().size())
		Resize(num_equations);
	auto maindiag = MainDiagonal();
	if(num_equations != maindiag.size())
	{
		for (size_t i = 0; i < num_equations-1; i++)
		{
			maindiag[i] = -14;
		}
	}
	auto lastidx = MainDiagonal().size()-1;
	maindiag[lastidx] = num_unknowns % 2 == 0 ? -15 : -14;
	auto ld = LowerDiagonal();
	auto ud = UpperDiagonal();
	auto buffer = Buffer();
	//utils::SolveTridiagonalSystem(&ld.front(), &maindiag.front(), &ud.front(), right_side, num_equations);
	utils::SolveTridiagonalSystemBuffered(&ld.front(), &maindiag.front(), &ud.front(), right_side, num_equations,&buffer.front());
}