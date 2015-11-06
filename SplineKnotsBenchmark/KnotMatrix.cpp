#include "stdafx.h"
#include "KnotMatrix.h"
#include "utils.h"
//#include "utils_template.cpp"


splineknots::KnotMatrix::~KnotMatrix() noexcept
{
	/*for (size_t i = 0; i < rowsCount_; i++)
		{
			delete[] matrix_[i];
		}
		delete[] matrix_;
		matrix_ = nullptr;*/
	utils::DeleteJaggedArray(matrix_, rows_count_, columns_count_);
}

size_t splineknots::KnotMatrix::RowsCount() const
{
	return rows_count_;
}

size_t splineknots::KnotMatrix::ColumnsCount() const
{
	return columns_count_;
}


splineknots::Knot& splineknots::KnotMatrix::operator()(int i, int j)
{
	return matrix_[i][j];
}

const splineknots::Knot& splineknots::KnotMatrix::operator()(int i, int j) const
{
	return matrix_[i][j];
}

splineknots::Knot*& splineknots::KnotMatrix::operator[](size_t k) const
{
	return matrix_[k];
}

splineknots::KnotMatrix::KnotMatrix(size_t rows, size_t columns)
	: rows_count_(rows),
	  columns_count_(columns)
//matrix_(new double*[rows])
{
	matrix_ = new splineknots::Knot*[rows];
	for (size_t i = 0; i < rows; i++)
	{
		matrix_[i] = new splineknots::Knot[columns];
	}
}

splineknots::KnotMatrix::KnotMatrix(const KnotMatrix& other)
	: rows_count_(other.rows_count_),
	  columns_count_(other.columns_count_)
{
	matrix_ = new Knot*[rows_count_];
	for (size_t i = 0; i < other.RowsCount(); i++)
	{
		matrix_[i] = new Knot[columns_count_];
		memcpy(matrix_[i], other.matrix_[i], columns_count_);
	}
	/*for (size_t i = 0; i < rowsCount_; i++)
		{
			delete[] matrix_[i];
		}
		delete[] matrix_;
		matrix_ = nullptr;
		rowsCount_ = other.getRowsCount();
		columnsCount_ = other.getColumnsCount();
		matrix_ = new Knot*[rowsCount_];
		for (size_t i = 0; i < other.getRowsCount(); i++)
		{
			matrix_[i] = new Knot[columnsCount_];
			memcpy(matrix_[i], other.matrix_[i], columnsCount_);
		}*/
}

splineknots::KnotMatrix::KnotMatrix(KnotMatrix&& other)
	: rows_count_(other.rows_count_),
	  columns_count_(other.columns_count_)
{
	matrix_ = other.matrix_;
	other.matrix_ = nullptr;
	other.rows_count_ = 0;
	other.columns_count_ = 0;
}

splineknots::KnotMatrix& splineknots::KnotMatrix::operator=(const KnotMatrix& other)
{
	if (&other != this)
	{
		utils::DeleteJaggedArray(matrix_, rows_count_, columns_count_);
		rows_count_ = other.rows_count_;
		columns_count_ = other.columns_count_;
		matrix_ = utils::CreateJaggedArray<Knot>(rows_count_, columns_count_);
		for (size_t i = 0; i < rows_count_; i++)
		{
			memcpy(matrix_[i], other.matrix_[i], columns_count_);
		}
	}
	return *this;
}

splineknots::KnotMatrix& splineknots::KnotMatrix::operator=(KnotMatrix&& other)
{
	if (&other != this)
	{
		utils::DeleteJaggedArray(matrix_, rows_count_, columns_count_);
		rows_count_ = other.rows_count_;
		columns_count_ = other.columns_count_;
		matrix_ = other.matrix_;
		other.matrix_ = nullptr;
		other.rows_count_ = 0;
		other.columns_count_ = 0;
	}
	return *this;
}
