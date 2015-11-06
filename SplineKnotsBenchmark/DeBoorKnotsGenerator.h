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
	class DeBoorKnotsGenerator //: public KnotsGenerator
	{
		InterpolativeMathFunction function_;
		std::unique_ptr<Tridiagonal> tridiagonal_;

	public:
		DeBoorKnotsGenerator(MathFunction math_function);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function);
		~DeBoorKnotsGenerator();
		const InterpolativeMathFunction& Function() const;
		std::unique_ptr<KnotMatrix> GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension);
	//protected:
		/*std::vector<double> MainDiagonal(size_t unknowns_count);
		std::vector<double> LowerDiagonal(size_t unknowns_count);
		std::vector<double> UpperDiagonal(size_t unknowns_count);*/

		/*std::vector<double> RightSide(RightSideSelector& right_side_autoiables, double h, double dfirst, double dlast,
		                                      int unknowns_count);
		void InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values);
		void FillXDerivations(KnotMatrix& values);
		void FillXYDerivations(KnotMatrix& values);
		void FillYDerivations(KnotMatrix& values);
		void FillYXDerivations(KnotMatrix& values);
		void FillXDerivations(int column_index, KnotMatrix& values);
		void FillXYDerivations(int column_index, KnotMatrix& values);
		void FillYDerivations(int row_index, KnotMatrix& values);
		void FillYXDerivations(int row_index, KnotMatrix& values);
		void SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast,
		                              int unknowns_count, UnknownsSetter& unknowns_setter);*/
		std::vector<double> RightSide(RightSideSelector& right_side_autoiables, double h, double dfirst, double dlast,
			int unknowns_count);
		void InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values);
		void FillXDerivations(KnotMatrix& values);
		void FillXYDerivations(KnotMatrix& values);
		void FillYDerivations(KnotMatrix& values);
		void FillYXDerivations(KnotMatrix& values);
		void FillXDerivations(int column_index, KnotMatrix& values);
		void FillXYDerivations(int column_index, KnotMatrix& values);
		void FillYDerivations(int row_index, KnotMatrix& values);
		void FillYXDerivations(int row_index, KnotMatrix& values);
		void SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast,
			int unknowns_count, UnknownsSetter& unknowns_setter);
	};
}
