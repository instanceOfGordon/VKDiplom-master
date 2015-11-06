// SplineKnotsBenchmark.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "SplineKnots.h"
#include "StopWatch.h"
#include <iostream>

int main()
{
	const unsigned int num_iterations = 2;
	
	splineknots::MathFunction function = [](double x, double y)
	{
		return sin(sqrt(x*x + y*y));
	};

	splineknots::DeBoorKnotsGenerator full(function);
	splineknots::ReducedDeboorKnotsGenerator reduced(function);
	splineknots::SurfaceDimension udimension(-3, 3, 7);
	splineknots::SurfaceDimension vdimension(udimension);

	double fulldumb = 0;
	//StopWatch<std::chrono::high_resolution_clock> sw;
	unsigned int start = clock();
	for (size_t i = 0; i < num_iterations; i++)
	{
		auto result = full.GenerateKnots(udimension, vdimension);
		fulldumb += result->operator()(1, 1).Dxy();
	}
	auto full_time = clock() - start;;//sw.Elapsed<std::chrono::microseconds>();
	
	//double reduceddumb = 0;
	start = clock();
	//sw.Reset();
	for (size_t i = 0; i < num_iterations; i++)
	{
		auto result = full.GenerateKnots(udimension, vdimension);
		fulldumb += result->operator()(1, 1).Dxy();
	}
	auto reduced_time = clock() - start;//sw.Elapsed<std::chrono::milliseconds>();


	std::cout << "Full : " << full_time << std::endl;
	std::cout << "Reduced : " << full_time << std::endl;
	std::cout << "Difference : " << static_cast<double>(full_time)/static_cast<double>(reduced_time) << std::endl;
	std::cout<< std::endl<< std::endl << "Just ignore it : " << fulldumb << std::endl;
	std::cin.get();
    return 0;
}

