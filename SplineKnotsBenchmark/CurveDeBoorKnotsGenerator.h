#pragma once
#include "DeBoorKnotsGenerator.h"
namespace splineknots {

	template<typename TKnotGenerator>
	class CurveKnotsGenerator final
	{
		std::unique_ptr<TKnotGenerator> knot_generator_;


		void InitializeKnots(SurfaceDimension& dimension, KnotMatrix& values)
		{
			auto uSize = abs(dimension.max - dimension.min) / (dimension.knot_count - 1);
			//auto vSize = abs(vdimension.max - vdimension.min) / (vdimension.knot_count - 1);
			auto u = dimension.min;
			const InterpolativeMathFunction& f = knot_generator_->Function();
			// Init Z
			for (auto i = 0; i < dimension.knot_count; i++, u += uSize)
			{
				auto z = f.Z()(u, 0);
				//Function.Z(u,v); //Z(u, v);
				values[i][0] = Knot(u, 0, z);
			}
			// Init Dx
			auto knotCountMin1 = dimension.knot_count - 1;
			auto dx = f.Dx()(values[0][0].X(), values[0][0].Y());
			values[0][0].SetDx(dx); //Function.Dx(values[0,j].X, values[0,j].Y);
			values[knotCountMin1][0].SetDx(f.Dx()(values[knotCountMin1][0].X(), values[knotCountMin1][0].Y()));
		}
	public:
		std::unique_ptr<KnotMatrix> GenerateKnots(SurfaceDimension& dimension)
		{
			if (dimension.knot_count < 4) {
				return{};
			}
			//SurfaceDimension vdim(1, 1, 1);
			auto values = new KnotMatrix(dimension.knot_count, 1);
			auto& valuesRef = *values;
			InitializeKnots(dimension, valuesRef);
			knot_generator_->FillXDerivations(valuesRef);
			return std::unique_ptr<KnotMatrix>(values);
		}


		CurveKnotsGenerator(MathFunction math_function)
			: knot_generator_(std::unique_ptr<TKnotGenerator>(new TKnotGenerator(math_function)))
		{
		}

		~CurveKnotsGenerator()
		{
			
		}
		
	};

	
}

#include "CurveDeBoorKnotsGenerator_template.cpp"