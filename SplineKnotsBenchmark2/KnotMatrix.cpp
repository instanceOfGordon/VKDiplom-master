#include "stdafx.h"
#include "KnotMatrix.h"
#include "utils.h"
#include <iostream>

//#include "utils_template.cpp"


splineknots::KnotMatrix::~KnotMatrix() noexcept
{
	for (size_t i = 0; i < rows_count_; i++)
	{
		delete[] x_[i];
		delete[] y_[i];
		delete[] z_[i];
		delete[] dx_[i];
		delete[] dy_[i];
		delete[] dxy_[i];
	}
	delete[] x_;
	delete[] y_;
	delete[] z_;
	delete[] dx_;
	delete[] dy_;
	delete[] dxy_;
	x_ = nullptr;
	y_ = nullptr;
	z_ = nullptr;
	dx_ = nullptr;
	dy_ = nullptr;
	dxy_ = nullptr;
}

size_t splineknots::KnotMatrix::RowsCount() const
{
	return rows_count_;
}

size_t splineknots::KnotMatrix::ColumnsCount() const
{
	return columns_count_;
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
				<< "x: " << x_[i][j] << '\n'
				<< "y: " << y_[i][j] << '\n'
				<< "z: " << z_[i][j] << '\n'
				<< "dx: " << dx_[i][j] << '\n'
				<< "dy: " << dy_[i][j] << '\n'
				<< "dxy: " << dxy_[i][j] << '\n';
		}
		cout << endl;
	}
	cout << "-------------------------------" << endl;
}

splineknots::KnotMatrix::KnotMatrix()
	:rows_count_(0),
	 columns_count_(0),
	 x_(nullptr), y_(nullptr), z_(nullptr),
	 dx_(nullptr), dy_(nullptr), dxy_(nullptr)
{
}

splineknots::KnotMatrix splineknots::KnotMatrix::NullMatrix()
{
	KnotMatrix nullval;
	return nullval;
}

bool splineknots::KnotMatrix::IsNull()
{
	if (!x_ || !y_ || !z_ || !dx_ || !dy_ || !dxy_ || rows_count_ < 1 || columns_count_ < 1)
		return true;
	return false;
}

splineknots::KnotMatrix::KnotMatrix(size_t rows, size_t columns)
	: rows_count_(rows),
	  columns_count_(columns)
{
	x_ = new double*[rows];
	y_ = new double*[rows];
	z_ = new double*[rows];
	dx_ = new double*[rows];
	dy_ = new double*[rows];
	dxy_ = new double*[rows];
	for (size_t i = 0; i < rows; i++)
	{
		x_[i] = new double[columns];
		y_[i] = new double[columns];
		z_[i] = new double[columns];
		dx_[i] = new double[columns];
		dy_[i] = new double[columns];
		dxy_[i] = new double[columns];
	}
}

splineknots::KnotMatrix::KnotMatrix(const KnotMatrix& other)
	: rows_count_(other.rows_count_),
	  columns_count_(other.columns_count_)
{
	x_ = new double*[rows_count_];
	y_ = new double*[rows_count_];
	z_ = new double*[rows_count_];
	dx_ = new double*[rows_count_];
	dy_ = new double*[rows_count_];
	dxy_ = new double*[rows_count_];
	for (size_t i = 0; i < other.RowsCount(); i++)
	{
		x_[i] = new double[columns_count_];
		y_[i] = new double[columns_count_];
		z_[i] = new double[columns_count_];
		dx_[i] = new double[columns_count_];
		dy_[i] = new double[columns_count_];
		dxy_[i] = new double[columns_count_];
		memcpy(x_[i], other.x_[i], columns_count_);
		memcpy(y_[i], other.y_[i], columns_count_);
		memcpy(z_[i], other.z_[i], columns_count_);
		memcpy(dx_[i], other.dx_[i], columns_count_);
		memcpy(dy_[i], other.dy_[i], columns_count_);
		memcpy(dxy_[i], other.dxy_[i], columns_count_);
	}
}

splineknots::KnotMatrix::KnotMatrix(KnotMatrix&& other)
	: rows_count_(other.rows_count_),
	  columns_count_(other.columns_count_)
{
	x_ = other.x_;
	other.x_ = nullptr;
	y_ = other.y_;
	other.y_ = nullptr;
	z_ = other.z_;
	other.z_ = nullptr;
	dx_ = other.dx_;
	other.dx_ = nullptr;
	dy_ = other.dy_;
	other.dy_ = nullptr;
	dxy_ = other.dxy_;
	other.dxy_ = nullptr;
	other.rows_count_ = 0;
	other.columns_count_ = 0;
}

splineknots::KnotMatrix& splineknots::KnotMatrix::operator=(const KnotMatrix& other)
{
	if (&other != this)
	{
		utils::DeleteJaggedArray(x_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(y_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(z_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(dx_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(dy_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(dxy_, rows_count_, columns_count_);
		rows_count_ = other.rows_count_;
		columns_count_ = other.columns_count_;
		x_ = utils::CreateJaggedArray<double>(rows_count_, columns_count_);
		y_ = utils::CreateJaggedArray<double>(rows_count_, columns_count_);
		z_ = utils::CreateJaggedArray<double>(rows_count_, columns_count_);
		dx_ = utils::CreateJaggedArray<double>(rows_count_, columns_count_);
		dy_ = utils::CreateJaggedArray<double>(rows_count_, columns_count_);
		dxy_ = utils::CreateJaggedArray<double>(rows_count_, columns_count_);
		for (size_t i = 0; i < rows_count_; i++)
		{
			memcpy(x_[i], other.x_[i], columns_count_);
			memcpy(y_[i], other.y_[i], columns_count_);
			memcpy(z_[i], other.z_[i], columns_count_);
			memcpy(dx_[i], other.dx_[i], columns_count_);
			memcpy(dy_[i], other.dy_[i], columns_count_);
			memcpy(dxy_[i], other.dxy_[i], columns_count_);
		}
	}
	return *this;
}

splineknots::KnotMatrix& splineknots::KnotMatrix::operator=(KnotMatrix&& other)
{
	if (&other != this)
	{
		utils::DeleteJaggedArray(x_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(y_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(z_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(dx_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(dy_, rows_count_, columns_count_);
		utils::DeleteJaggedArray(dxy_, rows_count_, columns_count_);
		rows_count_ = other.rows_count_;
		columns_count_ = other.columns_count_;
		x_ = other.x_;
		other.x_ = nullptr;
		y_ = other.y_;
		other.y_ = nullptr;
		z_ = other.z_;
		other.z_ = nullptr;
		dx_ = other.dx_;
		other.dx_ = nullptr;
		dy_ = other.dy_;
		other.dy_ = nullptr;
		dxy_ = other.dxy_;
		other.dxy_ = nullptr;
		other.rows_count_ = 0;
		other.columns_count_ = 0;
	}
	return *this;
}
