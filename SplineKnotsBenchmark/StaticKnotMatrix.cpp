#include "stdafx.h"

#include "StaticKnotMatrix.h"
namespace splineknots {
	template <size_t rows, size_t columns>
	StaticKnotMatrix<rows, columns>::StaticKnotMatrix()
		: KnotMatrix(rows, columns)
	{
	}

	template <size_t rows, size_t columns>
	StaticKnotMatrix<rows, columns>::~StaticKnotMatrix()
	{
	}

	template <size_t rows, size_t columns>
	Knot& StaticKnotMatrix<rows, columns>::getAt(int i, int j)
	{
		return matrix_[i][j];
	}

	//template <size_t rows, size_t columns>
	//Knot* StaticKnotMatrix<rows, columns>::operator[](int i)
	//{
	//	return matrix_[i];
	//}
}