#include "stdafx.h"
#include "utils.h"


namespace utils
{

	void SolveTridiagonalSystem(double* lower_diagonal, double* main_diagonal, double* upper_diagonal, double* right_side, size_t n)
	{
		auto upper_diagonal_copy = new double[n - 1];
		memcpy(upper_diagonal_copy, upper_diagonal, n - 1);
		upper_diagonal_copy[0] /= main_diagonal[0];
		right_side[0] /= main_diagonal[0];
		for (size_t i = 0; i < n; i++)
		{
			auto m = 1 / (main_diagonal[i] - lower_diagonal[i] * upper_diagonal_copy[i - 1]);
			upper_diagonal_copy[i] *= m;
			right_side[i] = (right_side[i] - lower_diagonal[i] * right_side[i - 1])*m;
		}
		for (size_t i = n - 1; i-- > 0;)
		{
			right_side[i] -= upper_diagonal_copy[i] * right_side[i + 1];
		}
	}
}