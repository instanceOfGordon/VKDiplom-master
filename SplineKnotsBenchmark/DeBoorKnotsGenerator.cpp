#include "stdafx.h"
#include "DeBoorKnotsGenerator.h"
#include <locale>
#include "Tridiagonal.h"
#include <omp.h>


namespace splineknots
{
	std::vector<std::vector<double>>& DeBoorKnotsGenerator::RightSidesBuffers()
	{
		return rightsides_buffers_;
	}

	Tridiagonals& DeBoorKnotsGenerator::Tridagonals()
	{
		return tridagonals_;
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(MathFunction math_function)
		: KnotsGenerator(math_function),
		  tridagonals_(),
		rightsides_buffers_(1)
	//tridiagonal_(new utils::Tridiagonal(1, 4, 1))
	{
		//tridagonals_.reserve(utils::num_threads);
		tridagonals_.push_back(std::make_unique<utils::Tridiagonal>(1, 4, 1));
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(InterpolativeMathFunction math_function)
		: KnotsGenerator(math_function),
		  tridagonals_(),
		rightsides_buffers_(1)
	{
		//tridagonals_.reserve(utils::num_threads);
		tridagonals_.push_back(std::make_unique<utils::Tridiagonal>(1, 4, 1));
	}


	DeBoorKnotsGenerator::~DeBoorKnotsGenerator()
	{
	}


	KnotMatrix DeBoorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
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

	void DeBoorKnotsGenerator::RightSide(RightSideSelector&  right_side_variables, double h, double dfirst, double dlast,
		int unknowns_count, double* rightside_buffer)
	{
		auto h3 = 3/h;
		rightside_buffer[0] = h * (right_side_variables(2) - right_side_variables(0)) - dfirst;
		rightside_buffer[unknowns_count - 1] = h3 * (right_side_variables(unknowns_count + 1) - right_side_variables(unknowns_count - 1)) - dlast;
		for (auto i = 1; i < unknowns_count - 1; i++)
		{
			rightside_buffer[i] = h3 * (right_side_variables(i + 2) - right_side_variables(i));
		}
	
	}

	void DeBoorKnotsGenerator::InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values)
	{
		auto uSize = abs(udimension.max - udimension.min) / (udimension.knot_count - 1);
		auto vSize = abs(vdimension.max - vdimension.min) / (vdimension.knot_count - 1);
		auto u = udimension.min;
		auto f = Function();
		// Init Z
		for (auto i = 0; i < udimension.knot_count; i++ , u += uSize)
		{
			auto v = vdimension.min;
			for (auto j = 0; j < vdimension.knot_count; j++ , v += vSize)
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
		           1,is_parallel_);
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
		           1,is_parallel_);
	}

	void DeBoorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
	{
		utils::For(0, static_cast<int>(values.RowsCount()),
		           [&](int i)
		           {
			           FillYXDerivations(i, values);
		           },
		           1,is_parallel_);
	}

	void DeBoorKnotsGenerator::FillXDerivations(int column_index, KnotMatrix& values)
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

		auto h = values(1, 0).X() - values(0, 0).X();
		auto dlast = values(values.RowsCount() - 1, column_index).Dx();
		auto dfirst = values(0, column_index).Dx();

		SolveTridiagonal(rget, h, dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillXYDerivations(int column_index, KnotMatrix& values)
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

		auto h = values(1, 0).X() - values(0, 0).X();
		auto dlast = values(values.RowsCount() - 1, column_index).Dxy();
		auto dfirst = values(0, column_index).Dxy();

		SolveTridiagonal(rget, h, dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillYDerivations(int row_index, KnotMatrix& values)
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

		auto h = values(0, 1).Y() - values(0, 0).Y();
		auto dlast = values(row_index, values.ColumnsCount() - 1).Dy();
		auto dfirst = values(row_index, 0).Dy();

		SolveTridiagonal(rget, h, dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillYXDerivations(int row_index, KnotMatrix& values)
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

		auto h = values(0, 1).Y() - values(0, 0).Y();
		auto dlast = values(row_index, values.ColumnsCount() - 1).Dxy();
		auto dfirst = values(row_index, 0).Dxy();

		SolveTridiagonal(rget, h, dfirst, dlast, unknowns_count, dset);
	}

	bool DeBoorKnotsGenerator::IsParallel()
	{
		return is_parallel_;
	}

	void DeBoorKnotsGenerator::SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast, int unknowns_count, UnknownsSetter& unknowns_setter)
	{
		auto results_buffer = RightSideBuffer(omp_get_thread_num());
		RightSide(selector, h, dfirst, dlast, unknowns_count,results_buffer);

		Tridiagonal(omp_get_thread_num()).Solve(unknowns_count, results_buffer);
		for (int k = 0; k < unknowns_count; k++)
		{
			unknowns_setter(k + 1, results_buffer[k]);
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
				std::unique_ptr<utils::Tridiagonal> copy_of_first(tridagonals_[0]->Clone());
				tridagonals_.push_back(std::move(copy_of_first));
			}
			rightsides_buffers_.resize(threads);
		}
		else
		{
			tridagonals_._Pop_back_n(tridagonals_.size() - 1);
			rightsides_buffers_._Pop_back_n(rightsides_buffers_.size() - 1);
		}
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(MathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal)
		: KnotsGenerator(math_function),
		  tridagonals_(),
		rightsides_buffers_(1)	
	{
		tridagonals_.push_back(std::move(tridiagonal));
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(InterpolativeMathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal)
		: KnotsGenerator(math_function),
		tridagonals_(),
		rightsides_buffers_(1)
	{
		tridagonals_.push_back(std::move(tridiagonal));
	}

	void DeBoorKnotsGenerator::InitializeBuffers(size_t u_count, size_t v_count)
	{
		auto size = std::max(u_count - 2, v_count - 2);
		for (size_t i = 0; i <rightsides_buffers_.size(); i++)
		{
			rightsides_buffers_[i].resize(size);
		}
		for (size_t i = 0; i <tridagonals_.size(); i++)
		{
			tridagonals_[i]->ResizeBuffer(size);
		}
	}

	double* DeBoorKnotsGenerator::RightSideBuffer(int index)
	{
		return &rightsides_buffers_[index].front();
	}

	utils::Tridiagonal& DeBoorKnotsGenerator::Tridiagonal(int index)
	{
		return *tridagonals_[index];
	}
}
