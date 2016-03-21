#include "stdafx.h"
#include <omp.h>
#include "ReducedDeboorTridiagonal.h"
#include "ReducedDeboorKnotsGenerator.h"
#include "utils.h"

#include <ostream>
#include <iostream>
#include "StopWatch.h"


splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(
	MathFunction math_function, bool buffered)
	: function_(math_function), tridagonals_(), is_parallel_(false), 
	deboor_(math_function)
{
	tridagonals_.push_back(std::make_unique<ReducedDeboorTridiagonal>(
		buffered));
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(
	InterpolativeMathFunction math_function, bool buffered)
	: function_(math_function), tridagonals_(), is_parallel_(false), 
	deboor_(math_function)
{
	tridagonals_.push_back(std::make_unique<ReducedDeboorTridiagonal>(
		buffered));
}


splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(
	const ReducedDeBoorKnotsGenerator& other)
	: function_(other.function_),
	  tridagonals_(other.tridagonals_.size()),
	  is_parallel_(other.is_parallel_),
	  deboor_(other.deboor_)
{
	for (size_t i = 0; i < other.tridagonals_.size(); i++)
	{
		auto tridcopy_ptr = new ReducedDeboorTridiagonal(
			*other.tridagonals_[i].get());
		auto trid = std::unique_ptr<ReducedDeboorTridiagonal>(tridcopy_ptr);
		tridcopy_ptr = nullptr;
		tridagonals_.push_back(std::move(trid));
	}
}

splineknots::ReducedDeBoorKnotsGenerator::ReducedDeBoorKnotsGenerator(
	ReducedDeBoorKnotsGenerator&& other):
	function_(std::move(other.function_)),
	tridagonals_(std::move(other.tridagonals_)),
	is_parallel_(other.is_parallel_),
	deboor_(std::move(other.deboor_))
{
}

splineknots::ReducedDeBoorKnotsGenerator& splineknots::
ReducedDeBoorKnotsGenerator::operator=(const ReducedDeBoorKnotsGenerator& 
	other)
{
	if (this == &other)
		return *this;
	tridagonals_.clear();
	for (size_t i = 0; i < other.tridagonals_.size(); i++)
	{
		auto tridcopy_ptr = new ReducedDeboorTridiagonal(
			*other.tridagonals_[i].get());
		auto trid = std::unique_ptr<ReducedDeboorTridiagonal>(
			tridcopy_ptr);
		tridcopy_ptr = nullptr;
		tridagonals_.push_back(std::move(trid));
	}
	is_parallel_ = other.is_parallel_;
	deboor_ = other.deboor_;
	return *this;
}

splineknots::ReducedDeBoorKnotsGenerator& splineknots::
ReducedDeBoorKnotsGenerator::operator=(ReducedDeBoorKnotsGenerator&& other)
{
	if (this == &other)
		return *this;
	tridagonals_ = std::move(other.tridagonals_);
	is_parallel_ = other.is_parallel_;
	deboor_ = std::move(deboor_);
	return *this;
}

void splineknots::ReducedDeBoorKnotsGenerator::RightSideCross(
	const KnotMatrix& knots, const int i, const double dfirst, 
	const double dlast, const int unknowns_count, double* rightside_buffer)
{
	auto even = unknowns_count % 2 == 0;
	auto equations_count = even ? unknowns_count / 2 - 1 : unknowns_count / 2;
	auto eta = even ? -4 : 1;
	auto& pchx = *precalculated_hx_.get();
	auto& pchy = *precalculated_hy_.get();

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
	auto thirtysix_div7_hxhy = 
		precalculated_reduced_cross_->thirtysix_div_7hxhy;
	auto onehundredeight_div7_hxhy = 
		precalculated_reduced_cross_->onehundredeight_div_7hxhy;
	auto onehundredfortyfour_div7_hxhy = 
		precalculated_reduced_cross_->onehundredfortyfour_div_7hxhy;
	auto twentyseven_div7_hxhy = 
		precalculated_reduced_cross_->twentyseven_div_7hxhy;
	auto columns = knots.ColumnsCount();

	auto iMin1 = i - 1;
	auto iMin2 = i - 2;
	auto one_div7 = 1 / 7;
	for (int k = 0, j = 4; k < equations_count - 1; k++ , j += 2)
	{
		auto j_plus_1 = j + 1;
		auto j_minus_1 = j - 2;
		auto j_plus_2 = j + 2;
		auto j_minus_2 = j - 2;
		rightside_buffer[k] = one_div7 * (knots.Dxy(iMin2, j_plus_2) +
			knots.Dxy(iMin2, j_minus_2)) - 2 * knots.Dxy(iMin1, j)
			+ three_div7_hx * (knots.Dy(iMin1, j_plus_2) + 
				knots.Dy(iMin1, j_minus_2)) +
			three_div7_hy * (knots.X(iMin2, j_minus_2) - 
				knots.X(iMin2, j_plus_2))
			+ nine_div7_hx * (knots.Dy(i, j_plus_2) + 
				knots.Dy(i, j_minus_2)) +
			nine_div7_hxhy * (knots.Z(iMin2, j_minus_2) - 
				knots.Z(iMin2, j_plus_2))
			+ twelve_div7_hx * (-knots.Dy(iMin1, j_plus_2) - 
				knots.Dy(iMin1, j_minus_2)) +
			twelve_div7_hy * (knots.X(iMin2, j_plus_1) - 
				knots.X(iMin2, j_minus_1))
			+ three_div7_hy * (knots.Dx(i, j_plus_2) + 
				knots.Dx(i, j_minus_2)) +
			twentyseven_div7_hxhy * (knots.Z(i, j_minus_2) - 
				knots.Z(i, j_plus_2)) - thirtysix_div7_hxhy * (
					knots.Z(iMin1, j_plus_2) - knots.Z(iMin1, j_minus_2) + 
					knots.Z(iMin2, j_plus_1) - knots.Z(iMin2, j_minus_1))
			- six_div_hx * knots.Dy(iMin2, j) + twelve_div_hy * (
				knots.Dx(i, j_plus_1) + knots.Dx(i, j_minus_1)) + 
			onehundredeight_div7_hxhy * (knots.Z(i, j_plus_1) + 
				knots.Z(i, j_minus_1)) - eighteen_div_hx * knots.Dy(i, j) + 
			onehundredfortyfour_div7_hxhy * (knots.Z(iMin1, j_minus_1) - 
				knots.Z(iMin1, j_plus_1)) + twentyfour_div_hx * knots.Z(
					iMin1, j);
	}
	rightside_buffer[0] -= dfirst;
	rightside_buffer[equations_count - 1] = one_div7 * (knots.Dxy(iMin2, 
		columns - 1) + knots.Dxy(iMin2, columns - 5)) - 2 * knots.Dxy(iMin1, 
			columns - 3)
		+ three_div7_hx * (knots.Dy(iMin1, columns - 1) + knots.Dy(iMin1, 
			columns - 5)) +
		three_div7_hy * (knots.X(iMin2, columns - 5) - knots.X(iMin2, columns 
			- 1)) + nine_div7_hx * (knots.Dy(i, columns - 1) + knots.Dy(
				i, columns - 5)) + nine_div7_hxhy * (knots.Z(iMin2, 
					columns - 5) - knots.Z(iMin2, columns - 1)) +
		twelve_div7_hx * (-knots.Dy(iMin1, columns - 1) - knots.Dy(iMin1, 
			columns - 5)) + twelve_div7_hy * (knots.X(iMin2, columns - 2) - 
				knots.X(iMin2, columns - 4)) + three_div7_hy * (knots.Dx(
					i, columns - 1) + knots.Dx(i, columns - 5)) + 
		twentyseven_div7_hxhy * (knots.Z(i, columns - 5) - knots.Z(i, 
			columns - 1)) - thirtysix_div7_hxhy * (knots.Z(iMin1, columns - 1)
				- knots.Z(iMin1, columns - 5) + knots.Z(iMin2, columns - 2) - 
				knots.Z(iMin2, columns - 4)) - six_div_hx * knots.Dy(iMin2, 
					columns - 3) +twelve_div_hy * (knots.Dx(i, columns - 2) + 
						knots.Dx(i, columns - 4)) + onehundredeight_div7_hxhy *
		(knots.Z(i, columns - 2) + knots.Z(i, columns - 4)) - eighteen_div_hx *
		knots.Dy(i, columns - 3) + onehundredfortyfour_div7_hxhy * (knots.Z(
			iMin1, columns - 4) - knots.Z(iMin1, columns - 2)) + 
		twentyfour_div_hx * knots.Z(iMin1, columns - 3) - eta * dlast;
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXDerivations(KnotMatrix& 
	values)
{
	auto h = precalculated_hx_->h;
	auto three_div_h = precalculated_hx_->three_div_h;
	int nrows = values.RowsCount();
	int ncols = values.ColumnsCount();
	utils::For(0, ncols,
	           [&](int j)
	           {
		           FillXDerivations(j, values);
		           for (size_t i = 1; i < nrows - 1; i += 2)
		           {
			           values.SetDx(i, j,
				           0.25 * (three_div_h * (values.Z(i + 1, j) - 
							   values.Z(i - 1, j)) - values.Dx(i + 1, j) + 
							   values.Dx(i - 1, j))
			           );
		           }
	           },
	           1, is_parallel_);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXYDerivations(KnotMatrix& 
	values)
{
//#pragma omp parallel sections
//	{
//#pragma omp section
//		{
//			deboor_.FillXYDerivations(0, values);
//		}
////#pragma omp section
//		{
//			deboor_.FillXYDerivations(values.ColumnsCount() - 1, values);
//		}
////#pragma omp section
//		{
//			deboor_.FillYXDerivations(0, values);
//		}
////#pragma omp section
//		{
//			deboor_.FillYXDerivations(values.RowsCount() - 1, values);
//		}
//	}
	deboor_.FillXYDerivations(0, values);
	deboor_.FillXYDerivations(values.ColumnsCount() - 1, values);
	deboor_.FillYXDerivations(0, values);
	deboor_.FillYXDerivations(values.RowsCount() - 1, values);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYDerivations(KnotMatrix& 
	values)
{
	auto h = precalculated_hy_->h;
	auto three_div_h = precalculated_hy_->three_div_h;
	int ncols = values.ColumnsCount();
	int nrows = values.RowsCount();
	utils::For(0, nrows,
	           [&](int i)
	           {
		           FillYDerivations(i, values);
		           for (size_t j = 1; j < ncols - 1; j += 2)
		           {
			           values.SetDy(i, j, 
						   0.25 * (three_div_h * (values.Z(i, j + 1) - 
								values.Z(i, j - 1)) - values.Dy(i, j + 1) + 
							   values.Dy(i, j - 1))
			           );
		           }
	           },
	           1, is_parallel_);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(KnotMatrix& 
	values)
{
	int nrows = values.RowsCount();

	utils::For(2, nrows,
	           [&](int i)
	           {
		           FillYXDerivations(i, values);
	           },
	           2, is_parallel_);

	auto one_div_16 = 1.0 / 16.0;
	auto hy = precalculated_hy_->h;

	auto three_div_16hy = precalculated_hy_->three_div_h * one_div_16;
	auto three_div_16hx = precalculated_hy_->three_div_h * one_div_16;

	int ncols = values.ColumnsCount();
	// Rest 1
	utils::For(1, nrows - 1,
	           [&](int i)
	           {
		           for (size_t j = 1; j < ncols - 1; j += 2)
		           {
			           values.SetDxy(i, j,
				           one_div_16 *
				           (values.Dxy(i + 1, j + 1) + values.Dxy(i + 1, 
							   j - 1) + values.Dxy(i - 1, j + 1) + values.Dxy(
								   i - 1, j - 1))
				           - three_div_16hy *
				           (values.Dx(i + 1, j + 1) + values.Dx(i + 1, j - 1) 
							   + values.Dx(i - 1, j + 1) + values.Dx(i - 1, 
								   j - 1))
				           - three_div_16hx * (values.Dy(i + 1, j + 1) + 
							   values.Dy(i + 1, j - 1) + values.Dy(i - 1, 
								   j + 1) + values.Dy(i - 1, j - 1))
			           );
		           }
	           },
	           2, is_parallel_);

	auto three_div_hy = 0.75 / hy;

	// Rests 2,3
	for (size_t j = 2; j < ncols - 2; j += 2)
	{
		values.SetDxy(1,j,
			0.25 * (three_div_hy * (values.Dx(1, j + 1) - values.Dx(1, j - 1))
				- values.Dxy(1, j + 1) - values.Dxy(1, j - 1))
		);
	}
	utils::For(2, nrows - 2,
	           [&](int i)
	           {
		           for (size_t j = 2; j < ncols - 2; j += 2)
		           {
			           values.SetDxy(i + 1, j,
				           0.25 * (three_div_hy * (values.Dx(i + 1, j + 1) - 
							   values.Dx(i + 1, j - 1)) - values.Dxy(i + 1, 
								   j + 1) - values.Dxy(i + 1, j - 1))
			           );
		           }
		           for (size_t j = 1; j < ncols - 2; j += 2)
		           {
			           values.SetDxy(i,j,
				           0.25 * (three_div_hy * (values.Dx(i, j + 1) - 
							   values.Dx(i, j - 1)) - values.Dxy(i, j + 1) - 
							   values.Dxy(i, j - 1))
			           );
		           }
	           },
	           2, is_parallel_);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXDerivations(const int 
	column_index, KnotMatrix& values)
{
	auto unknowns_count = values.RowsCount() - 2;
	if (unknowns_count == 0) return;

	auto dset = [&](int index, double value)
	{
		values.SetDx(index, column_index, value);
	};
	auto rget = [&](int index)
	{
		return values.Z(index, column_index);
	};

	auto dlast = values.Dx(values.RowsCount() - 1, column_index);
	auto dfirst = values.Dx(0, column_index);

	SolveTridiagonal(rget, *precalculated_hx_, dfirst, dlast, unknowns_count, 
		dset);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillXYDerivations(const int 
	column_index, KnotMatrix& values)
{
	auto unknowns_count = values.RowsCount() - 2;
	if (unknowns_count == 0) return;

	auto dset = [&](int index, double value)
	{
		values.SetDxy(index, column_index, value);
	};
	auto rget = [&](int index)
	{
		return values.Dy(index, column_index);
	};

	auto h = precalculated_hx_->h;
	auto dlast = values.Dxy(values.RowsCount() - 1, column_index);
	auto dfirst = values.Dxy(0, column_index);

	SolveTridiagonal(rget, *precalculated_hx_, dfirst, dlast, unknowns_count, dset);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYDerivations(const int
	row_index, KnotMatrix& values)
{
	auto unknowns_count = values.ColumnsCount() - 2;
	if (unknowns_count == 0) return;

	auto dset = [&](int index, double value)
	{
		values.SetDy(row_index, index, value);
	};
	auto rget = [&](int index)
	{
		return values.Z(row_index, index);
	};

	auto dlast = values.Dy(row_index, values.ColumnsCount() - 1);
	auto dfirst = values.Dy(row_index, 0);

	SolveTridiagonal(rget, *precalculated_hy_, dfirst, dlast, unknowns_count, 
		dset);
}

void splineknots::ReducedDeBoorKnotsGenerator::FillYXDerivations(const int 
	row_index, KnotMatrix& values)
{
	auto unknowns_count = values.ColumnsCount() - 2;
	if (unknowns_count == 0) return;
	auto h = values.Y(0, 1) - values.Y(0, 0);
	auto dfirst = values.Dxy(row_index, 0);
	auto dlast = values.Dxy(row_index, values.ColumnsCount() - 1);
	auto& tridiagonal = *tridagonals_[omp_get_thread_num()].get();
	auto result_buffer = tridiagonal.RightSideBuffer();
	RightSideCross(values, row_index, dfirst, dlast, unknowns_count, 
		result_buffer);
	tridiagonal.Solve(unknowns_count);
	for (size_t i = 0; i < unknowns_count / 2; i++)
	{
		values.SetDxy(row_index, 2 * i + 1, result_buffer[i]);
	}
}

const ::splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedReducedCross& 
splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedCross() const
{
	return *precalculated_reduced_cross_;
}

void splineknots::ReducedDeBoorKnotsGenerator::SetPrecalculatedCross(
	std::unique_ptr<PrecalculatedReducedCross> precalculated_reduced_cross)
{
	precalculated_reduced_cross_ = std::move(precalculated_reduced_cross);
}

void splineknots::ReducedDeBoorKnotsGenerator::InitializeBuffers(const size_t 
	u_count, const size_t v_count)
{
	auto size = max(u_count / 2 - 1, v_count / 2 - 1);
	auto& trid = tridagonals_;
	for (size_t i = 0; i < trid.size(); i++)
	{
		trid[i]->ResizeBuffers(size);
	}
	deboor_.InitializeBuffers(u_count, v_count);
}


splineknots::KnotMatrix splineknots::ReducedDeBoorKnotsGenerator::
GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& 
	vdimension, double* calculation_time)
{
	StopWatch sw;
	sw.Start();
	if (udimension.knot_count < 6 || vdimension.knot_count < 6)
	{
		deboor_.InitializeBuffers(udimension.knot_count, 
			vdimension.knot_count);
		return deboor_.GenerateKnots(udimension, vdimension);
	}
	KnotMatrix values(udimension, vdimension);
	InitializeBuffers(udimension.knot_count, vdimension.knot_count);
	
	InitializeKnots(udimension, vdimension, values);
	FillXDerivations(values);
	FillYDerivations(values);
	FillXYDerivations(values);
	FillYXDerivations(values);
	sw.Stop();
	if (calculation_time != nullptr)
	{
		*calculation_time = sw.EllapsedTime();
	}
	return values;
}

void splineknots::ReducedDeBoorKnotsGenerator::InParallel(bool value)
{
	is_parallel_ = value;
	auto threads = utils::num_threads;
	if (value)
	{
		tridagonals_.reserve(threads);
		for (auto i = tridagonals_.size(); i < threads; i++)
		{
			// create copy of tridiagonal solver
			auto copy_of_first_ptr = new ReducedDeboorTridiagonal(
				*tridagonals_[0]);
			std::unique_ptr<ReducedDeboorTridiagonal> copy_of_first(
				copy_of_first_ptr);
			copy_of_first_ptr = nullptr;
			tridagonals_.push_back(std::move(copy_of_first));
		}
	}
	else
	{
		tridagonals_._Pop_back_n(tridagonals_.size() - 1);
	}
}

splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedReduced::
PrecalculatedReduced(const double h) : DeBoorKnotsGenerator::Precalculated(h)
{
	six_div_h = 2 * three_div_h;
	twelve_div_h = 2 * six_div_h;
	eighteen_div_h = 3 * six_div_h;
	twentyfour_div_h = 2 * twelve_div_h;
	three_div_7h = (1 / 7) * three_div_h;
	nine_div_7h = 3 * three_div_h;
	twelve_div_7h = 4 * three_div_h;
}

splineknots::ReducedDeBoorKnotsGenerator::PrecalculatedReducedCross::
PrecalculatedReducedCross(const PrecalculatedReduced& precalculated_hx, 
	const PrecalculatedReduced& precalculated_hy)
{
	nine_div_7hxhy = (3 / 7) * precalculated_hy.three_div_h / 
		precalculated_hx.h;
	twentyseven_div_7hxhy = 3 * nine_div_7hxhy;
	thirtysix_div_7hxhy = 4 * nine_div_7hxhy;
	onehundredeight_div_7hxhy = 3 * thirtysix_div_7hxhy;
	onehundredfortyfour_div_7hxhy = 4 * thirtysix_div_7hxhy;
}

void splineknots::ReducedDeBoorKnotsGenerator::InitializeKnots(
	const SurfaceDimension& udimension, const SurfaceDimension& vdimension, 
	KnotMatrix& values)
{
	Precalculate(udimension, vdimension);
	deboor_.InitializeKnots(udimension, vdimension, values);
}

void splineknots::ReducedDeBoorKnotsGenerator::Precalculate(
	const SurfaceDimension& udimension, const SurfaceDimension& vdimension)
{
	precalculated_hx_ = std::make_unique<PrecalculatedReduced>(abs(
		udimension.max - udimension.min) / (udimension.knot_count - 1));
	precalculated_hy_ = std::make_unique<PrecalculatedReduced>(abs(
		vdimension.max - vdimension.min) / (vdimension.knot_count - 1));
	SetPrecalculatedCross(std::make_unique<PrecalculatedReducedCross>(
		*precalculated_hx_, *precalculated_hy_));
}
