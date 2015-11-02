#pragma once
#include "Knot.h"
#include "KnotMatrix.h"
namespace splineknots {

	template<size_t rows, size_t columns>
	class StaticKnotMatrix : public AKnotMatrix
	{
		Knot matrix_[rows][columns];

	public:
		//StaticKnotMatrix(size_t rows,size_t columns);
		StaticKnotMatrix();
		~StaticKnotMatrix();
		Knot& getAt(int i, int j) override;
	};

}
