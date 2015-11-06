#pragma once

#include "DeBoorKnotsGenerator.h"

namespace splineknots {
	class ReducedDeboorKnotsGenerator final//:
		//public DeBoorKnotsGenerator
	{
		InterpolativeMathFunction function_;
		std::unique_ptr<DeBoorKnotsGenerator> full_deboor_;
		std::unique_ptr<ReducedDeBoorTridiagonal> tridiagonal_;
	public:
		//std::unique_ptr<KnotMatrix> GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension) override;
		//ReducedDeboorKnotsGenerator(MathFunction function);
		//ReducedDeboorKnotsGenerator(InterpolativeMathFunction function);
		//~ReducedDeboorKnotsGenerator();

		//std::vector<double> MainDiagonal(size_t unknowns_count) override;
		//std::vector<double> LowerDiagonal(size_t unknowns_count) override;
		//std::vector<double> UpperDiagonal(size_t unknowns_count) override;

		//std::vector<double> RightSide(RightSideSelector& right_side_autoiables, double h, double dfirst, double dlast,
		//	int unknowns_count) override;
		//std::vector<double> RightSideCross(KnotMatrix& knots, int i, double dfirst, double dlast,
		//	int unknowns_count);
		////void InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values) override;
		//void FillXDerivations(KnotMatrix& values) override;
		//void FillXYDerivations(KnotMatrix& values) override;
		//void FillYDerivations(KnotMatrix& values) override;
		//void FillYXDerivations(KnotMatrix& values) override;
		//void FillYXDerivations(int row_index, KnotMatrix& values) override;
		//void SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast,
		//	int unknowns_count, UnknownsSetter& unknowns_setter) override;
		const InterpolativeMathFunction& Function() const;
		std::unique_ptr<KnotMatrix> GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension);
		ReducedDeboorKnotsGenerator(MathFunction function);
		ReducedDeboorKnotsGenerator(InterpolativeMathFunction function);
		~ReducedDeboorKnotsGenerator();

		//std::vector<double> MainDiagonal(size_t unknowns_count) override;
		//std::vector<double> LowerDiagonal(size_t unknowns_count) override;
		//std::vector<double> UpperDiagonal(size_t unknowns_count) override;

		std::vector<double> RightSide(RightSideSelector& right_side_autoiables, double h, double dfirst, double dlast,
			int unknowns_count);
		std::vector<double> RightSideCross(KnotMatrix& knots, int i, double dfirst, double dlast,
			int unknowns_count);
		//void InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values) override;
		void FillXDerivations(KnotMatrix& values);
		void FillXYDerivations(KnotMatrix& values);
		void FillYDerivations(KnotMatrix& values);
		void FillYXDerivations(KnotMatrix& values) ;
		void FillYXDerivations(int row_index, KnotMatrix& values);
		void SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast,
			int unknowns_count, UnknownsSetter& unknowns_setter);
	};
}

