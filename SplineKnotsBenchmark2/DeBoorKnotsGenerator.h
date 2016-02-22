#pragma once
#include "MathFunction.h"
#include "KnotMatrix.h"
#include "SurfaceDimension.h"
#include <functional>
#include <vector>
#include "Tridiagonal.h"
#include <omp.h>

namespace splineknots
{
	typedef std::vector<std::unique_ptr<Tridiagonal>> Tridiagonals;

	class DeBoorKnotsGenerator final
	{
		friend class ReducedDeboorKnotsGenerator;
		InterpolativeMathFunction function_;
		Tridiagonals tridagonals_;
		bool is_parallel_;
	public:
		struct Precalculated
		{
			double h, three_div_h;
			explicit Precalculated(const double h);
			~Precalculated() = default;
			Precalculated(const Precalculated& other) = default;
			Precalculated& operator=(const Precalculated& other) = default;
		};
		DeBoorKnotsGenerator(MathFunction math_function);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function);
		~DeBoorKnotsGenerator() = default;
		DeBoorKnotsGenerator(const DeBoorKnotsGenerator& other);
		DeBoorKnotsGenerator(DeBoorKnotsGenerator&& other);
		DeBoorKnotsGenerator& operator=(const DeBoorKnotsGenerator& other);
		DeBoorKnotsGenerator& operator=(DeBoorKnotsGenerator&& other);
		KnotMatrix GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension, double* calculation_time = nullptr);
		void InParallel(bool value);
		bool IsParallel();
		void InitializeBuffers(const size_t u_count, const size_t v_count);
		DeBoorKnotsGenerator(const MathFunction math_function, std::unique_ptr<Tridiagonal> tridiagonal);
		DeBoorKnotsGenerator(const InterpolativeMathFunction math_function, std::unique_ptr<Tridiagonal> tridiagonal);
		Tridiagonals& Tridagonals();
		Tridiagonal& Tridiagonal(const int index = 0);

		template<typename RightSideSelector>
		void RightSide(const RightSideSelector& right_side_variables, const Precalculated& precalculated, const double dfirst, const double dlast,
		               const int unknowns_count, double* const rightside_buffer)
		{
			auto h3 = precalculated.three_div_h;
			rightside_buffer[0] = h3 * (right_side_variables(2) - right_side_variables(0)) - dfirst;
			rightside_buffer[unknowns_count - 1] = h3 * (right_side_variables(unknowns_count + 1) - right_side_variables(unknowns_count - 1)) - dlast;
			for (auto i = 1; i < unknowns_count - 1; i++)
			{
				rightside_buffer[i] = h3 * (right_side_variables(i + 2) - right_side_variables(i));
			}
		}

		void Precalculate(const SurfaceDimension& udimension, const SurfaceDimension& vdimension);
		void InitializeKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension, KnotMatrix& values);
		void FillXDerivations(KnotMatrix& values);
		void FillXYDerivations(KnotMatrix& values);
		void FillYDerivations(KnotMatrix& values);
		void FillYXDerivations(KnotMatrix& values);
		void FillXDerivations(const int column_index, KnotMatrix& values);
		void FillXYDerivations(const int column_index, KnotMatrix& values);
		void FillYDerivations(const int row_index, KnotMatrix& values);
		void FillYXDerivations(const int row_index, KnotMatrix& values);
		
		template<typename RightSideSelector, typename UnknownsSetter>
		void SolveTridiagonal(const RightSideSelector& selector, const Precalculated& precalculated, const double dfirst, const double dlast,
		                      const int unknowns_count, UnknownsSetter& unknowns_setter)
		{
			auto& tridiagonal = Tridiagonal(omp_get_thread_num());
			auto rightside = tridiagonal.RightSideBuffer();
			RightSide(selector, precalculated, dfirst, dlast, unknowns_count, rightside);

			tridiagonal.Solve(unknowns_count);
			for (int k = 0; k < unknowns_count; k++)
			{
				unknowns_setter(k + 1, rightside[k]);
			}
		}

	private:
		std::unique_ptr<Precalculated> precalculated_hx_;
		std::unique_ptr<Precalculated> precalculated_hy_;
	public:
		const Precalculated& PrecalculatedHX() const;
		const Precalculated& PrecalculatedHY() const;
		void SetPrecalculatedHX(std::unique_ptr<Precalculated> precalculated);
		void SetPrecalculatedHY(std::unique_ptr<Precalculated> precalculated);
	};
}
