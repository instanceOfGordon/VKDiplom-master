#pragma once
namespace utils
{

	template<typename T>
	T* initArray(size_t length, T* arrayToInit, T value);

	template<typename T>
	void DeleteJaggedArray(T**& jaggedArray, size_t rows, size_t columns);

	template<typename T>
	T** CreateJaggedArray(size_t rows, size_t columns);
};

