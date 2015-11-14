// SplineKnotsBenchmark.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "SplineKnots.h"
#include "StopWatch.h"
#include <iostream>
#include <algorithm>
#include "MulVsDiv.h"
#include "CurveDeBoorKnotsGenerator.h"

void MulDivBenchmark()
{
	std::cout << std::endl << "---------------" << std::endl;
	std::cout  << "Multiplication vs division benchmark" << std::endl << std::endl;
	MulVsDiv bencher;
	bencher.BenchAll();
}

typedef std::chrono::high_resolution_clock Clock;

void CurveBenchmark()
{
	std::cout << std::endl << "---------------" << std::endl;
	std::cout << "Spline curve benchmark" << std::endl << std::endl;
	std::cout << "Enter number of iterations: " << std::endl;
	unsigned int num_iterations = 0;
	std::cin >> num_iterations;
	std::cout << "Enter number of knots: " << std::endl;
	unsigned int num_knots = 0;
	std::cin >> num_knots;
	std::cin.get();
	splineknots::MathFunction function = [](double x, double y)
	{
		return sin(sqrt(x*x + y*y));
	};

	splineknots::CurveKnotsGenerator<splineknots::DeBoorKnotsGenerator> full(function);
	splineknots::CurveKnotsGenerator<splineknots::ReducedDeboorKnotsGenerator> reduced(function);
	splineknots::SurfaceDimension udimension(-3, 3, num_knots);

	std::vector<std::unique_ptr<splineknots::KnotMatrix>> calculated_results;
	std::vector<unsigned int> full_times;
	full_times.reserve(num_iterations);
	std::vector<unsigned int> reduced_times;
	reduced_times.reserve(num_iterations);
	calculated_results.reserve(num_iterations * 2);
	auto dumbstart = clock();
	auto dumb1 = full.GenerateKnots(udimension);
	auto dumb2 = reduced.GenerateKnots(udimension);
	auto dumbfinish = dumbstart - clock();
	unsigned int start;// = clock();
	unsigned int finish;
	//dumbfinish += start / finish;
	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		auto result = full.GenerateKnots(udimension);
		finish = clock();
		calculated_results.push_back(move(result));
		full_times.push_back(finish - start);

	}
	std::sort(full_times.begin(), full_times.end());
	auto full_time = full_times[full_times.size() / 2];//utils::Average(&full_times.front(),full_times.size());//clock() - start;;//sw.Elapsed<std::chrono::microseconds>();
	std::cout << "Full : " << full_time << std::endl;

	start = clock();

	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		auto result = reduced.GenerateKnots(udimension);
		finish = clock();
		calculated_results.push_back(move(result));
		reduced_times.push_back(finish - start);

	}
	std::sort(reduced_times.begin(), reduced_times.end());
	auto reduced_time = reduced_times[reduced_times.size() / 2]; //utils::Average(&reduced_times.front(), reduced_times.size());//clock() - start;//sw.Elapsed<std::chrono::milliseconds>();



	std::cout << "Reduced : " << reduced_time << std::endl;
	std::cout << "Difference : " << static_cast<double>(full_time) / static_cast<double>(reduced_time) << std::endl;
	std::cout << std::endl << std::endl << "Just ignore it : " << calculated_results[num_iterations]->operator()(0, 0).Dx() + dumb1->operator()(0, 0).Dx() + dumb2->operator()(0, 0).Dx()<< dumbfinish << std::endl;
}

void SurfaceBenchmark()
{
	std::cout << std::endl << "---------------" << std::endl;
	std::cout << "Spline surface benchmark" << std::endl << std::endl;
	std::cout << "Enter number of iterations: " << std::endl;
	unsigned int num_iterations = 0;
	std::cin >> num_iterations;
	std::cout << "Enter number of knots: " << std::endl;
	unsigned int num_knots = 0;
	std::cin >> num_knots;
	std::cin.get();
	splineknots::MathFunction function = [](double x, double y)
		{
			return sin(sqrt(x*x + y*y));
		};

	splineknots::DeBoorKnotsGenerator full(function);
	splineknots::ReducedDeboorKnotsGenerator reduced(function);
	splineknots::SurfaceDimension udimension(-3, 3, num_knots);
	splineknots::SurfaceDimension vdimension(udimension);

	std::vector<std::unique_ptr<splineknots::KnotMatrix>> calculated_results;
	std::vector<unsigned int> full_times;
	full_times.reserve(num_iterations);
	std::vector<unsigned int> reduced_times;
	reduced_times.reserve(num_iterations);
	calculated_results.reserve(num_iterations * 2);
	auto dumbstart = clock();
	auto dumb1 = full.GenerateKnots(udimension, vdimension);
	auto dumb2 = reduced.GenerateKnots(udimension, vdimension);
	auto dumbfinish = dumbstart - clock();
	unsigned int start;// = clock();
	unsigned int finish;
	//dumbfinish += start / finish;
	for (size_t i = 0; i < num_iterations; i++)
	{
		//clock();
		start = clock();
		auto result = full.GenerateKnots(udimension, vdimension);
		finish = clock();
		calculated_results.push_back(move(result));
		full_times.push_back(finish - start);

	}
	std::sort(full_times.begin(), full_times.end());
	auto full_time = full_times[full_times.size()/2];//utils::Average(&full_times.front(),full_times.size());//clock() - start;;//sw.Elapsed<std::chrono::microseconds>();
	std::cout << "Full : " << full_time << std::endl;

	start = clock();

	for (size_t i = 0; i < num_iterations; i++)
	{
		clock();
		start = clock();
		auto result = reduced.GenerateKnots(udimension, vdimension);
		finish = clock();
		calculated_results.push_back(move(result));
		reduced_times.push_back(finish - start);

	}
	std::sort(reduced_times.begin(), reduced_times.end());
	auto reduced_time = reduced_times[reduced_times.size() / 2]; //utils::Average(&reduced_times.front(), reduced_times.size());//clock() - start;//sw.Elapsed<std::chrono::milliseconds>();


	
	std::cout << "Reduced : " << reduced_time << std::endl;
	std::cout << "Difference : " << static_cast<double>(full_time)/static_cast<double>(reduced_time) << std::endl;
	std::cout << std::endl << std::endl << "Just ignore it : " << calculated_results[num_iterations]->operator()(0, 0).Dx() + dumb1->operator()(0, 0).Dx() + dumb2->operator()(0, 0).Dx() << dumbfinish << std::endl;
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
		std::cout << "1: Multiplication vs division benchmark." << std::endl;
		std::cout << "2: Spline curve benchmark." << std::endl;
		std::cout << "3: Spline surface benchmark." << std::endl;
		std::cout << "Q: End program" << std::endl;
		char input;
		std::cin >> input;
		switch (input)
		{
		case '1':
			MulDivBenchmark();
			break;
		case '2':
			CurveBenchmark();
			break;
		case '3':
			SurfaceBenchmark();
			break;
		case 'q':
		case 'Q':
			repeat = false;
			break;
		}
		std::cout << "===================" << std::endl;
		std::cout << "any key: Restart program." << std::endl;
		std::cout << "Q: End program" << std::endl;
		std::cin >> input;
		if (input == 'Q' || input == 'q')
			repeat = false;
	}
	return 0;
}

