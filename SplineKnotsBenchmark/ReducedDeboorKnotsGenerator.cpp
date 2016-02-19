#include "stdafx.h"
#include <omp.h>
#include "ReducedDeboorTridiagonal.h"
#include "ReducedDeboorKnotsGenerator.h"
#include "utils.h"
#include <algorithm>
#include <ostream>
#include <iostream>


splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(MathFunction math_function)
	: DeBoorKnotsGenerator(math_function, std::make_unique<splineknots::ReducedDeBoorTridiagonal>()),
	full_tridiagonal_(std::make_unique<splineknots::Tridiagonal>(1,4,1))
{
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(InterpolativeMathFunction math_function)
	: DeBoorKnotsGenerator(math_function, std::make_unique<splineknots::ReducedDeBoorTridiagonal>()),
	full_tridiagonal_(std::make_unique<splineknots::Tridiagonal>(1, 4, 1))
{
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(const ReducedDeBoorKnotsGenerator& other)
	: DeBoorKnotsGenerator(other)
{
	full_tridiagonal_ = std::unique_ptr<splineknots::Tridiagonal>(other.full_tridiagonal_->Clone());
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(ReducedDeBoorKnotsGenerator&& other)
	: DeBoorKnotsGenerator(std::move(other))
{
	full_tridiagonal_ = std::move(other.full_tridiagonal_);
}
splineknots::ReducedDeBoorKnotsGenerator& splineknots::ReducedDeBoorKnotsGenerator::operator=(const ReducedDeBoorKnotsGenerator& other)
{
	if (this == &other)
		return *this;
	DeBoorKnotsGenerator::operator =(other);
	full_tridiagonal_ = std::unique_ptr<splineknots::Tridiagonal>(other.full_tridiagonal_->Clone());
	return *this;
}

splineknots::ReducedDeBoorKnotsGenerator& splineknots::ReducedDeBoorKnotsGenerator::operator=(ReducedDeBoorKnotsGenerator&& other)
{
	if (this == &other)
		return *this;
	DeBoorKnotsGenerator::operator =(std::move(other));
	full_tridiagonal_ = std::move(other.full_tridiagonal_);
	return *this;
}

void splineknots::ReducedDeBoorKnotsGenerator::RightSideCross(const KnotMatrix& knots, const int i, const double dfirst,
	const double dlast, const int unknowns_count, double* rightside_buffer)
{
	auto even = unknowns_count % 2 == 0;
	auto equations_count = even ? unknowns_count / 2 - 1 : unknowns_count / 2;
	auto eta = even ? -4 : 1;
	auto& pchx = static_cast<PrecalculatedReduced&>(const_cast<Precalculated&>(PrecalculatedHX()));
	auto& pchy = static_cast<PrecalculatedReduced&>(const_cast<Precalculated&>(PrecalculatedHY()));

	auto six_div_hx = pchx.six_div_h;
	auto eighteen_div_hx = pchx.eighteen_div_h;
	auto twentyfour_div_hx = pchx.twentyfour_div_h;
	auto twelve_div_hy = pchy.twelve_div_h;
	auto three_div7_hx = pchx.three_div_7h;
	auto nine_div7_hx = pchx.nine_div_7h;
	auto twelve_div7_hx = pchx.twelve_div_h;
	auto three_div7_hy = pchy.three_div_7h;
	auto twelve_div7_hy = pchy.twelve_div_h;

	auto nine_div7_hxhy = precalculated_reduced_cross_->nine_div_7hxhy;
	auto thirtysix_div7_hxhy = precalculated_reduced_cross_->thirtysix_div_7hxhy;
	auto onehundredeight_div7_hxhy = precalculated_reduced_cross_->onehundredeight_div_7hxhy;
	auto onehundredfortyfour_div7_hxhy = precalculated_reduced_cross_->onehundredfortyfour_div_7hxhy;
	auto twentyseven_div7_hxhy = precalculated_reduced_cross_->twentyseven_div_7hxhy;
	auto columns = knots.ColumnsCount();

	auto iMin1 = i - 1;
	auto iMin2 = i - 2;
	auto one_div7 = 1 / 7;
	for (int k = 0, j = 4; k < equations_count - 1; k++, j += 2)
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
			twentyseven_div7_hxhy * (knots(i, j_minus_2).Z() - knots(i, j_plus_2).Z())
			-
			thirtysix_div7_hxhy *
			(knots(iMin1, j_plus_2).Z() - knots(iMin1, j_minus_2).Z() + knots(iMin2, j_plus_1).Z() - knots(iMin2, j_minus_1).Z())
			- six_div_hx * knots(iMin2, j).Dy() + twelve_div_hy * (knots(i, j_plus_1).Dx() + knots(i, j_minus_1).Dx()) +
			onehundredeight_div7_hxhy * (knots(i, j_plus_1).Z() + knots(i, j_minus_1).Z())
			- eighteen_div_hx * knots(i, j).Dy() + onehundredfortyfour_div7_hxhy * (knots(iMin1, j_minus_1).Z() - knots(iMin1, j_plus_1).Z()) +
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
		twelve_div_hy * (knots(i, columns - 2).Dx() + knots(i, columns - 4).Dx()) +
		onehundredeight_div7_hxhy * (knots(i, columns - 2).Z() + knots(i, columns - 4).Z())
		- eighteen_div_hx * knots(i, columns - 3).Dy() +
		onehundredfortyfour_div7_hxhy * (knots(iMin1, columns - 4).Z() - knots(iMin1, columns - 2).Z()) +
		twentyfour_div_hx * knots(iMin1, columns - 3).Z() - eta * dlast;
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXDerivations(KnotMatrix& values)
{
	auto h = PrecalculatedHX().h;
	auto three_div_h = PrecalculatedHX().three_div_h;
	int nrows = values.RowsCount();
	int ncols = values.ColumnsCount();
	utils::For(0, ncols,
		[&](int j)
	{
		DeBoorKnotsGenerator::FillXDerivations(j, values);
		for (size_t i = 1; i < nrows - 1; i += 2)
		{
			values(i, j).SetDx(
				0.25*(three_div_h * (values(i + 1, j).Z() - values(i - 1, j).Z())
					- values(i + 1, j).Dx() + values(i - 1, j).Dx())
				);
		}
	},
		1, IsParallel());

}

void splineknots::ReducedDeBoorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
{
	auto rightside_buffer = full_tridiagonal_->RightSideBuffer();
	DeBoorKnotsGenerator::FillXYDerivations(0, values, rightside_buffer);
	DeBoorKnotsGenerator::FillXYDerivations(values.ColumnsCount() - 1, values, rightside_buffer);
	DeBoorKnotsGenerator::FillYXDerivations(0, values, rightside_buffer);
	DeBoorKnotsGenerator::FillYXDerivations(values.RowsCount() - 1, values, rightside_buffer);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYDerivations(KnotMatrix& values)
{
	auto h = PrecalculatedHY().h;
	auto three_div_h = PrecalculatedHY().three_div_h;
	int ncols = values.ColumnsCount();
	int nrows = values.RowsCount();
	utils::For(0, nrows,
		[&](int i)
	{
		DeBoorKnotsGenerator::FillYDerivations(i, values);
		for (size_t j = 1; j < ncols - 1; j += 2)
		{
			values(i, j).SetDy(
				0.25*(three_div_h * (values(i, j + 1).Z() - values(i, j - 1).Z())
					- values(i, j + 1).Dy() + values(i, j - 1).Dy())
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
	auto hy = PrecalculatedHY().h;

	auto three_div_16hy = PrecalculatedHX().three_div_h*one_div_16;
	auto three_div_16hx = PrecalculatedHY().three_div_h*one_div_16;

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

	auto three_div_hy = 0.75 / hy;

	// Rests 2,3
	for (size_t j = 2; j < ncols - 2; j += 2)
	{
		values(1, j).SetDxy(
			0.25*(three_div_hy * (values(1, j + 1).Dx() - values(1, j - 1).Dx())
				- values(1, j + 1).Dxy() - values(1, j - 1).Dxy())
			);
	}
	utils::For(2, nrows - 2,
		[&](int i)
	{
		for (size_t j = 2; j < ncols - 2; j += 2)
		{
			values(i + 1, j).SetDxy(
				0.25*(three_div_hy * (values(i + 1, j + 1).Dx() - values(i + 1, j - 1).Dx())
					- values(i + 1, j + 1).Dxy() - values(i + 1, j - 1).Dxy())
				);
		}
		for (size_t j = 1; j < ncols - 2; j += 2)
		{
			values(i, j).SetDxy(
				0.25*(three_div_hy * (values(i, j + 1).Dx() - values(i, j - 1).Dx())
					- values(i, j + 1).Dxy() - values(i, j - 1).Dxy())
				);
		}
	},
		2, IsParallel());
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(const int row_index, KnotMatrix& values, double* rightside_buffer)
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

void splineknots::ReducedDeBoorKnotsGenerator::SolveTridiagonal(const RightSideSelector& selector, const Precalculated& precalculated, const double dfirst, const double dlast, const int unknowns_count, UnknownsSetter& unknowns_setter, double* rightside_buffer)
{
	if(rightside_buffer!=nullptr)
	{
		DeBoorKnotsGenerator::SolveTridiagonal(selector, precalculated, dfirst, dlast, unknowns_count,
		                                       unknowns_setter, rightside_buffer);
		return;
	}
	auto& tridiagonal = Tridiagonal(omp_get_thread_num());
	auto results_buffer = tridiagonal.RightSideBuffer();
	RightSide(selector, precalculated, dfirst, dlast, unknowns_count, results_buffer);
	tridiagonal.Solve(unknowns_count);
	for (size_t k = 0; k < unknowns_count / 2 - 1; k++)
	{
		unknowns_setter(2 * (k + 1), results_buffer[k]);
	}
}

const ::splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedReducedCross& splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedCross() const
{
	return *precalculated_reduced_cross_;
}

void splineknots::ReducedDeBoorKnotsGenerator::SetPrecalculatedCross(std::unique_ptr<PrecalculatedReducedCross> precalculated_reduced_cross)
{
	precalculated_reduced_cross_ = std::move(precalculated_reduced_cross);
}

void splineknots::ReducedDeBoorKnotsGenerator::InitializeBuffers(const size_t u_count, const size_t v_count)
{
	full_tridiagonal_->ResizeBuffers(std::max(u_count - 2, v_count - 2));
	auto size = std::max(u_count / 2-1, v_count / 2-1);
	auto& trid = Tridagonals();
	for (size_t i = 0; i < Tridagonals().size(); i++)
	{
		trid[i]->ResizeBuffers(size);
	}
}

void splineknots::ReducedDeBoorKnotsGenerator::RightSide(const RightSideSelector& right_side_variables, const Precalculated& precalculated, const double dfirst, const double dlast, const int unknowns_count, double* const rightside_buffer)
{
	auto even = unknowns_count % 2 == 0;
	auto tau = even ? 0 : 2;
	auto eta = even ? -4 : 1;
	auto upsilon = even ? unknowns_count : unknowns_count - 1;
	auto equations_count = even ? unknowns_count / 2 - 1 : unknowns_count / 2;
	auto three_div_h = precalculated.three_div_h;
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

splineknots::KnotMatrix splineknots::ReducedDeBoorKnotsGenerator::GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension)
{
	//Precalculate(udimension, vdimension);
	
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

splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedReduced::PrecalculatedReduced(const double h) : DeBoorKnotsGenerator::Precalculated(h)
{
	six_div_h = 2 * three_div_h;
	twelve_div_h = 2 * six_div_h;
	eighteen_div_h = 3 * six_div_h;
	twentyfour_div_h = 2 * twelve_div_h;
	three_div_7h = (1 / 7) * three_div_h;
	nine_div_7h = 3 * three_div_h;
	twelve_div_7h = 4 * three_div_h;
}

splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedReducedCross::PrecalculatedReducedCross(const Precalculated& precalculated_hx, const Precalculated& precalculated_hy)
{
	nine_div_7hxhy = (3 / 7) * precalculated_hy.three_div_h / precalculated_hx.h;
	twentyseven_div_7hxhy = 3 * nine_div_7hxhy;
	thirtysix_div_7hxhy = 4 * nine_div_7hxhy;
	onehundredeight_div_7hxhy = 3 * thirtysix_div_7hxhy;
	onehundredfortyfour_div_7hxhy = 4 * thirtysix_div_7hxhy;
}

void splineknots::ReducedDeBoorKnotsGenerator::Precalculate(const SurfaceDimension& udimension, const SurfaceDimension& vdimension)
{
	SetPrecalculatedHX(std::make_unique<PrecalculatedReduced>(abs(udimension.max - udimension.min) / (udimension.knot_count - 1)));
	SetPrecalculatedHY(std::make_unique<PrecalculatedReduced>(abs(vdimension.max - vdimension.min) / (vdimension.knot_count - 1)));
	auto& pchx = PrecalculatedHX();
	auto& pchy = PrecalculatedHY();
	SetPrecalculatedCross(std::make_unique<PrecalculatedReducedCross>(pchx, pchy));
}