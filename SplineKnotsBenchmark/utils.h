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
};

#include "utils_template.cpp"