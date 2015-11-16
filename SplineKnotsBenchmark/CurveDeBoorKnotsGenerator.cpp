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
splineknots::CurveDeBoorKnotsGenerator::CurveDeBoorKnotsGenerator(std::unique_ptr<DeBoorKnotsGenerator> de_boor_knots_generator)
	: knot_generator_(move(de_boor_knots_generator))
{
}

void splineknots::CurveDeBoorKnotsGenerator::InitializeKnots(SurfaceDimension& dimension, KnotMatrix& values)
{
	auto uSize = abs(dimension.max - dimension.min) / (dimension.knot_count - 1);
	//auto vSize = abs(vdimension.max - vdimension.min) / (vdimension.knot_count - 1);
	auto v = dimension.min;
	const InterpolativeMathFunction& f = knot_generator_->Function();
	// Init Z
	for (auto i = 0; i < dimension.knot_count; i++, v += uSize)
	{
		auto z = f.Z()(v, 0);
		//Function.Z(u,v); //Z(u, v);
		values[0][i] = Knot(0, v, z);
	}
	// Init Dx
	auto knotCountMin1 = dimension.knot_count - 1;
	auto dy = f.Dy()(values[0][0].X(), values[0][0].Y());
	values[0][0].SetDy(dy); //Function.Dx(values[0,j].X, values[0,j].Y);
	values[0][knotCountMin1].SetDy(f.Dy()(values[0][knotCountMin1].X(), values[0][knotCountMin1].Y()));
}

splineknots::KnotMatrix splineknots::CurveDeBoorKnotsGenerator::GenerateKnots(SurfaceDimension& dimension)
{
	if (dimension.knot_count < 6) {
		CurveDeBoorKnotsGenerator cdeboor(std::make_unique<DeBoorKnotsGenerator>(knot_generator_->Function()));
		return cdeboor.GenerateKnots(dimension);
	}
	//SurfaceDimension vdim(1, 1, 1);
	KnotMatrix values(1, dimension.knot_count);
	
	InitializeKnots(dimension, values);
	knot_generator_->FillYDerivations(values);
	return values;
}