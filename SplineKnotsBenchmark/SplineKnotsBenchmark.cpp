// SplineKnotsBenchmark.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "SplineKnots.h"
#include "StopWatch.h"
#include <iostream>
#include <algorithm>

bool Benchmark()
{
	std::cout << "Enter number of iterations: " << std::endl;
	unsigned int num_iterations = 0;
	std::cin >> num_iterations;
	std::cout << "Enter number of knots: " << std::endl;
	unsigned int num_knots = 0;
	std::cin >> num_knots;
	std::cin.get();
	//unsigned int start;
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
	std::vector<unsigned int> full_times;
	full_times.reserve(num_iterations);
	std::vector<unsigned int> reduced_times;
	reduced_times.reserve(num_iterations);
	calculated_results.reserve(num_iterations * 2);
	
	unsigned int start;// = clock();
	unsigned int finish;
	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		auto result = full.GenerateKnots(udimension, vdimension);
		finish = clock();
		calculated_results.push_back(move(result));
		full_times.push_back(finish - start);
		//fulldumb += result->operator()(1, 1).Z();
	}
	std::sort(full_times.begin(), full_times.end());
	auto full_time = full_times[full_times.size()/2];//utils::Average(&full_times.front(),full_times.size());//clock() - start;;//sw.Elapsed<std::chrono::microseconds>();
	std::cout << "Full : " << full_time << std::endl;
	//double reduceddumb = 0;
	start = clock();
	//sw.Reset();
	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		auto result = reduced.GenerateKnots(udimension, vdimension);
		finish = clock();
		calculated_results.push_back(move(result));
		reduced_times.push_back(finish - start);
		//fulldumb += result->operator()(1, 1).Z();
	}
	std::sort(reduced_times.begin(), reduced_times.end());
	auto reduced_time = reduced_times[reduced_times.size() / 2]; //utils::Average(&reduced_times.front(), reduced_times.size());//clock() - start;//sw.Elapsed<std::chrono::milliseconds>();


	
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

