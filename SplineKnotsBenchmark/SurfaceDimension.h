#pragma once
namespace splineknots
{
	struct SurfaceDimension
	{
		const double min, max;
		const int knot_count;
		SurfaceDimension(double min, double max, int knot_count);
	};
}
