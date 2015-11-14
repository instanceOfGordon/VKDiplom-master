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
	auto maindiag = MainDiagonal();
	//auto size = 
	if (num_equations > MainDiagonal().size())
		Resize(num_equations);
	if(num_equations != MainDiagonal().size())
	{
		for (size_t i = 0; i < num_equations-1; i++)
		{
			maindiag[i] = -14;
		}
	}
	maindiag[num_equations -1] = num_unknowns % 2 == 0 ? -15 : -14;
	auto ld = LowerDiagonal();
	auto ud = UpperDiagonal();
	utils::SolveTridiagonalSystem(&ld.front(), &maindiag.front(), &ud.front(), right_side, num_equations);
}