#pragma once

#include "DeBoorKnotsGenerator.h"

namespace splineknots
{
	class ReducedDeBoorKnotsGenerator final :
		public DeBoorKnotsGenerator
	{
	public:	
		ReducedDeBoorKnotsGenerator(MathFunction function);
		ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function);
		~ReducedDeBoorKnotsGenerator() override;
		KnotMatrix GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension) override;
	protected:
		void InitializeBuffers(size_t u_count, size_t v_count) override;
		void RightSide(const RightSideSelector& right_side_variables, double h, double dfirst, double dlast,
		               int unknowns_count, double* rightside_buffer) override;
		void RightSideCross(const KnotMatrix& knots, int i, double dfirst, double dlast,
		                    int unknowns_count, double* rightside_buffer);
		void FillXDerivations(KnotMatrix& values) override;
		void FillXYDerivations(KnotMatrix& values) override;
		void FillYDerivations(KnotMatrix& values) override;
		void FillYXDerivations(KnotMatrix& values) override;
		void FillYXDerivations(int row_index, KnotMatrix& values) override;
		void SolveTridiagonal(const RightSideSelector& selector, double h, double dfirst, double dlast,
		                      int unknowns_count, UnknownsSetter& unknowns_setter) override;
	};
}
