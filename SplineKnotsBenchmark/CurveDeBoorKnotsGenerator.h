#pragma once
#include "DeBoorKnotsGenerator.h"
namespace splineknots {

	class CurveDeBoorKnotsGenerator final
	{
		std::unique_ptr<DeBoorKnotsGenerator> knot_generator_;
	public:

		CurveDeBoorKnotsGenerator(std::unique_ptr<DeBoorKnotsGenerator> deBoorKnotsGenerator);
		void InitializeKnots(SurfaceDimension& dimension, KnotMatrix& values);
		KnotMatrix GenerateKnots(SurfaceDimension& dimension);

	};
	
}

//#include "CurveDeBoorKnotsGenerator_template.cpp"