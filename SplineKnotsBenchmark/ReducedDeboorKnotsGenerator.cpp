#include "stdafx.h"
#include <omp.h>
#include "ReducedDeboorTridiagonal.h"
#include "ReducedDeboorKnotsGenerator.h"
#include "utils.h"
#include <algorithm>


splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(MathFunction math_function)
	: DeBoorKnotsGenerator(math_function, std::make_unique<splineknots::ReducedDeBoorTridiagonal>())
{
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(InterpolativeMathFunction math_function)
	: DeBoorKnotsGenerator(math_function, std::make_unique<splineknots::ReducedDeBoorTridiagonal>())
{
}

splineknots::ReducedDeBoorKnotsGenerator::~ReducedDeBoorKnotsGenerator()
{
}

void splineknots::ReducedDeBoorKnotsGenerator::RightSide(const RightSideSelector& right_side_variables, double h,
                                                         double dfirst, double dlast, int unknowns_count, double* rightside_buffer)
{
	auto even = unknowns_count % 2 == 0;
	auto tau = even ? 0 : 2;
	auto eta = even ? -4 : 1;
	auto upsilon = even ? unknowns_count : unknowns_count - 1;
	auto equations_count = even ? unknowns_count / 2 - 1 : unknowns_count / 2;
	auto three_div_h = 3 / h;
	auto twelve_div_h = three_div_h * 4;
	rightside_buffer[0] = three_div_h * (right_side_variables(4) - right_side_variables(0)) - twelve_div_h * (right_side_variables(3) - right_side_variables(1)) - dfirst;

	rightside_buffer[equations_count - 1] = three_div_h *
		(right_side_variables(upsilon + tau) -
			right_side_variables(upsilon - 2))
		-
		twelve_div_h *
		(right_side_variables(upsilon + 1) - right_side_variables(upsilon - 1)) -
		eta * dlast;

	for (auto k = 2; k < equations_count; k++)
	{
		auto k2 = k * 2;
		rightside_buffer[k - 1] = three_div_h * (right_side_variables(2 * (k + 1)) - right_side_variables(2 * (k - 1)) - twelve_div_h * (right_side_variables(k2 + 1) - right_side_variables(k2 - 1)));
	}

	//I do not know (yet) why but these must be half of values designed by L. Mino
	//This cycle shouldn't be here
	for (int i = 1; i < equations_count - 1; i++)
	{
		rightside_buffer[i] *= 0.5;
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::RightSideCross(const KnotMatrix& knots, int i, double dfirst,
                                                              double dlast, int unknowns_count, double* rightside_buffer)
{
	auto even = unknowns_count % 2 == 0;
	auto equations_count = even ? unknowns_count / 2 - 1 : unknowns_count / 2;
	auto eta = even ? -4 : 1;
	auto one_div7 = 1.0 / 7.0;
	auto hx = knots(1, 0).X() - knots(0, 0).X();
	auto hy = knots(0, 1).Y() - knots(0, 0).Y();
	auto one_div_hx = 1.0 / hx;
	auto one_div_hy = 1.0 / hy;
	auto three_div_hx = one_div_hx * 3;
	auto six_div_hx = 2 * three_div_hx;
	auto eighteen_div_hx = 6 * one_div_hx;
	auto twentyfour_div_hx = 8 * three_div_hx;
	auto three_div_hy = 3.0 * one_div_hy;
	auto twelwe_div_hy = three_div_hy * 4;
	auto three_div7_hx = one_div7 * three_div_hx;
	auto nine_div7_hx = 3 * three_div7_hx;
	auto twelve_div7_hx = 4 * three_div7_hx;
	auto three_div7_hy = one_div7 * three_div_hy;
	auto twelve_div7_hy = 4 * three_div7_hy;
	auto three_div7_hxhy = three_div7_hy * one_div_hy;
	auto nine_div7_hxhy = 3 * three_div7_hxhy;
	auto thirtysix_div7_hxhy = 12 * three_div7_hxhy;
	auto onehundredeight_div7_hxhy = 3 * thirtysix_div7_hxhy;
	auto onehundredfortyfour_div7_hxhy = 4 * thirtysix_div7_hxhy;
	auto twentyseven_div7_hxhy = 9 * three_div7_hxhy;
	auto columns = knots.ColumnsCount();

	auto iMin1 = i - 1;
	auto iMin2 = i - 2;
	for (int k = 0, j = 4; k < equations_count - 1; k++ , j += 2)
	{
		auto j_plus_1 = j + 1;
		auto j_minus_1 = j - 2;
		auto j_plus_2 = j + 2;
		auto j_minus_2 = j - 2;
		rightside_buffer[k] = one_div7 * (knots(iMin2, j_plus_2).Dxy() + knots(iMin2, j_minus_2).Dxy()) - 2 * knots(iMin1, j).Dxy()
			+ three_div7_hx * (knots(iMin1, j_plus_2).Dy() + knots(iMin1, j_minus_2).Dy()) +
			three_div7_hy * (knots(iMin2, j_minus_2).X() - knots(iMin2, j_plus_2).X())
			+ nine_div7_hx * (knots(i, j_plus_2).Dy() + knots(i, j_minus_2).Dy()) +
			nine_div7_hxhy * (knots(iMin2, j_minus_2).Z() - knots(iMin2, j_plus_2).Z())
			+ twelve_div7_hx * (-knots(iMin1, j_plus_2).Dy() - knots(iMin1, j_minus_2).Dy()) +
			twelve_div7_hy * (knots(iMin2, j_plus_1).X() - knots(iMin2, j_minus_1).X())
			+ three_div7_hy * (knots(i, j_plus_2).Dx() + knots(i, j_minus_2).Dx()) +
			twentyseven_div7_hxhy * (knots(i, j_minus_2).Z() -knots(i, j_plus_2).Z())
			-
			thirtysix_div7_hxhy *
			(knots(iMin1, j_plus_2).Z() - knots(iMin1, j_minus_2).Z() + knots(iMin2, j_plus_1).Z() - knots(iMin2, j_minus_1).Z())
			- six_div_hx * knots(iMin2, j).Dy() + twelwe_div_hy * (knots(i, j_plus_1).Dx() + knots(i, j_minus_1).Dx()) +
			onehundredeight_div7_hxhy * (knots(i, j_plus_1).Z() + knots(i, j_minus_1).Z())
			- eighteen_div_hx * knots(i, j).Dy() + onehundredfortyfour_div7_hxhy * (knots(iMin1, j_minus_1).Z() -knots(iMin1, j_plus_1).Z()) +
			twentyfour_div_hx * knots(iMin1, j).Z();
	}
	rightside_buffer[0] -= dfirst;
	rightside_buffer[equations_count - 1] = one_div7 * (knots(iMin2, columns - 1).Dxy() + knots(iMin2, columns - 5).Dxy()) -
		2 * knots(iMin1, columns - 3).Dxy()
		+ three_div7_hx * (knots(iMin1, columns - 1).Dy() + knots(iMin1, columns - 5).Dy()) +
		three_div7_hy * (knots(iMin2, columns - 5).X() - knots(iMin2, columns - 1).X())
		+ nine_div7_hx * (knots(i, columns - 1).Dy() + knots(i, columns - 5).Dy()) +
		nine_div7_hxhy * (knots(iMin2, columns - 5).Z() - knots(iMin2, columns - 1).Z())
		+
		twelve_div7_hx * (-knots(iMin1, columns - 1).Dy() - knots(iMin1, columns - 5).Dy()) +
		twelve_div7_hy * (knots(iMin2, columns - 2).X() - knots(iMin2, columns - 4).X())
		+ three_div7_hy * (knots(i, columns - 1).Dx() + knots(i, columns - 5).Dx()) +
		twentyseven_div7_hxhy * (knots(i, columns - 5).Z() - knots(i, columns - 1).Z())
		-
		thirtysix_div7_hxhy *
		(knots(iMin1, columns - 1).Z() - knots(iMin1, columns - 5).Z() +
			knots(iMin2, columns - 2).Z() - knots(iMin2, columns - 4).Z())
		- six_div_hx * knots(iMin2, columns - 3).Dy() +
		twelwe_div_hy * (knots(i, columns - 2).Dx() + knots(i, columns - 4).Dx()) +
		onehundredeight_div7_hxhy * (knots(i, columns - 2).Z() + knots(i, columns - 4).Z())
		- eighteen_div_hx * knots(i, columns - 3).Dy() +
		onehundredfortyfour_div7_hxhy * (knots(iMin1, columns - 4).Z() - knots(iMin1, columns - 2).Z()) +
		twentyfour_div_hx * knots(iMin1, columns - 3).Z() - eta * dlast;
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXDerivations(KnotMatrix& values)
{
	int ncols = values.ColumnsCount();
	utils::For(0, ncols,
		[&](int j)
	{
		DeBoorKnotsGenerator::FillXDerivations(j, values);
	},
		1, IsParallel());
	auto h = values(1, 0).X() - values(0, 0).X();
	auto three_div_4h = 0.75 / h;
	int nrows = values.RowsCount();
	// Rest
	utils::For(1, nrows - 1,
		[&](int i)
	{
		for (size_t j = 0; j < ncols; j++)
		{
			values(i, j).SetDx(
				three_div_4h * (values(i + 1, j).Z() - values(i - 1, j).Z())
				- 0.25 * (values(i + 1, j).Dx() + values(i - 1, j).Dx())
				);
		}
	},
		2, IsParallel());
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
{
	DeBoorKnotsGenerator::FillXYDerivations(0, values);
	DeBoorKnotsGenerator::FillXYDerivations(values.ColumnsCount() - 1, values);
	DeBoorKnotsGenerator::FillYXDerivations(0, values);
	DeBoorKnotsGenerator::FillYXDerivations(values.RowsCount() - 1, values);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYDerivations(KnotMatrix& values)
{
	int nrows = values.RowsCount();
	utils::For(0, nrows,
		[&](int i)
	{
		DeBoorKnotsGenerator::FillYDerivations(i, values);
	},
		1, IsParallel());
	auto h = values(0, 1).Y() - values(0, 0).Y();
	auto three_div_4h = 0.75 / h;
	int ncols = values.ColumnsCount();

	utils::For(0, nrows,
		[&](int i)
	{
		for (size_t j = 1; j < ncols - 1; j += 2)
		{
			values(i, j).SetDy(
				three_div_4h * (values(i, j + 1).Z() - values(i, j - 1).Z())
				- 0.25 * (values(i, j + 1).Dy() + values(i, j - 1).Dy())
				);
		}
	},
		1, IsParallel());
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
{
	int nrows = values.RowsCount();

	utils::For(2, nrows,
		[&](int i)
	{
		FillYXDerivations(i, values);
	},
		2, IsParallel());

	auto one_div_16 = 1.0 / 16.0;
	auto three_div_16 = 3 / 16;
	auto hx = values(1, 0).X() - values(0, 0).X();
	auto hy = values(0, 1).Y() - values(0, 0).Y();
	auto three_div_16hy = three_div_16 / hy;
	auto three_div_16hx = three_div_16 / hx;

	int ncols = values.ColumnsCount();
	// Rest 1
	utils::For(1, nrows - 1,
		[&](int i)
	{
		for (size_t j = 1; j < ncols - 1; j += 2)
		{
			values(i, j).SetDxy(
				one_div_16 *
				(values(i + 1, j + 1).Dxy() + values(i + 1, j - 1).Dxy() + values(i - 1, j + 1).Dxy() +
					values(i - 1, j - 1).Dxy())
				- three_div_16hy *
				(values(i + 1, j + 1).Dx() + values(i + 1, j - 1).Dx() + values(i - 1, j + 1).Dx() +
					values(i - 1, j - 1).Dx())
				- three_div_16hx *
				(values(i + 1, j + 1).Dy() + values(i + 1, j - 1).Dy() + values(i - 1, j + 1).Dy() +
					values(i - 1, j - 1).Dy())
				);
		}
	},
		2, IsParallel());

	auto three_div_4hy = 0.75 / hy;
	// Rest 2
	utils::For(1, nrows - 1,
		[&](int i)
	{
		for (size_t j = 2; j < ncols - 2; j += 2)
		{
			values(i, j).SetDxy(
				three_div_4hy * (values(i, j + 1).Dx() - values(i, j - 1).Dx())
				- 0.25 * (values(i, j + 1).Dxy() - values(i, j - 1).Dxy())
				);
		}
	},
		2, IsParallel());
	// Rest 3
	utils::For(2, nrows - 2,
		[&](int i)
	{
		for (size_t j = 1; j < ncols - 2; j += 2)
		{
			values(i, j).SetDxy(
				three_div_4hy * (values(i, j + 1).Dx() - values(i, j - 1).Dx())
				- 0.25 * (values(i, j + 1).Dxy() - values(i, j - 1).Dxy())
				);
		}
	},
		2, IsParallel());
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(int row_index, KnotMatrix& values)
{
	auto unknowns_count = values.ColumnsCount() - 2;
	if (unknowns_count == 0) return;
	auto h = values(0, 1).Y() - values(0, 0).Y();
	auto dfirst = values(row_index, 0).Dxy();
	auto dlast = values(row_index, values.ColumnsCount() - 1).Dxy();
	auto& tridiagonal = Tridiagonal(omp_get_thread_num());
	auto result_buffer = tridiagonal.RightSideBuffer();
	RightSideCross(values, row_index, dfirst, dlast, unknowns_count, result_buffer);
	tridiagonal.Solve(unknowns_count);
	for (size_t i = 0; i < unknowns_count / 2; i++)
	{
		values(row_index, 2 * i + 1).SetDxy(result_buffer[i]);
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::SolveTridiagonal(const RightSideSelector& selector, double h, double dfirst, double dlast, int unknowns_count, UnknownsSetter& unknowns_setter)
{
	auto& tridiagonal = Tridiagonal(omp_get_thread_num());
	auto results_buffer = tridiagonal.RightSideBuffer();
	RightSide(selector, h, dfirst, dlast, unknowns_count, results_buffer);
	tridiagonal.Solve(unknowns_count);
	for (size_t k = 0; k < unknowns_count / 2 - 1; k++)
	{
		unknowns_setter(2 * (k + 1), results_buffer[k]);
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::InitializeBuffers(size_t u_count, size_t v_count)
{
	auto size = std::max(u_count / 2 - 1, v_count / 2 - 1);
	auto& trid = Tridagonals();
	for (size_t i = 0; i < Tridagonals().size(); i++)
	{
		trid[i]->ResizeBuffers(size);
	}
}


splineknots::KnotMatrix splineknots::ReducedDeBoorKnotsGenerator::GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension)
{
	if (udimension.knot_count < 6 || vdimension.knot_count < 6)
	{
		DeBoorKnotsGenerator deboor(Function());
		return deboor.GenerateKnots(udimension, vdimension);
	}

	KnotMatrix values(udimension.knot_count, vdimension.knot_count);
	InitializeBuffers(udimension.knot_count, vdimension.knot_count);
	InitializeKnots(udimension, vdimension, values);
	FillXDerivations(values);
	FillYDerivations(values);
	FillXYDerivations(values);
	FillYXDerivations(values);
	return values;
}
