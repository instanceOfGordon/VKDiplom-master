#pragma once

namespace splineknots
{
	class KnotMatrix
	{
		size_t rows_count_;
		size_t columns_count_;
		double** x_;
		double** y_;
		double** z_;
		double** dx_;
		double** dy_;
		double** dxy_;
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
		double X(size_t i, size_t j) const
		{
			return x_[i][j];
		}
		double Y(size_t i, size_t j) const
		{
			return y_[i][j];
		}
		double Z(size_t i, size_t j) const
		{
			return z_[i][j];
		}
		double Dx(size_t i, size_t j) const
		{
			return dx_[i][j];
		}
		double Dy(size_t i, size_t j) const
		{
			return dy_[i][j];
		}
		double Dxy(size_t i, size_t j) const
		{
			return dxy_[i][j];
		}
		void SetX(size_t i, size_t j, const double value)
		{
			x_[i][j] = value;
		}
		void SetY(size_t i, size_t j, const double value)
		{
			y_[i][j] = value;
		}
		void SetZ(size_t i, size_t j, const double value)
		{
			z_[i][j] = value;
		}
		void SetDx(size_t i, size_t j, const double value)
		{
			dx_[i][j] = value;
		}
		void SetDy(size_t i, size_t j, const double value)
		{
			dy_[i][j] = value;
		}
		void SetDxy(size_t i, size_t j, const double value)
		{
			dxy_[i][j] = value;
		}

		void Print();
	};
}
