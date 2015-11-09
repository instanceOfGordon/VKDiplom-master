#include "stdafx.h"
//#include "utils.h"


namespace utils
{
	/*template<typename T>
	T* utils::InitArray(size_t length, T* arrayToInit, T& value)
	{
		for (size_t i = 0; i < length; i++)
		{
			arrayToInit[i] = std::copy(value);
		}
		return arrayToInit;
	}*/


	template <typename T>
	T* InitArray(size_t length, T* arrayToInit, T value)
	{
		for (size_t i = 0; i < length; i++)
		{
			arrayToInit[i] = std::copy(value);
		}
		return arrayToInit;
	}

	template<typename T>
	void DeleteJaggedArray(T**& jaggedArray, size_t rows, size_t columns)
	{
		for (size_t i = 0; i < rows; i++)
		{
			delete[] jaggedArray[i];
			jaggedArray[i] = nullptr;
		}
		delete[] jaggedArray;
		jaggedArray = nullptr;
	}

	template <typename T>
	T** CreateJaggedArray(size_t rows, size_t columns)
	{
		auto res = new T*[rows];
		for (size_t i = 0; i < columns; i++)
		{
			res[i] = new T[columns];
		}
		
		return res;
	}

	template<typename T>
	double Average(T* arr, size_t arr_size)
	{
		T sum =0;
		for (size_t i = 0; i < arr_size; i++)
		{
			sum += arr[i];
		}
		return static_cast<double>(sum) / arr_size;
	}
}
