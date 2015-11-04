#include "stdafx.h"
#include "ReducedDeboorKnotsGenerator.h"


splineknots::ReducedDeboorKnotsGenerator::ReducedDeboorKnotsGenerator(MathFunction math_function)
	: DeBoorKnotsGenerator(math_function)
{
}


splineknots::ReducedDeboorKnotsGenerator::~ReducedDeboorKnotsGenerator()
{
}

std::vector<double> splineknots::ReducedDeboorKnotsGenerator::MainDiagonal(size_t unknowns_count)
{
	auto equations_count = (unknowns_count + 2) / 2 - 1;
	std::vector<double> diag(equations_count, -14);
	diag[equations_count - 1] = unknowns_count % 2 == 0 ? -15 : -14;
	return diag;
}

std::vector<double> splineknots::ReducedDeboorKnotsGenerator::LowerDiagonal(size_t unknowns_count)
{
	return DeBoorKnotsGenerator::LowerDiagonal((unknowns_count + 2) / 2 - 1);
}

std::vector<double> splineknots::ReducedDeboorKnotsGenerator::UpperDiagonal(size_t unknowns_count)
{
	return DeBoorKnotsGenerator::UpperDiagonal((unknowns_count + 2) / 2 - 1);
}

std::vector<double> splineknots::ReducedDeboorKnotsGenerator::RightSide(RightSideSelector& right_side_variables, double h, double dfirst, double dlast, int unknowns_count)
{
	unknowns_count += 2;
	auto even = unknowns_count % 2 == 0;
	auto tau = even ? 0 : 2;
	auto eta = even ? -4 : 1;
	auto upsilon = even ? unknowns_count - 2 : unknowns_count - 3;
	//dlast = eta*dlast;
	auto equations_count = unknowns_count / 2 - 1;
	auto rs = std::vector<double>(equations_count);

	//auto threeDivH = 1.5 / h;
	auto three_div_h = 3 / h;
	auto twelve_div_h = three_div_h * 4;
	rs[0] = three_div_h*(right_side_variables(4) - right_side_variables(0)) - twelve_div_h*(right_side_variables(3) - right_side_variables(1)) - dfirst;

	rs[equations_count - 1] = three_div_h *
		(right_side_variables(upsilon + tau) -
			right_side_variables(upsilon - 2))
		-
		twelve_div_h *
		(right_side_variables(upsilon + 1) - right_side_variables(upsilon - 1)) -
		eta * dlast;

	for (auto k = 2; k < equations_count; k++)
	{
		auto k2 = k * 2;
		rs[k - 1] = three_div_h * (right_side_variables(2 * (k + 1)) - right_side_variables(2 * (k - 1)) - twelve_div_h * (right_side_variables(k2 + 1) - right_side_variables(k2 - 1)));
	}

	//I do not know (yet) why but these must be half of values designed by L. Mino
	//This cycle shouldn't be here
	for (int i = 0; i < rs.size(); i++)
	{
		rs[i] *= 0.5;
	}

	return rs;
}

std::vector<double> splineknots::ReducedDeboorKnotsGenerator::RightSideCross(KnotMatrix& knots, int i, double dfirst, double dlast, int unknowns_count)
{
	unknowns_count += 2;
	auto even = unknowns_count % 2 == 0;
	auto equationsCount = unknowns_count / 2 - 1; //even ? unknowns_count/2 - 2 : unknowns_count/2 - 3;
	auto tau = even ? 0 : 2;
	auto eta = even ? -4 : 1;
	auto upsilon = even ? unknowns_count - 2 : unknowns_count - 3;
	//dlast = eta*dlast;
	std::vector<double> rs(equationsCount);
	auto one_div7 = 1 / 7;
	auto hx = knots(1, 0).X() - knots(0, 0).X();
	auto hy = knots(0, 1).Y() - knots(0, 0).Y();
	auto one_div_hx = 1.0 / hx;
	auto one_div_hy = 1.0 / hy;
	auto three_div_hx = 3.0 / hx;
	//auto twelweDivHx = threeDivHx * 4;
	auto three_div_hy = 3.0 / hy;
	//auto twelweDivHy = threeDivHy * 4;
	auto three_div7_hx = one_div7*three_div_hx;
	auto three_div7_hy = one_div7*three_div_hy;
	auto three_div7_hxhy = three_div7_hy / hy;
	auto rows = knots.RowsCount();
	auto columns = knots.ColumnsCount();
	
	auto iMin1 = i - 1;
	auto iMin2 = i - 2;
	rs[0] = one_div7 * (knots(iMin2, 6).Dxy() + knots(iMin2, 2).Dxy()) - 2 * knots(iMin1, 4).Dxy()
		+ three_div7_hx * (knots(iMin1, 6).Dy() + knots(iMin1, 2).Dy()) + three_div7_hy * (-knots(iMin2, 6).X() + knots(iMin2, 2).X())
		+ 3 * three_div_hx * (knots(i, 6).Dy()+ knots(i, 2).Dy()) + 3 * three_div7_hxhy * (-knots(iMin2, 6).Z() + knots(iMin2, 2).Z())
		+ 4 * three_div7_hx * (-knots(iMin1, 6).Dy()- knots(iMin1, 2).Dy()) + 4 * three_div7_hy * (knots(iMin2, 5).X() - knots(iMin2, 3).X())
		+ three_div7_hy * (knots(i, 6).Dx() + knots(i, 2).Dx()) + 9 * three_div7_hxhy * (-knots(i, 6).Z() + knots(i, 2).Z())
		- 12 * three_div7_hxhy * (knots(iMin1, 6).Z() - knots(iMin1, 2).Z() + knots(iMin2, 5).Z() - knots(0, 3).Z())
		- 6 * one_div_hx * knots(iMin2, 4).Dy()+ 12 * one_div_hy * (knots(i, 5).Dx() + knots(i, 3).Dx()) +
		36 * three_div7_hxhy * (knots(i, 5).Z() + knots(i, 3).Z())
		- 18 * one_div_hx * knots(i, 4).Dy()+ 48 * three_div7_hxhy * (-knots(iMin1, 5).Z() + knots(iMin1, 3).Z()) +
		24 * one_div_hx * knots(iMin1, 4).Z() - dfirst;
	rs[equationsCount - 1] = one_div7*(knots(iMin2, columns - 1).Dxy() + knots(iMin2, columns - 5).Dxy()) -
		2 * knots(iMin1, columns - 3).Dxy()
		+ three_div7_hx*(knots(iMin1, columns - 1).Dy()+ knots(iMin1, columns - 5).Dy()) +
		three_div7_hy*(-knots(iMin2, columns - 1).X() + knots(iMin2, columns - 5).X())
		+ 3 * three_div_hx*(knots(i, columns - 1).Dy()+ knots(i, columns - 5).Dy()) +
		3 * three_div7_hxhy*(-knots(iMin2, columns - 1).Z() + knots(iMin2, columns - 5).Z())
		+
		4 * three_div7_hx*(-knots(iMin1, columns - 1).Dy()- knots(iMin1, columns - 5).Dy()) +
		4 * three_div7_hy*(knots(iMin2, columns - 2).X() - knots(iMin2, columns - 4).X())
		+ three_div7_hy*(knots(i, columns - 1).Dx() + knots(i, columns - 5).Dx()) +
		9 * three_div7_hxhy*(-knots(i, columns - 1).Z() + knots(i, columns - 5).Z())
		-
		12 * three_div7_hxhy*
		(knots(iMin1, columns - 1).Z() - knots(iMin1, columns - 5).Z() +
			knots(iMin2, columns - 2).Z() - knots(iMin2, columns - 4).Z())
		- 6 * one_div_hx*knots(iMin2, columns - 3).Dy()+
		12 * one_div_hy*(knots(i, columns - 2).Dx() + knots(i, columns - 4).Dx()) +
		36 * three_div7_hxhy*(knots(i, columns - 2).Z() + knots(i, columns - 4).Z())
		- 18 * one_div_hx*knots(i, columns - 3).Dy()+
		48 * three_div7_hxhy*(-knots(iMin1, columns - 2).Z() + knots(iMin1, columns - 4).Z()) +
		24 * one_div_hx*knots(iMin1, columns - 3).Z() - eta*dlast;
	for (int k = 1, j = 6; k < equationsCount - 1; k++, j += 2)
	{
		//auto i2 = i * 2;
		rs[k] = one_div7*(knots(iMin2, j + 2).Dxy() + knots(iMin2, j - 2).Dxy()) - 2 * knots(iMin1, j).Dxy()
			+ three_div7_hx*(knots(iMin1, j + 2).Dy()+ knots(iMin1, j - 2).Dy()) +
			three_div7_hy*(-knots(iMin2, j + 2).X() + knots(iMin2, j - 2).X())
			+ 3 * three_div_hx*(knots(i, j + 2).Dy()+ knots(i, j - 2).Dy()) +
			3 * three_div7_hxhy*(-knots(iMin2, j + 2).Z() + knots(iMin2, j - 2).Z())
			+ 4 * three_div7_hx*(-knots(iMin1, j + 2).Dy()- knots(iMin1, j - 2).Dy()) +
			4 * three_div7_hy*(knots(iMin2, j + 1).X() - knots(iMin2, j - 1).X())
			+ three_div7_hy*(knots(i, j + 2).Dx() + knots(i, j - 2).Dx()) +
			9 * three_div7_hxhy*(-knots(i, j + 2).Z() + knots(i, j - 2).Z())
			-
			12 * three_div7_hxhy*
			(knots(iMin1, j + 2).Z() - knots(iMin1, j - 2).Z() + knots(iMin2, j + 1).Z() - knots(iMin2, j - 1).Z())
			- 6 * one_div_hx*knots(iMin2, j).Dy()+ 12 * one_div_hy*(knots(i, j + 1).Dx() + knots(i, j - 1).Dx()) +
			36 * three_div7_hxhy*(knots(i, j + 1).Z() + knots(i, j - 1).Z())
			- 18 * one_div_hx*knots(i, j).Dy()+ 48 * three_div7_hxhy*(-knots(iMin1, j + 1).Z() + knots(iMin1, j - 1).Z()) +
			24 * one_div_hx*knots(iMin1, j).Z();
	}
	return rs;
}

void splineknots::ReducedDeboorKnotsGenerator::InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillXDerivations(KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillYDerivations(KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillXDerivations(int column_index, KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillXYDerivations(int column_index, KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillYDerivations(int row_index, KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::FillYXDerivations(int row_index, KnotMatrix& values)
{
}

void splineknots::ReducedDeboorKnotsGenerator::SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast, int unknowns_count, UnknownsSetter& unknowns_setter)
{
}

std::unique_ptr<splineknots::KnotMatrix> splineknots::ReducedDeboorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
{
}