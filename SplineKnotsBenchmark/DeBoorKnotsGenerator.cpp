#include "stdafx.h"
#include "DeBoorKnotsGenerator.h"

#include "Tridiagonal.h"
#include <omp.h>
#include "SplineKnots.h"

#include <algorithm>

namespace splineknots
{
	Tridiagonals& DeBoorKnotsGenerator::Tridagonals()
	{
		return tridagonals_;
	}

	DeBoorKnotsGenerator::Precalculated::Precalculated(const double h)
		: h(h), three_div_h(3 / h)
	{
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(MathFunction math_function)
		: KnotsGenerator(math_function),
		tridagonals_(), is_parallel_(false)
	{
		tridagonals_.push_back(std::make_unique<splineknots::Tridiagonal>(1, 4, 1));
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(InterpolativeMathFunction math_function)
		: KnotsGenerator(math_function),
		tridagonals_(), is_parallel_(false)
	{
		tridagonals_.push_back(std::make_unique<splineknots::Tridiagonal>(1, 4, 1));
	}


	DeBoorKnotsGenerator::DeBoorKnotsGenerator(MathFunction math_function, std::unique_ptr<splineknots::Tridiagonal> tridiagonal)
		: KnotsGenerator(math_function),
		tridagonals_(), is_parallel_(false)
	{
		tridagonals_.push_back(std::move(tridiagonal));
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(InterpolativeMathFunction math_function, std::unique_ptr<splineknots::Tridiagonal> tridiagonal)
		: KnotsGenerator(math_function),
		tridagonals_(), is_parallel_(false)
	{
		tridagonals_.push_back(std::move(tridiagonal));
	}

	DeBoorKnotsGenerator::~DeBoorKnotsGenerator()
	{
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(const DeBoorKnotsGenerator& other) :
		KnotsGenerator(other),
		tridagonals_(other.tridagonals_.size()),
		is_parallel_(other.is_parallel_)
	{
		for (size_t i = 0; i < other.tridagonals_.size(); i++)
		{
			auto trid = std::unique_ptr<splineknots::Tridiagonal>(other.tridagonals_[i]->Clone());
			tridagonals_.push_back(std::move(trid));
		}
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(DeBoorKnotsGenerator&& other) :
		KnotsGenerator(std::move(other)),
		tridagonals_(std::move(other.tridagonals_)),
		is_parallel_(other.is_parallel_)
	{
	}

	DeBoorKnotsGenerator& DeBoorKnotsGenerator::operator=(const DeBoorKnotsGenerator& other)
	{
		if (this == &other)
			return *this;
		KnotsGenerator::operator =(other);
		tridagonals_.clear();
		for (size_t i = 0; i < other.tridagonals_.size(); i++)
		{
			auto trid = std::unique_ptr<splineknots::Tridiagonal>(other.tridagonals_[i]->Clone());
			tridagonals_.push_back(std::move(trid));
		}
		is_parallel_ = other.is_parallel_;
		return *this;
	}

	DeBoorKnotsGenerator& DeBoorKnotsGenerator::operator=(DeBoorKnotsGenerator&& other)
	{
		if (this == &other)
			return *this;
		KnotsGenerator::operator =(std::move(other));
		tridagonals_ = std::move(other.tridagonals_);
		is_parallel_ = other.is_parallel_;
		return *this;
	}

	KnotMatrix DeBoorKnotsGenerator::GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension)
	{
		if (udimension.knot_count < 4 || vdimension.knot_count < 4)
		{
			return KnotMatrix::NullMatrix();
		}
		KnotMatrix values(udimension.knot_count, vdimension.knot_count);
		InitializeBuffers(udimension.knot_count, vdimension.knot_count);
		InitializeKnots(udimension, vdimension, values);
		FillXDerivations(values);
		FillXYDerivations(values);
		FillYDerivations(values);
		FillYXDerivations(values);
		return values;
	}

	void DeBoorKnotsGenerator::RightSide(const RightSideSelector& right_side_variables, const Precalculated& precalculated, const double dfirst, const double dlast,
		const int unknowns_count, double* rightside_buffer)
	{
		auto h3 = precalculated.three_div_h;
		rightside_buffer[0] = h3 * (right_side_variables(2) - right_side_variables(0)) - dfirst;
		rightside_buffer[unknowns_count - 1] = h3 * (right_side_variables(unknowns_count + 1) - right_side_variables(unknowns_count - 1)) - dlast;
		for (auto i = 1; i < unknowns_count - 1; i++)
		{
			rightside_buffer[i] = h3 * (right_side_variables(i + 2) - right_side_variables(i));
		}
	}

	void DeBoorKnotsGenerator::Precalculate(const SurfaceDimension& udimension, const SurfaceDimension& vdimension)
	{
		SetPrecalculatedHX(std::make_unique<Precalculated>(abs(udimension.max - udimension.min) / (udimension.knot_count - 1)));
		SetPrecalculatedHY(std::make_unique<Precalculated>(abs(vdimension.max - vdimension.min) / (vdimension.knot_count - 1)));
	}

	void DeBoorKnotsGenerator::InitializeKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension, KnotMatrix& values)
	{
		Precalculate(udimension, vdimension);
		auto hx = precalculated_hx_->h;
		auto hy = precalculated_hy_->h;
		auto u = udimension.min;
		auto f = Function();
		// Init Z
		for (auto i = 0; i < udimension.knot_count; i++, u += hx)
		{
			auto v = vdimension.min;
			for (auto j = 0; j < vdimension.knot_count; j++, v += hy)
			{
				auto z = f.Z()(u, v);
				//Function.Z(u,v); //Z(u, v);
				values(i, j) = Knot(u, v, z);
			}
		}
		// Init Dx
		auto uKnotCountMin1 = udimension.knot_count - 1;
		for (auto j = 0; j < vdimension.knot_count; j++)
		{
			auto dx = f.Dx()(values(0, j).X(), values(0, j).Y());
			values(0, j).SetDx(dx); //Function.Dx(values(0,j).X, values(0,j).Y);
			values(uKnotCountMin1, j).SetDx(f.Dx()(values(uKnotCountMin1, j).X(), values(uKnotCountMin1, j).Y()));
		}
		// Init Dy
		auto vKnotCountMin1 = vdimension.knot_count - 1;
		for (auto i = 0; i < udimension.knot_count; i++)
		{
			values(i, 0).SetDy(f.Dy()(values(i, 0).X(), values(i, 0).Y()));
			values(i, vKnotCountMin1).SetDy(
				f.Dy()(values(i, vKnotCountMin1).X(), values(i, vKnotCountMin1).Y())
				);
		}
		// Init Dxy
		values(0, 0).SetDxy(f.Dxy()(values(0, 0).X(), values(0, 0).Y()));
		values(uKnotCountMin1, 0).SetDxy(f.Dxy()(values(uKnotCountMin1, 0).X(), values(uKnotCountMin1, 0).Y()));
		values(0, vKnotCountMin1).SetDxy(f.Dxy()(values(0, vKnotCountMin1).X(), values(0, vKnotCountMin1).Y()));
		values(uKnotCountMin1, vKnotCountMin1).SetDxy(f.Dxy()(values(uKnotCountMin1, vKnotCountMin1).X(), values(uKnotCountMin1, vKnotCountMin1).Y()));
	}

	void DeBoorKnotsGenerator::FillXDerivations(KnotMatrix& values)
	{
		utils::For(0, static_cast<int>(values.ColumnsCount()),
			[&](int j)
		{
			FillXDerivations(j, values);
		},
			1, is_parallel_);
	}

	void DeBoorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
	{
		FillXYDerivations(0, values);
		FillXYDerivations(values.ColumnsCount() - 1, values);
	}

	void DeBoorKnotsGenerator::FillYDerivations(KnotMatrix& values)
	{
		utils::For(0, static_cast<int>(values.RowsCount()),
			[&](int i)
		{
			FillYDerivations(i, values);
		},
			1, is_parallel_);
	}

	void DeBoorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
	{
		utils::For(0, static_cast<int>(values.RowsCount()),
			[&](int i)
		{
			FillYXDerivations(i, values);
		},
			1, is_parallel_);
	}

	void DeBoorKnotsGenerator::FillXDerivations(const int column_index, KnotMatrix& values)
	{
		auto unknowns_count = values.RowsCount() - 2;
		if (unknowns_count == 0) return;

		UnknownsSetter dset = [&](int index, double value)
		{
			values[index][column_index].SetDx(value);
		};
		RightSideSelector rget = [&](int index)
		{
			return values[index][column_index].Z();
		};

		auto dlast = values(values.RowsCount() - 1, column_index).Dx();
		auto dfirst = values(0, column_index).Dx();

		SolveTridiagonal(rget, PrecalculatedHX(), dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillXYDerivations(const int column_index, KnotMatrix& values)
	{
		auto unknowns_count = values.RowsCount() - 2;
		if (unknowns_count == 0) return;

		UnknownsSetter dset = [&](int index, double value)
		{
			values[index][column_index].SetDxy(value);
		};
		RightSideSelector rget = [&](int index)
		{
			return values(index, column_index).Dy();
		};

		auto h = precalculated_hx_->h;
		auto dlast = values(values.RowsCount() - 1, column_index).Dxy();
		auto dfirst = values(0, column_index).Dxy();

		SolveTridiagonal(rget, PrecalculatedHX(), dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillYDerivations(const int row_index, KnotMatrix& values)
	{
		auto unknowns_count = values.ColumnsCount() - 2;
		if (unknowns_count == 0) return;

		UnknownsSetter dset = [&](int index, double value)
		{
			values[row_index][index].SetDy(value);
		};
		RightSideSelector rget = [&](int index)
		{
			return values(row_index, index).Z();
		};

		auto dlast = values(row_index, values.ColumnsCount() - 1).Dy();
		auto dfirst = values(row_index, 0).Dy();

		SolveTridiagonal(rget, PrecalculatedHY(), dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillYXDerivations(const int row_index, KnotMatrix& values)
	{
		auto unknowns_count = values.ColumnsCount() - 2;
		if (unknowns_count == 0) return;

		UnknownsSetter dset = [&](int index, double value)
		{
			values[row_index][index].SetDxy(value);
		};
		RightSideSelector rget = [&](int index)
		{
			return values(row_index, index).Dx();
		};

		auto dlast = values(row_index, values.ColumnsCount() - 1).Dxy();
		auto dfirst = values(row_index, 0).Dxy();

		SolveTridiagonal(rget, PrecalculatedHY(), dfirst, dlast, unknowns_count, dset);
	}

	bool DeBoorKnotsGenerator::IsParallel()
	{
		return is_parallel_;
	}

	const DeBoorKnotsGenerator::Precalculated& DeBoorKnotsGenerator::PrecalculatedHX() const
	{
		return *precalculated_hx_;
	}

	const DeBoorKnotsGenerator::Precalculated& DeBoorKnotsGenerator::PrecalculatedHY() const
	{
		return *precalculated_hy_;
	}

	void DeBoorKnotsGenerator::SetPrecalculatedHX(std::unique_ptr<Precalculated> precalculated)
	{
		precalculated_hx_ = std::move(precalculated);
	}

	void DeBoorKnotsGenerator::SetPrecalculatedHY(std::unique_ptr<Precalculated> precalculated)
	{
		precalculated_hy_ = std::move(precalculated);
	}

	void DeBoorKnotsGenerator::SolveTridiagonal(const RightSideSelector& selector, const Precalculated& precalculated, const double dfirst, const double dlast, const int unknowns_count, UnknownsSetter& unknowns_setter)
	{
		auto& tridiagonal = Tridiagonal(omp_get_thread_num());
		auto rightside = tridiagonal.RightSideBuffer();
		RightSide(selector, precalculated, dfirst, dlast, unknowns_count, rightside);

		tridiagonal.Solve(unknowns_count);
		for (int k = 0; k < unknowns_count; k++)
		{
			unknowns_setter(k + 1, rightside[k]);
		}
	}

	void DeBoorKnotsGenerator::InParallel(bool value)
	{
		is_parallel_ = value;
		auto threads = utils::num_threads;
		if (value)
		{
			tridagonals_.reserve(threads);
			for (auto i = tridagonals_.size(); i < threads; i++)
			{
				// create copy of tridiagonal solver
				std::unique_ptr<splineknots::Tridiagonal> copy_of_first(tridagonals_[0]->Clone());
				tridagonals_.push_back(std::move(copy_of_first));
			}
		}
		else
		{
			tridagonals_._Pop_back_n(tridagonals_.size() - 1);
		}
	}

	void DeBoorKnotsGenerator::InitializeBuffers(const size_t u_count, const size_t v_count)
	{
		auto size = std::max(u_count - 2, v_count - 2);
		for (size_t i = 0; i < tridagonals_.size(); i++)
		{
			tridagonals_[i]->ResizeBuffers(size);
		}
	}

	splineknots::Tridiagonal& DeBoorKnotsGenerator::Tridiagonal(int index)
	{
		return *tridagonals_[index];
	}
}
