#pragma once

#include "DeBoorKnotsGenerator.h"

namespace splineknots {
	class ReducedDeBoorKnotsGenerator :
		public DeBoorKnotsGenerator
	{

	public:
		KnotMatrix GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension) override;
		//KnotMatrix* GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension) override;
		ReducedDeBoorKnotsGenerator(MathFunction function);
		ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function);
		~ReducedDeBoorKnotsGenerator();

		std::vector<double> RightSide(RightSideSelector& right_side_autoiables, double h, double dfirst, double dlast,
			int unknowns_count) override;
		std::vector<double> RightSideCross(KnotMatrix& knots, int i, double dfirst, double dlast,
			int unknowns_count);
		//void InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values) override;
		void FillXDerivations(KnotMatrix& values) override;
		void FillXYDerivations(KnotMatrix& values) override;
		void FillYDerivations(KnotMatrix& values) override;
		void FillYXDerivations(KnotMatrix& values) override;
		void FillYXDerivations(int row_index, KnotMatrix& values) override;
		void SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast,
			int unknowns_count, UnknownsSetter& unknowns_setter) override;
		
	};
}

