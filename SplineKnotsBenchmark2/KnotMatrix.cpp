#include "stdafx.h"
#include "KnotMatrix.h"
#include "utils.h"
#include <iostream>

//#include "utils_template.cpp"


splineknots::KnotMatrix::~KnotMatrix() noexcept
{
	for (size_t i = 0; i < rows_count_; i++)
	{
		delete[] matrix_[i];
	}
	delete[] matrix_;
	matrix_ = nullptr;
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

void splineknots::KnotMatrix::Print()
{
	using namespace std;
	cout << "---------- Knot matrix ----------" << endl;
	for (size_t i = 0; i < rows_count_; i++)
	{
		cout << "Row " << i << " :\n";
		for (size_t j = 0; j < columns_count_; j++)
		{
			cout << j << ":\n"
				<< "x: " << matrix_[i][j].X() << '\n'
				<< "y: " << matrix_[i][j].Y() << '\n'
				<< "z: " << matrix_[i][j].Z() << '\n'
				<< "dx: " << matrix_[i][j].Dx() << '\n'
				<< "dy: " << matrix_[i][j].Dy() << '\n'
				<< "dxy: " << matrix_[i][j].Dxy() << '\n';
		}
		cout << endl;
	}
	cout << "-------------------------------" << endl;
}

splineknots::KnotMatrix::KnotMatrix()
	:rows_count_(0),
	 columns_count_(0),
	 matrix_(nullptr)
{
}

splineknots::KnotMatrix splineknots::KnotMatrix::NullMatrix()
{
	KnotMatrix nullval;
	return nullval;
}

bool splineknots::KnotMatrix::IsNull()
{
	if (matrix_ || rows_count_ < 1 || columns_count_ < 1)
		return true;
	return false;
}

splineknots::KnotMatrix::KnotMatrix(size_t rows, size_t columns)
	: rows_count_(rows),
	  columns_count_(columns)
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
