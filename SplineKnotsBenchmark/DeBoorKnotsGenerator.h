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
		Tridiagonals tridagonals_;
	
		std::vector<std::vector<double>> rightsides_buffers_;
		
		bool is_parallel_ = false;

	public:
		DeBoorKnotsGenerator(MathFunction math_function);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function);
		virtual ~DeBoorKnotsGenerator();
		//const InterpolativeMathFunction& Function() const;
	 KnotMatrix GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension) override;
		//virtual KnotMatrix* GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension);
	
	 virtual void RightSide(RightSideSelector& right_side_variables, double h, double dfirst, double dlast,
		 int unknowns_count, double* rightside_buffer);
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
		
		
		/*void EnableParallelization();
		void DisableParallelization();*/
		void InParallel(bool value);
		bool IsParallel();
	protected:

		DeBoorKnotsGenerator(MathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal);
		DeBoorKnotsGenerator(InterpolativeMathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal);

		std::vector<std::vector<double>>& RightSidesBuffers();
		Tridiagonals& Tridagonals();
		virtual void InitializeBuffers(size_t u_count, size_t v_count);
		utils::Tridiagonal& Tridiagonal(int index = 0);
		double* RightSideBuffer(int index = 0);	
	};
}
