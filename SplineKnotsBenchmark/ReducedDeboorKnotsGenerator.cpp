#include "stdafx.h"
#include "ReducedDeboorKnotsGenerator.h"


splineknots::ReducedDeboorKnotsGenerator::ReducedDeboorKnotsGenerator(MathFunction math_function)
	//: DeBoorKnotsGenerator(math_function)
	: function_(math_function), 
	full_deboor_(new DeBoorKnotsGenerator(math_function)),
	tridiagonal_(new ReducedDeBoorTridiagonal)
{
}

splineknots::ReducedDeboorKnotsGenerator::ReducedDeboorKnotsGenerator(InterpolativeMathFunction math_function)
	//: DeBoorKnotsGenerator(math_function)
	: function_(math_function),
	full_deboor_(new DeBoorKnotsGenerator(math_function)),
	tridiagonal_(new ReducedDeBoorTridiagonal)
{
}

splineknots::ReducedDeboorKnotsGenerator::~ReducedDeboorKnotsGenerator()
{
}

//std::vector<double> splineknots::ReducedDeboorKnotsGenerator::MainDiagonal(size_t unknowns_count)
//{
//	auto equations_count = (unknowns_count + 2) / 2 - 1;
//	std::vector<double> diag(equations_count, -14);
//	diag[equations_count - 1] = unknowns_count % 2 == 0 ? -15 : -14;
//	return diag;
//}
//
//std::vector<double> splineknots::ReducedDeboorKnotsGenerator::LowerDiagonal(size_t unknowns_count)
//{
//	return DeBoorKnotsGenerator::LowerDiagonal((unknowns_count + 2) / 2 - 1);
//}
//
//std::vector<double> splineknots::ReducedDeboorKnotsGenerator::UpperDiagonal(size_t unknowns_count)
//{
//	return DeBoorKnotsGenerator::UpperDiagonal((unknowns_count + 2) / 2 - 1);
//}

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
	auto three_div7_hx = one_div7 * three_div_hx;
	auto three_div7_hy = one_div7 * three_div_hy;
	auto three_div7_hxhy = three_div7_hy / hy;
	auto rows = knots.RowsCount();
	auto columns = knots.ColumnsCount();

	auto iMin1 = i - 1;
	auto iMin2 = i - 2;
	rs[0] = one_div7 * (knots(iMin2, 6).Dxy() + knots(iMin2, 2).Dxy()) - 2 * knots(iMin1, 4).Dxy()
		+ three_div7_hx * (knots(iMin1, 6).Dy() + knots(iMin1, 2).Dy()) + three_div7_hy * (-knots(iMin2, 6).X() + knots(iMin2, 2).X())
		+ 3 * three_div_hx * (knots(i, 6).Dy() + knots(i, 2).Dy()) + 3 * three_div7_hxhy * (-knots(iMin2, 6).Z() + knots(iMin2, 2).Z())
		+ 4 * three_div7_hx * (-knots(iMin1, 6).Dy() - knots(iMin1, 2).Dy()) + 4 * three_div7_hy * (knots(iMin2, 5).X() - knots(iMin2, 3).X())
		+ three_div7_hy * (knots(i, 6).Dx() + knots(i, 2).Dx()) + 9 * three_div7_hxhy * (-knots(i, 6).Z() + knots(i, 2).Z())
		- 12 * three_div7_hxhy * (knots(iMin1, 6).Z() - knots(iMin1, 2).Z() + knots(iMin2, 5).Z() - knots(0, 3).Z())
		- 6 * one_div_hx * knots(iMin2, 4).Dy() + 12 * one_div_hy * (knots(i, 5).Dx() + knots(i, 3).Dx()) +
		36 * three_div7_hxhy * (knots(i, 5).Z() + knots(i, 3).Z())
		- 18 * one_div_hx * knots(i, 4).Dy() + 48 * three_div7_hxhy * (-knots(iMin1, 5).Z() + knots(iMin1, 3).Z()) +
		24 * one_div_hx * knots(iMin1, 4).Z() - dfirst;
	rs[equationsCount - 1] = one_div7 * (knots(iMin2, columns - 1).Dxy() + knots(iMin2, columns - 5).Dxy()) -
		2 * knots(iMin1, columns - 3).Dxy()
		+ three_div7_hx * (knots(iMin1, columns - 1).Dy() + knots(iMin1, columns - 5).Dy()) +
		three_div7_hy * (-knots(iMin2, columns - 1).X() + knots(iMin2, columns - 5).X())
		+ 3 * three_div_hx * (knots(i, columns - 1).Dy() + knots(i, columns - 5).Dy()) +
		3 * three_div7_hxhy * (-knots(iMin2, columns - 1).Z() + knots(iMin2, columns - 5).Z())
		+
		4 * three_div7_hx * (-knots(iMin1, columns - 1).Dy() - knots(iMin1, columns - 5).Dy()) +
		4 * three_div7_hy * (knots(iMin2, columns - 2).X() - knots(iMin2, columns - 4).X())
		+ three_div7_hy * (knots(i, columns - 1).Dx() + knots(i, columns - 5).Dx()) +
		9 * three_div7_hxhy * (-knots(i, columns - 1).Z() + knots(i, columns - 5).Z())
		-
		12 * three_div7_hxhy *
		(knots(iMin1, columns - 1).Z() - knots(iMin1, columns - 5).Z() +
			knots(iMin2, columns - 2).Z() - knots(iMin2, columns - 4).Z())
		- 6 * one_div_hx * knots(iMin2, columns - 3).Dy() +
		12 * one_div_hy * (knots(i, columns - 2).Dx() + knots(i, columns - 4).Dx()) +
		36 * three_div7_hxhy * (knots(i, columns - 2).Z() + knots(i, columns - 4).Z())
		- 18 * one_div_hx * knots(i, columns - 3).Dy() +
		48 * three_div7_hxhy * (-knots(iMin1, columns - 2).Z() + knots(iMin1, columns - 4).Z()) +
		24 * one_div_hx * knots(iMin1, columns - 3).Z() - eta * dlast;
	for (int k = 1, j = 6; k < equationsCount - 1; k++ , j += 2)
	{
		//auto i2 = i * 2;
		rs[k] = one_div7 * (knots(iMin2, j + 2).Dxy() + knots(iMin2, j - 2).Dxy()) - 2 * knots(iMin1, j).Dxy()
			+ three_div7_hx * (knots(iMin1, j + 2).Dy() + knots(iMin1, j - 2).Dy()) +
			three_div7_hy * (-knots(iMin2, j + 2).X() + knots(iMin2, j - 2).X())
			+ 3 * three_div_hx * (knots(i, j + 2).Dy() + knots(i, j - 2).Dy()) +
			3 * three_div7_hxhy * (-knots(iMin2, j + 2).Z() + knots(iMin2, j - 2).Z())
			+ 4 * three_div7_hx * (-knots(iMin1, j + 2).Dy() - knots(iMin1, j - 2).Dy()) +
			4 * three_div7_hy * (knots(iMin2, j + 1).X() - knots(iMin2, j - 1).X())
			+ three_div7_hy * (knots(i, j + 2).Dx() + knots(i, j - 2).Dx()) +
			9 * three_div7_hxhy * (-knots(i, j + 2).Z() + knots(i, j - 2).Z())
			-
			12 * three_div7_hxhy *
			(knots(iMin1, j + 2).Z() - knots(iMin1, j - 2).Z() + knots(iMin2, j + 1).Z() - knots(iMin2, j - 1).Z())
			- 6 * one_div_hx * knots(iMin2, j).Dy() + 12 * one_div_hy * (knots(i, j + 1).Dx() + knots(i, j - 1).Dx()) +
			36 * three_div7_hxhy * (knots(i, j + 1).Z() + knots(i, j - 1).Z())
			- 18 * one_div_hx * knots(i, j).Dy() + 48 * three_div7_hxhy * (-knots(iMin1, j + 1).Z() + knots(iMin1, j - 1).Z()) +
			24 * one_div_hx * knots(iMin1, j).Z();
	}
	return rs;
}

void splineknots::ReducedDeboorKnotsGenerator::FillXDerivations(KnotMatrix& values)
{
	auto ncols = values.ColumnsCount();
	for (size_t j = 0; j < ncols; j++)
	{
		full_deboor_->FillXDerivations(j, values);
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

void splineknots::ReducedDeboorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
{
	full_deboor_->FillXYDerivations(0, values);
	full_deboor_->FillXYDerivations(values.ColumnsCount()-1, values);
	full_deboor_->FillYXDerivations(0, values);
	full_deboor_->FillYXDerivations(values.RowsCount() - 1, values);
}

void splineknots::ReducedDeboorKnotsGenerator::FillYDerivations(KnotMatrix& values)
{
	auto nrows = values.RowsCount();
	for (size_t i = 0; i < nrows; i++)
	{
		full_deboor_->FillYDerivations(i, values);
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
				three_div_4h * (values(i + 1, j).Z() - values(i - 1, j).Z())
				- 0.25 * (values(i + 1, j).Dy() + values(i - 1, j).Dy())
				);
		}
	}
}

void splineknots::ReducedDeboorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
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

	auto ncols = values.ColumnsCount();
	for (size_t i = 1; i < nrows-1; i+=2)
	{
		for (size_t j = 1; j < ncols-1; j+=2)
		{
			values(i, j).SetDxy(
				one_div_16*
				(values(i+1,j+1).Dxy()+values(i+1,j-1).Dxy()+values(i-1,j+1).Dxy() +
					values(i-1,j-1).Dxy())
				- three_div_16*one_div_hy*
				(values(i + 1, j + 1).Dx() + values(i + 1, j - 1).Dx() + values(i - 1, j + 1).Dx() +
					values(i - 1, j - 1).Dx())
				- three_div_16*one_div_hx*
				(values(i + 1, j + 1).Dy() + values(i + 1, j - 1).Dy() + values(i - 1, j + 1).Dy() +
					values(i - 1, j - 1).Dy())
			);
		}
	}

	for (size_t i = 1; i < nrows-1; i+=2)
	{
		for (size_t j = 2; j < ncols-2; j+=2)
		{
			values(i, j).SetDxy(
				0.75*one_div_hy*(values(i,j+1).Dx()-values(i,j-1).Dx())
				-0.25*(values(i,j+1).Dxy()-values(i,j-1).Dxy())
			);
		}
	}

	for (size_t i = 2; i < nrows - 2; i += 2)
	{
		for (size_t j = 1; j < ncols - 2; j += 2)
		{
			values(i, j).SetDxy(
				0.75*one_div_hy*(values(i, j + 1).Dx() - values(i, j - 1).Dx())
				- 0.25*(values(i, j + 1).Dxy() - values(i, j - 1).Dxy())
			);
		}
	}
}

void splineknots::ReducedDeboorKnotsGenerator::FillYXDerivations(int row_index, KnotMatrix& values)
{
	auto unknowns_count = values.ColumnsCount() - 2;
	if (unknowns_count == 0) return;
	auto h = values(0, 1).Y() - values(0, 0).Y();
	auto dfirst = values(row_index, 0).Dxy();
	auto dlast = values(row_index, values.ColumnsCount() - 1).Dxy();

	auto result = RightSideCross(values, row_index, dfirst, dlast, unknowns_count);
	/*auto ldiag = LowerDiagonal(unknowns_count);
	auto mdiag = MainDiagonal(unknowns_count);
	auto udiag = UpperDiagonal(unknowns_count);
	utils::SolveTridiagonalSystem(&ldiag.front(), &mdiag.front(), &udiag.front(), &result.front(), result.size());*/
	tridiagonal_->Solve(unknowns_count, &result.front());
	for (size_t i = 0; i < result.size(); i++)
	{
		values(row_index, 2 * i + 1).SetDxy(result[i]);
	}
}

void splineknots::ReducedDeboorKnotsGenerator::SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast, int unknowns_count, UnknownsSetter& unknowns_setter)
{
	auto result = RightSide(selector, h, dfirst, dlast, unknowns_count);
	/*auto ldiag = LowerDiagonal(unknowns_count);
	auto mdiag = MainDiagonal(unknowns_count);
	auto udiag = UpperDiagonal(unknowns_count);
	utils::SolveTridiagonalSystem(&ldiag.front(), &mdiag.front(), &udiag.front(), &result.front(), result.size());*/
	tridiagonal_->Solve(unknowns_count, &result.front());
	for (size_t k = 0; k < result.size(); k++)
	{
		unknowns_setter(2*(k + 1), result[k]);
	}
}

const splineknots::InterpolativeMathFunction& splineknots::ReducedDeboorKnotsGenerator::Function() const
{
	return function_;
}

std::unique_ptr<splineknots::KnotMatrix> splineknots::ReducedDeboorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
{
	if(udimension.knot_count<6|| vdimension.knot_count<6)
	{
		//DeBoorKnotsGenerator deboor(Function());
			return full_deboor_->GenerateKnots(udimension, vdimension);
	}
	
	auto values = new KnotMatrix(udimension.knot_count, vdimension.knot_count);
	auto& valuesRef = *values;
	FillXDerivations(valuesRef);
	FillYDerivations(valuesRef);
	FillXYDerivations(valuesRef);
	FillYXDerivations(valuesRef);

	return std::unique_ptr<KnotMatrix>(values);
}
