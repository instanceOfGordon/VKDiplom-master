#pragma once
#include "stdafx.h"
#include <vector>
namespace utils {
	class Tridiagonal
	{
	public:
		//static Tridiagonal& Instance();
		Tridiagonal(double lower_value, double main_value, double upper_value);
		virtual ~Tridiagonal();

		void Resize(size_t newsize);
		virtual void Solve(size_t num_unknowns, double* right_side);
	private:
		std::unique_ptr<std::vector<double>> lower_diagonal_;
		std::unique_ptr<std::vector<double>> main_diagonal_;
		std::unique_ptr<std::vector<double>> upper_diagonal_;
		std::unique_ptr<std::vector<double>> lu_buffer_;
		static const size_t kInitCount = 1501;


		const double lower_diagonal_value;
		const double main_diagonal_value;
		const double upper_diagonal_value;

	protected:

		const std::vector<double>& LowerDiagonal() const;

		const std::vector<double>& Buffer();

		const std::vector<double>& MainDiagonal() const;

		const std::vector<double>& UpperDiagonal() const;

		const double& LowerDiagonalValue() const;

		const double& MainDiagonalValue() const;

		const double& UpperDiagonalValue() const;
	};

	class ReducedDeBoorTridiagonal : public Tridiagonal
	{
	public:
		//static Tridiagonal& Instance();
		ReducedDeBoorTridiagonal();
		virtual ~ReducedDeBoorTridiagonal();

		void Solve(size_t num_unknowns, double* right_side) override;

	};

}