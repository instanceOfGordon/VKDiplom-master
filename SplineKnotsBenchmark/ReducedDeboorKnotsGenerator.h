#pragma once

#include "DeBoorKnotsGenerator.h"

namespace splineknots
{
	class ReducedDeBoorKnotsGenerator final :
		public DeBoorKnotsGenerator
	{
		std::unique_ptr<splineknots::Tridiagonal> full_tridiagonal_;
	public:
		ReducedDeBoorKnotsGenerator(MathFunction function);
		ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function);
		~ReducedDeBoorKnotsGenerator() override =default;
		ReducedDeBoorKnotsGenerator(const ReducedDeBoorKnotsGenerator& other);
		ReducedDeBoorKnotsGenerator(ReducedDeBoorKnotsGenerator&& other);
		ReducedDeBoorKnotsGenerator& operator=(const ReducedDeBoorKnotsGenerator& other);
		ReducedDeBoorKnotsGenerator& operator=(ReducedDeBoorKnotsGenerator&& other);

		KnotMatrix GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension) override;
	protected:

		struct PrecalculatedReduced final : public Precalculated
		{
			double six_div_h,
				twelve_div_h,
				eighteen_div_h,
				twentyfour_div_h,
				three_div_7h,
				nine_div_7h,
				twelve_div_7h;

			PrecalculatedReduced(const double h);
		};

		struct PrecalculatedReducedCross final
		{
			double nine_div_7hxhy,
				twentyseven_div_7hxhy,
				thirtysix_div_7hxhy,
				onehundredeight_div_7hxhy,
				onehundredfortyfour_div_7hxhy;

			PrecalculatedReducedCross(const Precalculated& precalculated_hx, const Precalculated& precalculated_hy);
		};

		void Precalculate(const SurfaceDimension& udimension, const SurfaceDimension& vdimension) override;
		void InitializeBuffers(size_t u_count, size_t v_count) override;
		void RightSide(const RightSideSelector& right_side_variables, const Precalculated& precalculated, 
			const double dfirst, const double dlast, const int unknowns_count, double* const rightside_buffer)
			override;

		void RightSideCross(const KnotMatrix& knots, const int i, const double dfirst, const double dlast,
			const int unknowns_count, double* rightside_buffer);
		void FillXDerivations(KnotMatrix& values) override;
		void FillXYDerivations(KnotMatrix& values) override;
		void FillYDerivations(KnotMatrix& values) override;
		void FillYXDerivations(KnotMatrix& values) override;
		void FillYXDerivations(const int row_index, KnotMatrix& values, double* rightside_buffer = nullptr) 
			override;


		void SolveTridiagonal(const RightSideSelector& selector, const Precalculated& precalculated,
		                      const double dfirst, const double dlast, const int unknowns_count,
		                      UnknownsSetter& unknowns_setter, double* rightside_buffer = nullptr) override;

	private:
		std::unique_ptr<PrecalculatedReducedCross> precalculated_reduced_cross_;
	protected:
		const PrecalculatedReducedCross& PrecalculatedCross() const;
		void SetPrecalculatedCross(std::unique_ptr<PrecalculatedReducedCross> precalculated_reduced_cross);
	};
}