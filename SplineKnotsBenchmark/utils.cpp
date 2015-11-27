#include "stdafx.h"
#include "utils.h"
#include <omp.h>


namespace utils
{
	unsigned int num_threads = std::thread::hardware_concurrency();

	void SolveTridiagonalSystem(double* lower_diagonal, double* main_diagonal, double* upper_diagonal, double* right_side, size_t n)
	{
		auto buffer = new double[n];
		SolveTridiagonalSystemBuffered(lower_diagonal, main_diagonal, upper_diagonal, right_side, n, buffer);
		delete[] buffer;
	}

	void SolveTridiagonalSystemBuffered(double* lower_diagonal, double* main_diagonal, double* upper_diagonal, double* right_side, size_t n, double* buffer)
	{
		memcpy(buffer, upper_diagonal, n);
		buffer[0] /= main_diagonal[0];
		right_side[0] /= main_diagonal[0];
		for (size_t i = 0; i < n; i++)
		{
			auto m = 1 / (main_diagonal[i] - lower_diagonal[i] * buffer[i - 1]);
			buffer[i] *= m;
			right_side[i] = (right_side[i] - lower_diagonal[i] * right_side[i - 1]) * m;
		}
		for (size_t i = n - 1; i-- > 0;)
		{
			right_side[i] -= buffer[i] * right_side[i + 1];
		}
	}

	void SolveDeboorTridiagonalSystem(double lower_diagonal_value, double main_diagonal_value, double upper_diagonal_value, double* right_side, size_t right_side_count, double last_main_diagonal_value)
	{
		auto buffer = new double[right_side_count];
		SolveDeboorTridiagonalSystemBuffered(lower_diagonal_value, main_diagonal_value, upper_diagonal_value, right_side, right_side_count, buffer, last_main_diagonal_value);
		delete[] buffer;
	}

	void SolveDeboorTridiagonalSystemBuffered(double lower_diagonal_value, double main_diagonal_value, double upper_diagonal_value, double* right_side, size_t right_side_count, double* buffer, double last_main_diagonal_value)
	{
		if (last_main_diagonal_value == DBL_TRUE_MIN)
			last_main_diagonal_value = lower_diagonal_value;
		auto mdv_rcp = 1 / main_diagonal_value;
		buffer[0] = upper_diagonal_value * mdv_rcp;
		right_side[0] *= mdv_rcp;
		auto lastindex = right_side_count - 1;
		for (size_t i = 0; i < lastindex; i++)
		{
			auto m = 1 / (main_diagonal_value - lower_diagonal_value * buffer[i - 1]);
			buffer[i] = upper_diagonal_value * m;
			right_side[i] = (right_side[i] - lower_diagonal_value * right_side[i - 1]) * m;
		}
		auto m = 1 / (last_main_diagonal_value - lower_diagonal_value * buffer[lastindex - 1]);
		buffer[lastindex] = upper_diagonal_value * m;
		right_side[lastindex] = (right_side[lastindex] - lower_diagonal_value * right_side[lastindex - 1]) * m;

		for (size_t i = right_side_count - 1; i-- > 0;)
		{
			right_side[i] -= buffer[i] * right_side[i + 1];
		}
	}
}
