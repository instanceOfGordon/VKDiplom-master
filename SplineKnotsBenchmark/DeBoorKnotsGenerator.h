#pragma once
#include "KnotsGenerator.h"
#include "SurfaceDimension.h"
#include <functional>
#include <vector>
#include "Tridiagonal.h"


namespace splineknots
{
	typedef std::function<double(int)> RightSideSelector;
	typedef std::function<void(int, double)> UnknownsSetter;
	typedef std::vector<std::unique_ptr<Tridiagonal>> Tridiagonals;

	class DeBoorKnotsGenerator : public KnotsGenerator
	{
		Tridiagonals tridagonals_;
		//std::vector<std::vector<double>> rightsides_buffers_;
		bool is_parallel_;
	public:
		DeBoorKnotsGenerator(MathFunction math_function);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function);
		~DeBoorKnotsGenerator() override;
		DeBoorKnotsGenerator(const DeBoorKnotsGenerator& other);
		DeBoorKnotsGenerator(DeBoorKnotsGenerator&& other);
		DeBoorKnotsGenerator& operator=(const DeBoorKnotsGenerator& other);
		DeBoorKnotsGenerator& operator=(DeBoorKnotsGenerator&& other);
		KnotMatrix GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension) override;
		void InParallel(bool value);
		bool IsParallel();

	protected:
		struct Precalculated
		{
			double h, three_div_h;
			explicit Precalculated(const double h);
			virtual ~Precalculated() = default;
			Precalculated(const Precalculated& other) = default;
			Precalculated& operator=(const Precalculated& other) = default;
		};

		virtual void InitializeBuffers(const size_t u_count, const size_t v_count);
		DeBoorKnotsGenerator(const MathFunction math_function, std::unique_ptr<Tridiagonal> tridiagonal);
		DeBoorKnotsGenerator(const InterpolativeMathFunction math_function, std::unique_ptr<Tridiagonal> tridiagonal);
		Tridiagonals& Tridagonals();
		Tridiagonal& Tridiagonal(const int index = 0);
		virtual void RightSide(const RightSideSelector& right_side_variables, const Precalculated& precalculated, const double dfirst, const double dlast,
		                       const int unknowns_count, double* const rightside_buffer);
		virtual void Precalculate(const SurfaceDimension& udimension, const SurfaceDimension& vdimension);
		void InitializeKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension, KnotMatrix& values);
		virtual void FillXDerivations(KnotMatrix& values);
		virtual void FillXYDerivations(KnotMatrix& values);
		virtual void FillYDerivations(KnotMatrix& values);
		virtual void FillYXDerivations(KnotMatrix& values);
		virtual void FillXDerivations(const int column_index, KnotMatrix& values);
		virtual void FillXYDerivations(const int column_index, KnotMatrix& values);
		virtual void FillYDerivations(const int row_index, KnotMatrix& values);
		virtual void FillYXDerivations(const int row_index, KnotMatrix& values);
		virtual void SolveTridiagonal(const RightSideSelector& selector,const Precalculated& precalculated, const double dfirst, const double dlast,
		                              const int unknowns_count, UnknownsSetter& unknowns_setter);
	private:
		std::unique_ptr<Precalculated> precalculated_hx_;
		std::unique_ptr<Precalculated> precalculated_hy_;
	protected:
		const Precalculated& PrecalculatedHX() const;
		const Precalculated& PrecalculatedHY() const;
		void SetPrecalculatedHX(std::unique_ptr<Precalculated> precalculated);
		void SetPrecalculatedHY(std::unique_ptr<Precalculated> precalculated);
	};
}
