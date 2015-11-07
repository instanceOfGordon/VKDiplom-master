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
		static const size_t kInitCount = 401;


		const double lower_diagonal_value;
		const double main_diagonal_value;
		const double upper_diagonal_value;

	protected:

		const std::vector<double>& LowerDiagonal() const
		{
			return *lower_diagonal_;
			//return *(lower_diagonal_.get());
		}

		const std::vector<double>& MainDiagonal() const
		{
			return *(main_diagonal_.get());
		}

		const std::vector<double>& UpperDiagonal() const
		{
			return *(upper_diagonal_.get());
		}

		const double& LowerDiagonalValue() const
		{
			return lower_diagonal_value;
		}

		const double& MainDiagonalValue() const
		{
			return main_diagonal_value;
		}

		const double& UpperDiagonalValue() const
		{
			return upper_diagonal_value;
		}
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