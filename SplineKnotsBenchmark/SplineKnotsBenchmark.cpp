// SplineKnotsBenchmark.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "SplineKnots.h"
#include "StopWatch.h"
#include <iostream>

bool Benchmark()
{
	std::cout << "Enter number of iterations: " << std::endl;
	unsigned int num_iterations = 0;
	std::cin >> num_iterations;
	std::cout << "Enter number of knots: " << std::endl;
	unsigned int num_knots = 0;
	std::cin >> num_knots;
	std::cin.get();
	unsigned int start;
	splineknots::MathFunction function = [](double x, double y)
		{
			return sin(sqrt(x*x + y*y));
		};

	splineknots::DeBoorKnotsGenerator full(function);
	splineknots::ReducedDeboorKnotsGenerator reduced(function);
	splineknots::SurfaceDimension udimension(-3, 3, num_knots);
	splineknots::SurfaceDimension vdimension(udimension);

	//double fulldumb = 0;
	//StopWatch<std::chrono::high_resolution_clock> sw;
	std::vector<std::unique_ptr<splineknots::KnotMatrix>> calculated_results;
	calculated_results.reserve(num_iterations * 2);
	
	start = clock();
	for (size_t i = 0; i < num_iterations; i++)
	{
		auto result = full.GenerateKnots(udimension, vdimension);
		calculated_results.push_back(move(result));
		//fulldumb += result->operator()(1, 1).Z();
	}
	auto full_time = clock() - start;;//sw.Elapsed<std::chrono::microseconds>();
	std::cout << "Full : " << full_time << std::endl;
	//double reduceddumb = 0;
	start = clock();
	//sw.Reset();
	for (size_t i = 0; i < num_iterations; i++)
	{
		auto result = reduced.GenerateKnots(udimension, vdimension);
		calculated_results.push_back(move(result));
		//fulldumb += result->operator()(1, 1).Z();
	}
	auto reduced_time = clock() - start;//sw.Elapsed<std::chrono::milliseconds>();


	
	std::cout << "Reduced : " << reduced_time << std::endl;
	std::cout << "Difference : " << static_cast<double>(full_time)/static_cast<double>(reduced_time) << std::endl;
	std::cout<< std::endl<< std::endl << "Just ignore it : " << calculated_results[num_iterations]->operator()(0,0).Dx() << std::endl;
	std::cout << std::endl << "Press any key to repeat benchmark." << std::endl;
	std::cout << std::endl << "Or press 'Q' to quit." << std::endl;
	//calculated_results.clear();
	/*for (auto& m : calculated_results)
	{
		delete m;
		m = nullptr;
	}*/
	char input;
	std::cin >> input;
	if (input == 'Q' || input == 'q')
		return false;
	return true;
}

int main()
{
	bool repeat = true;
	while (repeat) {
		// Console clear ...
		// ... for Windows, 
		system("cls");
		// ... for Linux/Unix. 
		//system("clear");
		repeat = Benchmark();
	}
	return 0;
}

