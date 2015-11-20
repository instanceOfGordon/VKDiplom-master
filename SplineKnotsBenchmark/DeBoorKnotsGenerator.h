#pragma once
#include "KnotsGenerator.h"
#include "SurfaceDimension.h"
#include <functional>
#include <vector>
#include "Tridiagonal.h"
#include "utils.h"
#include "Parallel.h"
#include "Cloneable.h"

namespace splineknots
{
	

	typedef std::function<double(int)> RightSideSelector;
	typedef std::function<void(int, double)> UnknownsSetter;
	typedef std::vector<std::unique_ptr<utils::Tridiagonal>> Tridiagonals;
	class DeBoorKnotsGenerator : public KnotsGenerator
	{
	
	private:
		//InterpolativeMathFunction function_;
		//std::unique_ptr<utils::Tridiagonal> tridiagonal_;
		Tridiagonals tridagonals_;
		//utils::Loop loop_;
		//utils::Loop::Parallelization parallelization_type_;
		bool is_parallel_ = false;
		
	public:
		DeBoorKnotsGenerator(MathFunction math_function);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function);
		virtual ~DeBoorKnotsGenerator();
		//const InterpolativeMathFunction& Function() const;
	 KnotMatrix GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension) override;
		//virtual KnotMatrix* GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension);
	
		virtual std::vector<double> RightSide(RightSideSelector& right_side_variables, double h, double dfirst, double dlast,
			int unknowns_count);
		void InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values);
		virtual void FillXDerivations(KnotMatrix& values);
		virtual void FillXYDerivations(KnotMatrix& values);
		virtual void FillYDerivations(KnotMatrix& values);
		virtual void FillYXDerivations(KnotMatrix& values);
		virtual void FillXDerivations(int column_index, KnotMatrix& values);
		virtual void FillXYDerivations(int column_index, KnotMatrix& values);
		virtual void FillYDerivations(int row_index, KnotMatrix& values);
		virtual void FillYXDerivations(int row_index, KnotMatrix& values);
		virtual void SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast,
			int unknowns_count, UnknownsSetter& unknowns_setter);
		utils::Tridiagonal& Tridiagonal(int index = 0);
		/*void EnableParallelization();
		void DisableParallelization();*/
		void InParallel(bool value);
		bool IsParallel();
	protected:
		DeBoorKnotsGenerator(MathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal);
		

	};
}
