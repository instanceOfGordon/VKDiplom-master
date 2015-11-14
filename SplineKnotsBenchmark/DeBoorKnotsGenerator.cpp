#include "stdafx.h"
#include "DeBoorKnotsGenerator.h"
#include <complex>
#include <locale>
#include "Tridiagonal.h"


namespace splineknots
{
	
	DeBoorKnotsGenerator::DeBoorKnotsGenerator(MathFunction math_function)
		: KnotsGenerator(math_function),
		tridiagonal_(new utils::Tridiagonal(1,4,1))
	{
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(InterpolativeMathFunction math_function)
		: KnotsGenerator(math_function),
		tridiagonal_(new utils::Tridiagonal(1, 4, 1))
	{
	}

	DeBoorKnotsGenerator::~DeBoorKnotsGenerator()
	{
	}


	std::unique_ptr<KnotMatrix> DeBoorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
	//KnotMatrix*  DeBoorKnotsGenerator::GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension)
	{
		if (udimension.knot_count < 4 || vdimension.knot_count < 4)
		{
			return{};
		}
		auto values = new KnotMatrix(udimension.knot_count, vdimension.knot_count);
		auto& valuesRef = *values;
		InitializeKnots(udimension, vdimension, valuesRef);
		FillXDerivations(valuesRef);
		FillXYDerivations(valuesRef);
		FillYDerivations(valuesRef);
		FillYXDerivations(valuesRef);
		//return values;
		return std::unique_ptr<KnotMatrix>(values);
	}

	std::vector<double> DeBoorKnotsGenerator::RightSide(RightSideSelector& right_side_variables, double h, double dfirst, double dlast, int unknowns_count)
	{
		std::vector<double> rs(unknowns_count);
		h = 3 / h;
		rs[0] = h * (right_side_variables(2) - right_side_variables(0)) - dfirst;
		rs[unknowns_count - 1] = h * (right_side_variables(unknowns_count + 1) - right_side_variables(unknowns_count - 1)) - dlast;
		for (auto i = 1; i < unknowns_count - 1; i++)
		{
			rs[i] = h * (right_side_variables(i + 2) - right_side_variables(i));
		}
		// Optimizing compiler shouldn't return copy of vector instance
		return rs;
	}

	void DeBoorKnotsGenerator::InitializeKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension, KnotMatrix& values)
	{
		auto uSize = abs(udimension.max - udimension.min) / (udimension.knot_count - 1);
		auto vSize = abs(vdimension.max - vdimension.min) / (vdimension.knot_count - 1);
		auto u = udimension.min;
		auto f = Function();
		// Init Z
		for (auto i = 0; i < udimension.knot_count; i++,u += uSize)
		{
			auto v = vdimension.min;
			for (auto j = 0; j < vdimension.knot_count; j++,v += vSize)
			{
				auto z = f.Z()(u, v);
				//Function.Z(u,v); //Z(u, v);
				values(i,j) = Knot(u, v, z);
			}
		}
		// Init Dx
		auto uKnotCountMin1 = udimension.knot_count - 1;
		for (auto j = 0; j < vdimension.knot_count; j++)
		{
			auto dx = f.Dx()(values(0,j).X(), values(0,j).Y());
			values(0,j).SetDx(dx); //Function.Dx(values(0,j).X, values(0,j).Y);
			values(uKnotCountMin1, j).SetDx(f.Dx()(values(uKnotCountMin1, j).X(), values(uKnotCountMin1, j).Y()));
		}
		// Init Dy
		auto vKnotCountMin1 = vdimension.knot_count - 1;
		for (auto i = 0; i < udimension.knot_count; i++)
		{
			values(i,0).SetDy(f.Dy()(values(i,0).X(), values(i,0).Y()));
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
		for (auto j = 0; j < values.ColumnsCount(); j++)
		{
			FillXDerivations(j, values);
		}
	}

	void DeBoorKnotsGenerator::FillXYDerivations(KnotMatrix& values)
	{
		FillXYDerivations(0, values);
		FillXYDerivations(values.ColumnsCount()-1, values);
	}

	void DeBoorKnotsGenerator::FillYDerivations(KnotMatrix& values)
	{
		for (auto i = 0; i < values.RowsCount(); i++)
		{
			FillYDerivations(i, values);
		}
	}

	void DeBoorKnotsGenerator::FillYXDerivations(KnotMatrix& values)
	{
		for (auto i = 0; i < values.RowsCount(); i++)
		{
			FillYXDerivations(i, values);
		}
	}

	void DeBoorKnotsGenerator::FillXDerivations(int column_index, KnotMatrix& values)
	{
		auto unknowns_count = values.RowsCount() - 2;
		if (unknowns_count == 0) return;

		UnknownsSetter dset = [values,column_index](int index, double value)
		{
			values[index][column_index].SetDx(value);
		};
		RightSideSelector rget = [values,column_index](int index)
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

		UnknownsSetter dset = [values, column_index](int index, double value)
		{
			values[index][column_index].SetDxy(value);
		};
		RightSideSelector rget = [values, column_index](int index)
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

		UnknownsSetter dset = [values, row_index](int index, double value)
		{
			values[row_index][index].SetDy(value);
		};
		RightSideSelector rget = [values, row_index](int index)
		{
			return values(row_index, index).Z();
		};

		auto h = values(0, 1).Y() - values(0, 0).Y();
		auto dlast = values(row_index, values.ColumnsCount()- 1).Dy();
		auto dfirst = values(row_index, 0).Dy();

		SolveTridiagonal(rget, h, dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::FillYXDerivations(int row_index, KnotMatrix& values)
	{
		auto unknowns_count = values.ColumnsCount() - 2;
		if (unknowns_count == 0) return;

		UnknownsSetter dset = [values, row_index](int index, double value)
		{
			values[row_index][index].SetDxy(value);
		};
		RightSideSelector rget = [values, row_index](int index)
		{
			return values(row_index, index).Dx();
		};

		auto h = values(0, 1).Y() - values(0, 0).Y();
		auto dlast = values(row_index, values.ColumnsCount() - 1).Dxy();
		auto dfirst = values(row_index, 0).Dxy();

		SolveTridiagonal(rget, h, dfirst, dlast, unknowns_count, dset);
	}

	void DeBoorKnotsGenerator::SolveTridiagonal(RightSideSelector& selector, double h, double dfirst, double dlast, int unknowns_count, UnknownsSetter& unknowns_setter)
	{
		auto result = RightSide(selector, h, dfirst, dlast, unknowns_count);
		tridiagonal_->Solve(unknowns_count, &result.front());
		for (size_t k = 0; k < result.size(); k++)
		{
			unknowns_setter(k + 1, result[k]);
		}
	}

	utils::Tridiagonal& DeBoorKnotsGenerator::Tridiagonal()
	{
		return *tridiagonal_;
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(MathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal)
		: KnotsGenerator(math_function),
		tridiagonal_(std::move(tridiagonal))
	{
	}

	DeBoorKnotsGenerator::DeBoorKnotsGenerator(InterpolativeMathFunction math_function, std::unique_ptr<utils::Tridiagonal> tridiagonal)
		: KnotsGenerator(math_function),
		tridiagonal_(std::move(tridiagonal))
	{
	}
}
