#pragma once
#include "Knot.h"

namespace splineknots
{
	class Row;
	class KnotMatrix
	{
		
		size_t rows_count_;
		size_t columns_count_;
		Knot** matrix_;
		KnotMatrix();
	public:
		static KnotMatrix NullMatrix();
		bool IsNull();
		KnotMatrix(size_t rows_, size_t columns_);
		KnotMatrix(const KnotMatrix& other);
		KnotMatrix(KnotMatrix&& other);
		KnotMatrix& operator =(const KnotMatrix& other);
		KnotMatrix& operator =(KnotMatrix&& other);
		~KnotMatrix() noexcept;
		size_t RowsCount() const;
		size_t ColumnsCount() const;
		Knot& operator()(int i, int j);
		const Knot& operator()(int i, int j) const;
		Knot*& operator[](size_t k) const;
		void Print();
	};
}