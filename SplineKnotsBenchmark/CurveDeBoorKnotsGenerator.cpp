#include "stdafx.h"
#include "CurveDeBoorKnotsGenerator.h"




//std::unique_ptr<splineknots::KnotMatrix> splineknots::CurveDeBoorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
//{
//	if (udimension.knot_count < 4 || vdimension.knot_count < 4)
//	{
//		return{};
//	}
//	auto values = new KnotMatrix(udimension.knot_count, vdimension.knot_count);
//	auto& valuesRef = *values;
//	InitializeKnots(udimension, vdimension, valuesRef);
//	FillXDerivations(valuesRef);
//	FillXYDerivations(valuesRef);
//	FillYDerivations(valuesRef);
//	FillYXDerivations(valuesRef);
//	//return values;
//	return std::unique_ptr<KnotMatrix>(values);
//}