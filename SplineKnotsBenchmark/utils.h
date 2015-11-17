#pragma once

namespace utils
{

	template<typename T>
	T* InitArray(size_t length, T* arrayToInit, T value);

	template<typename T>
	void DeleteJaggedArray(T**& jaggedArray, size_t rows, size_t columns);

	template<typename T>
	T** CreateJaggedArray(size_t rows, size_t columns);

	void SolveTridiagonalSystem(double* lower_diagonal, double* main_diagonal,
		double* upper_diagonal, double* right_side, size_t n);

	void SolveTridiagonalSystemBuffered(double* lower_diagonal, double* main_diagonal,
		double* upper_diagonal, double* right_side, size_t n, double* buffer);

	void SolveDeboorTridiagonalSystem(double lower_diagonal_value, double main_diagonal_value,
		double upper_diagonal_value, double* right_side, size_t right_side_count,
		double last_main_diagonal_value = DBL_TRUE_MIN);

	void SolveDeboorTridiagonalSystemBuffered(double lower_diagonal_value, double main_diagonal_value,
		double upper_diagonal_value, double* right_side, size_t right_side_count, double* buffer, 
		double last_main_diagonal_value = DBL_TRUE_MIN);


	template<typename T>
	double Average(T* arr, size_t arr_size);
};

#include "utils_template.cpp"