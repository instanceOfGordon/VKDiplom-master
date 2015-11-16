#include "stdafx.h"
#include "ReducedDeboorKnotsGenerator.h"


splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(MathFunction math_function)
	: DeBoorKnotsGenerator(math_function, std::make_unique<utils::ReducedDeBoorTridiagonal>())
{
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(InterpolativeMathFunction math_function)
	: DeBoorKnotsGenerator(math_function, std::make_unique<utils::ReducedDeBoorTridiagonal>())

{
}

splineknots::ReducedDeBoorKnotsGenerator::~ReducedDeBoorKnotsGenerator()
{
}

std::vector<double> splineknots::ReducedDeBoorKnotsGenerator::RightSide(RightSideSelector& right_side_variables, double h, double dfirst, double dlast, int unknowns_count)
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
	rs[0] = three_div_h * (right_side_variables(4) - right_side_variables(0)) - twelve_div_h * (right_side_variables(3) - right_side_variables(1)) - dfirst;

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

std::vector<double> splineknots::ReducedDeBoorKnotsGenerator::RightSideCross(KnotMatrix& knots, int i, double dfirst, double dlast, int unknowns_count)
{
	unknowns_count += 2;
	auto even = unknowns_count % 2 == 0;
	auto equationsCount = unknowns_count / 2 - 1; //even ? unknowns_count/2 - 2 : unknowns_count/2 - 3;
	//auto tau = even ? 0 : 2;
	auto eta = even ? -4 : 1;
	//auto upsilon = even ? unknowns_count - 2 : unknowns_count - 3;
	//dlast = eta*dlast;
	std::vector<double> rs(equationsCount);
	auto one_div7 = 1 / 7;
	auto hx = knots(1, 0).X() - knots(0, 0).X();
	auto hy = knots(0, 1).Y() - knots(0, 0).Y();
	auto one_div_hx = 1.0 / hx;
	auto one_div_hy = 1.0 / hy;
	auto three_div_hx = 3.0 / hx;
	auto six_div_hx = 2 *three_div_hx;
	
	auto eighteen_div_hx = 6*one_div_hx;
	auto twentyfour_div_hx = 8 * three_div_hx;
	//auto twelweDivHx = threeDivHx * 4;
	auto three_div_hy = 3.0 / hy;
	//auto nine_div_hy = 3 * three_div_hy;
	auto twelwe_div_hy = three_div_hy * 4;
	auto three_div7_hx = one_div7 * three_div_hx;
	auto nine_div7_hx = 3 * three_div7_hx;
	auto twelve_div7_hx = 4*three_div7_hx;
	auto three_div7_hy = one_div7 * three_div_hy;
	auto twelve_div7_hy = 4 * three_div7_hy;
	auto three_div7_hxhy = three_div7_hy / hy;
	auto nine_div7_hxhy = 3 * three_div7_hxhy;

	auto thirtysix_div7_hxhy =  12 * three_div7_hxhy;
	auto onehundredeight_div7_hxhy = 3 * thirtysix_div7_hxhy;
	auto onehundredfortyfour_div7_hxhy = 4 * thirtysix_div7_hxhy;
	auto twentyseven_div7_hxhy = 9 * three_div7_hxhy;

	auto rows = knots.RowsCount();
	auto columns = knots.ColumnsCount();

	auto iMin1 = i - 1;
	auto iMin2 = i - 2;
	rs[0] = one_div7 * (knots(iMin2, 6).Dxy() + knots(iMin2, 2).Dxy()) - 2 * knots(iMin1, 4).Dxy()
		+ three_div7_hx * (knots(iMin1, 6).Dy() + knots(iMin1, 2).Dy()) + three_div7_hy * (-knots(iMin2, 6).X() + knots(iMin2, 2).X())
		+ nine_div7_hx * (knots(i, 6).Dy() + knots(i, 2).Dy()) + nine_div7_hxhy * (-knots(iMin2, 6).Z() + knots(iMin2, 2).Z())
		+ twelve_div7_hx * (-knots(iMin1, 6).Dy() - knots(iMin1, 2).Dy()) + twelve_div7_hy * (knots(iMin2, 5).X() - knots(iMin2, 3).X())
		+ three_div7_hy * (knots(i, 6).Dx() + knots(i, 2).Dx()) + twentyseven_div7_hxhy * (-knots(i, 6).Z() + knots(i, 2).Z())
		- thirtysix_div7_hxhy * (knots(iMin1, 6).Z() - knots(iMin1, 2).Z() + knots(iMin2, 5).Z() - knots(0, 3).Z())
		- six_div_hx * knots(iMin2, 4).Dy() + twelwe_div_hy * (knots(i, 5).Dx() + knots(i, 3).Dx()) +
		onehundredeight_div7_hxhy * (knots(i, 5).Z() + knots(i, 3).Z())
		- eighteen_div_hx * knots(i, 4).Dy() + onehundredfortyfour_div7_hxhy * (-knots(iMin1, 5).Z() + knots(iMin1, 3).Z()) +
		twentyfour_div_hx * knots(iMin1, 4).Z() - dfirst;
	rs[equationsCount - 1] = one_div7 * (knots(iMin2, columns - 1).Dxy() + knots(iMin2, columns - 5).Dxy()) -
		2 * knots(iMin1, columns - 3).Dxy()
		+ three_div7_hx * (knots(iMin1, columns - 1).Dy() + knots(iMin1, columns - 5).Dy()) +
		three_div7_hy * (-knots(iMin2, columns - 1).X() + knots(iMin2, columns - 5).X())
		+ nine_div7_hx * (knots(i, columns - 1).Dy() + knots(i, columns - 5).Dy()) +
		nine_div7_hxhy * (-knots(iMin2, columns - 1).Z() + knots(iMin2, columns - 5).Z())
		+
		twelve_div7_hx * (-knots(iMin1, columns - 1).Dy() - knots(iMin1, columns - 5).Dy()) +
		twelve_div7_hy * (knots(iMin2, columns - 2).X() - knots(iMin2, columns - 4).X())
		+ three_div7_hy * (knots(i, columns - 1).Dx() + knots(i, columns - 5).Dx()) +
		twentyseven_div7_hxhy * (-knots(i, columns - 1).Z() + knots(i, columns - 5).Z())
		-
		thirtysix_div7_hxhy *
		(knots(iMin1, columns - 1).Z() - knots(iMin1, columns - 5).Z() +
			knots(iMin2, columns - 2).Z() - knots(iMin2, columns - 4).Z())
		- six_div_hx * knots(iMin2, columns - 3).Dy() +
		twelwe_div_hy * (knots(i, columns - 2).Dx() + knots(i, columns - 4).Dx()) +
		onehundredeight_div7_hxhy * (knots(i, columns - 2).Z() + knots(i, columns - 4).Z())
		- eighteen_div_hx * knots(i, columns - 3).Dy() +
		onehundredfortyfour_div7_hxhy * (-knots(iMin1, columns - 2).Z() + knots(iMin1, columns - 4).Z()) +
		twentyfour_div_hx * knots(iMin1, columns - 3).Z() - eta * dlast;
	for (int k = 1, j = 6; k < equationsCount - 1; k++ , j += 2)
	{
		//auto i2 = i * 2;
		rs[k] = one_div7 * (knots(iMin2, j + 2).Dxy() + knots(iMin2, j - 2).Dxy()) - 2 * knots(iMin1, j).Dxy()
			+ three_div7_hx * (knots(iMin1, j + 2).Dy() + knots(iMin1, j - 2).Dy()) +
			three_div7_hy * (-knots(iMin2, j + 2).X() + knots(iMin2, j - 2).X())
			+ nine_div7_hx * (knots(i, j + 2).Dy() + knots(i, j - 2).Dy()) +
			nine_div7_hxhy * (-knots(iMin2, j + 2).Z() + knots(iMin2, j - 2).Z())
			+ twelve_div7_hx * (-knots(iMin1, j + 2).Dy() - knots(iMin1, j - 2).Dy()) +
			twelve_div7_hy * (knots(iMin2, j + 1).X() - knots(iMin2, j - 1).X())
			+ three_div7_hy * (knots(i, j + 2).Dx() + knots(i, j - 2).Dx()) +
			twentyseven_div7_hxhy * (-knots(i, j + 2).Z() + knots(i, j - 2).Z())
			-
			thirtysix_div7_hxhy *
			(knots(iMin1, j + 2).Z() - knots(iMin1, j - 2).Z() + knots(iMin2, j + 1).Z() - knots(iMin2, j - 1).Z())
			- six_div_hx * knots(iMin2, j).Dy() + twelwe_div_hy * (knots(i, j + 1).Dx() + knots(i, j - 1).Dx()) +
			onehundredeight_div7_hxhy * (knots(i, j + 1).Z() + knots(i, j - 1).Z())
			- eighteen_div_hx * knots(i, j).Dy() + onehundredfortyfour_div7_hxhy * (-knots(iMin1, j + 1).Z() + knots(iMin1, j - 1).Z()) +
			twentyfour_div_hx * knots(iMin1, j).Z();
	}
	return rs;
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXDerivations(KnotMatrix& values)
{
	auto ncols = values.ColumnsCount();
	for (size_t j = 0; j < ncols; j++)
	{
		DeBoorKnotsGenerator::FillXDerivations(j, values);
	}
	auto h = values(1, 0).X() - values(0, 0).X();
	auto one_div_h = 1.0 / h;
	auto three_div_4h = 0.75 * one_div_h;
	auto nrows = values.RowsCount();
	for (size_t i = 1; i < nrows - 1; i += 2)
	{
		for (size_t j = 0; j < ncols; j++)
		{
			values(i, j).SetDx(
				three_div_4h * (values(i + 1, j).Z() - values(i - 1, j).Z())
				- 0.25 * (values(i + 1, j).Dx() + values(i - 1, j).Dx())
			);
		}
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
{
	DeBoorKnotsGenerator::FillXYDerivations(0, values);
	DeBoorKnotsGenerator::FillXYDerivations(values.ColumnsCount()-1, values);
	DeBoorKnotsGenerator::FillYXDerivations(0, values);
	DeBoorKnotsGenerator::FillYXDerivations(values.RowsCount() - 1, values);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYDerivations(KnotMatrix& values)
{
	auto nrows = values.RowsCount();
	for (size_t i = 0; i < nrows; i++)
	{
		DeBoorKnotsGenerator::FillYDerivations(i, values);
	}
	auto h = values(0, 1).Y() - values(0, 0).Y();
	auto one_div_h = 1.0 / h;
	auto three_div_4h = 0.75 * one_div_h;
	auto ncols = values.ColumnsCount();
	for (size_t i = 0; i < nrows ; i++)
	{
		for (size_t j = 1; j < ncols-1; j+=2)
		{
			values(i, j).SetDy(
				three_div_4h * (values(i, j+1).Z() - values(i, j-1).Z())
				- 0.25 * (values(i, j+1).Dy() + values(i, j-1).Dy())
				);
		}
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
{
	auto nrows = values.RowsCount();
	for (size_t i = 2; i < nrows; i+=2)
	{
		FillYXDerivations(i, values);
	}

	auto one_div_16 = 1.0 / 16.0;
	auto three_div_16 = one_div_16 * 3;
	auto hx = values(1, 0).X() - values(0, 0).X();
	auto hy = values(0, 1).Y() - values(0, 0).Y();
	auto one_div_hx = 1.0 / hx;
	auto one_div_hy = 1.0 / hy;
	auto three_div_16hy = three_div_16*one_div_hy;
	auto three_div_16hx = three_div_16*one_div_hx;

	auto ncols = values.ColumnsCount();
	for (size_t i = 1; i < nrows-1; i+=2)
	{
		for (size_t j = 1; j < ncols-1; j+=2)
		{
			values(i, j).SetDxy(
				one_div_16*
				(values(i+1,j+1).Dxy()+values(i+1,j-1).Dxy()+values(i-1,j+1).Dxy() +
					values(i-1,j-1).Dxy())
				- three_div_16hy*
				(values(i + 1, j + 1).Dx() + values(i + 1, j - 1).Dx() + values(i - 1, j + 1).Dx() +
					values(i - 1, j - 1).Dx())
				- three_div_16hx*
				(values(i + 1, j + 1).Dy() + values(i + 1, j - 1).Dy() + values(i - 1, j + 1).Dy() +
					values(i - 1, j - 1).Dy())
			);
		}
	}

	auto three_div_4hy = 0.75*one_div_hy;
	for (size_t i = 1; i < nrows-1; i+=2)
	{
		for (size_t j = 2; j < ncols-2; j+=2)
		{
			values(i, j).SetDxy(
				three_div_4hy*(values(i,j+1).Dx()-values(i,j-1).Dx())
				-0.25*(values(i,j+1).Dxy()-values(i,j-1).Dxy())
			);
		}
	}

	for (size_t i = 2; i < nrows - 2; i += 2)
	{
		for (size_t j = 1; j < ncols - 2; j += 2)
		{
			values(i, j).SetDxy(
				three_div_4hy*(values(i, j + 1).Dx() - values(i, j - 1).Dx())
				- 0.25*(values(i, j + 1).Dxy() - values(i, j - 1).Dxy())
			);
		}
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(int row_index, KnotMatrix& values)
{
	auto unknowns_count = values.ColumnsCount() - 2;
	if (unknowns_count == 0) return;
	auto h = values(0, 1).Y() - values(0, 0).Y();
	auto dfirst = values(row_index, 0).Dxy();
	auto dlast = values(row_index, values.ColumnsCount() - 1).Dxy();

	auto result = RightSideCross(values, row_index, dfirst, dlast, unknowns_count);

	Tridiagonal().Solve(unknowns_count, &result.front());
	for (size_t i = 0; i < result.size(); i++)
	{
		values(row_index, 2 * i + 1).SetDxy(result[i]);
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast, int unknowns_count, UnknownsSetter& unknowns_setter)
{
	auto result = RightSide(selector, h, dfirst, dlast, unknowns_count);
	Tridiagonal().Solve(unknowns_count, &result.front());
	for (size_t k = 0; k < result.size(); k++)
	{
		unknowns_setter(2*(k + 1), result[k]);
	}
}

std::unique_ptr<splineknots::KnotMatrix> splineknots::ReducedDeBoorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
//splineknots::KnotMatrix*  splineknots::ReducedDeboorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
{
	if(udimension.knot_count<6|| vdimension.knot_count<6)
	{
		DeBoorKnotsGenerator deboor(Function());
			return deboor.GenerateKnots(udimension, vdimension);
	}
	
	auto values = new KnotMatrix(udimension.knot_count, vdimension.knot_count);
	auto& valuesRef = *values;
	DeBoorKnotsGenerator::InitializeKnots(udimension, vdimension, valuesRef);
	FillXDerivations(valuesRef);
	FillYDerivations(valuesRef);
	FillXYDerivations(valuesRef);
	FillYXDerivations(valuesRef);

	//return values;
	return std::unique_ptr<KnotMatrix>(values);
}
