#pragma once
#include "Knot.h"
namespace splineknots {
	
	class KnotMatrix
	{
		size_t rows_count_;
		size_t columns_count_;

		Knot** matrix_;

	public:
		
		KnotMatrix(size_t rows_, size_t columns_);
		KnotMatrix(const KnotMatrix& other);
		KnotMatrix(KnotMatrix&& other);
		KnotMatrix& operator =(const KnotMatrix& other);
		KnotMatrix& operator =(KnotMatrix&& other);
		~KnotMatrix() noexcept;
		size_t RowsCount() const;

		size_t ColumnsCount() const;
		//Knot& GetAt(int i, int j);
		Knot& operator()(int i, int j);
		const Knot& operator()(int i, int j) const;
		//friend void swap(KnotMatrix& left, KnotMatrix& right);
		Knot*& operator[](size_t k) const;
	
		
	};
}

