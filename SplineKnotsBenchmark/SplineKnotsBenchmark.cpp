// SplineKnotsBenchmark.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "SplineKnots.h"
#include "StopWatch.h"
#include <iostream>
#include <algorithm>
#include "MulVsDiv.h"
#include "CurveDeBoorKnotsGenerator.h"
#include "ComparisonBenchmarkResult.h"

typedef std::chrono::high_resolution_clock Clock;
typedef std::chrono::steady_clock::time_point TimePoint;
typedef std::chrono::duration<long long, std::nano> Duration;

void MulDivBenchmark()
{
	MulVsDiv bencher;
	bencher.BenchAll();
}


ComparisonBenchmarkResult CurveBenchmark(int num_iterations, int num_knots)
{
	const int num_repetitions = 100;
	splineknots::MathFunction function = [](double x, double y)
		{
			return sin(sqrt(x * x + y * y));
		};

	splineknots::CurveDeBoorKnotsGenerator full(std::make_unique<splineknots::DeBoorKnotsGenerator>(function));
	splineknots::CurveDeBoorKnotsGenerator reduced(std::make_unique<splineknots::ReducedDeBoorKnotsGenerator>(function));
	full.WrappedGenerator().Tridiagonal().Resize(num_knots); 
	reduced.WrappedGenerator().Tridiagonal().Resize(num_knots);
	splineknots::SurfaceDimension udimension(-3, 3, num_knots);

	std::vector<splineknots::KnotMatrix> calculated_results;
	std::vector<unsigned int> full_times;
	full_times.reserve(num_iterations);
	std::vector<unsigned int> reduced_times;
	reduced_times.reserve(num_iterations);
	calculated_results.reserve(num_iterations * 2 * num_repetitions);

	unsigned int start;// = clock();
	unsigned int finish;

	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		for (size_t i = 0; i < num_repetitions; i++)
		{
			auto result = reduced.GenerateKnots(udimension);
			calculated_results.push_back(std::move(result));
		}
		finish = clock();
		reduced_times.push_back(finish - start);
	}

	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		for (size_t j = 0; j < num_repetitions; j++)
		{
			auto result = full.GenerateKnots(udimension);
			calculated_results.push_back(std::move(result));
		}
		finish = clock();

		full_times.push_back(finish - start);
	}

	auto full_time = *std::min_element(full_times.begin(), full_times.end());//full_times[full_times.size()/2];//utils::Average(&full_times.front(),full_times.size());//clock() - start;;//sw.Elapsed<std::chrono::microseconds>();

	auto reduced_time = *std::min_element(reduced_times.begin(), reduced_times.end());//reduced_times[reduced_times.size() / 2]; //utils::Average(&reduced_times.front(), reduced_times.size());//clock() - start;//sw.Elapsed<std::chrono::milliseconds>();
	//std::cout << std::endl << std::endl << "Just ignore it : " << calculated_results[num_iterations]->operator()(0, 0).Dx() << std::endl;
	return ComparisonBenchmarkResult(full_time, reduced_time);
}

ComparisonBenchmarkResult SurfaceBenchmark(int num_iterations, int num_knots, bool in_parallel = false)
{
	splineknots::MathFunction function = [](double x, double y)
		{
			return sin(sqrt(x * x + y * y));
		};

	splineknots::DeBoorKnotsGenerator full(function);
	splineknots::ReducedDeBoorKnotsGenerator reduced(function);
	full.InParallel(in_parallel);
	reduced.InParallel(in_parallel);
	splineknots::SurfaceDimension udimension(-3, 3, num_knots);
	splineknots::SurfaceDimension vdimension(udimension);

	std::vector<splineknots::KnotMatrix> calculated_results;
	std::vector<unsigned int> full_times;
	full_times.reserve(num_iterations);
	std::vector<unsigned int> reduced_times;
	reduced_times.reserve(num_iterations);
	calculated_results.reserve(num_iterations * 2);

	unsigned int start;// = clock();
	unsigned int finish;

	//const int dumb_interations = 3;
	//std::vector<std::unique_ptr<splineknots::KnotMatrix>> dumb_results;
	//std::vector<unsigned int> dumb_times;
	//dumb_times.reserve(dumb_interations * 2);
	//calculated_results.reserve(dumb_interations * 4);

	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		auto result = reduced.GenerateKnots(udimension, vdimension);
		finish = clock();
		//result.Print();
		
		calculated_results.push_back(std::move(result));
		reduced_times.push_back(finish - start);
	}

	for (size_t i = 0; i < num_iterations; i++)
	{
		start = clock();
		auto result = full.GenerateKnots(udimension, vdimension);
		finish = clock();
		//result.Print();
		
		calculated_results.push_back(std::move(result));
		full_times.push_back(finish - start);
	}



	auto full_time = *std::min_element(full_times.begin(), full_times.end());//full_times[full_times.size()/2];//utils::Average(&full_times.front(),full_times.size());//clock() - start;;//sw.Elapsed<std::chrono::microseconds>();

	auto reduced_time = *std::min_element(reduced_times.begin(), reduced_times.end());//reduced_times[reduced_times.size() / 2]; //utils::Average(&reduced_times.front(), reduced_times.size());//clock() - start;//sw.Elapsed<std::chrono::milliseconds>();
	//std::cout << std::endl << std::endl << "Just ignore it : " << calculated_results[num_iterations]->operator()(0, 0).Dx() << std::endl;
	return ComparisonBenchmarkResult(full_time, reduced_time);


	//std::cout << std::endl << std::endl << "Just ignore it : " << calculated_results[num_iterations]->operator()(0, 0).Dx() + dumb1->operator()(0, 0).Dx() + dumb2->operator()(0, 0).Dx() << dumbfinish << std::endl;
}

void PrintDeboorResult(ComparisonBenchmarkResult& result)
{
	std::cout << "Full : " << result.FirstAlg() << std::endl;
	std::cout << "Reduced : " << result.SecondAlg() << std::endl;
	std::cout << "Difference : " << result.Ratio() << std::endl;
}

int main()
{
	bool repeat = true;

	while (repeat)
	{
		/*auto dumb = SurfaceBenchmark(1, 199);
		std::cout << "Random number (ignore it)" << dumb.Ratio() << std::endl << std::endl;*/
		std::cout << clock();
		//std::this_thread::sleep_for(std::chrono::milliseconds(500));
		// Console clear ...
		// ... for Windows, 
		system("cls");
		// ... for Linux/Unix. 
		//system("clear");
		std::cout << "1: Multiplication vs division benchmark." << std::endl;
		std::cout << "2: Spline curve benchmark." << std::endl;
		std::cout << "3: Spline surface benchmark." << std::endl;
		std::cout << "4: Spline surface benchmark (in parallel)." << std::endl;
		std::cout << "Q: End program" << std::endl;
		char input;
		std::cin >> input;
		std::cout << std::endl << "---------------" << std::endl;
		unsigned int num_iterations;
		unsigned int num_knots;
		ComparisonBenchmarkResult result(1, 1);
		switch (input)
		{
		case '1':
			std::cout << std::endl << "---------------" << std::endl;
			std::cout << "Multiplication vs division benchmark" << std::endl << std::endl;
			MulDivBenchmark();
			break;
		case '2':
			std::cout << std::endl << "---------------" << std::endl;
			std::cout << "Spline curve benchmark" << std::endl << std::endl;
			std::cout << "Enter number of iterations: " << std::endl;
			std::cin >> num_iterations;
			std::cout << "Enter number of knots: " << std::endl;
			std::cin >> num_knots;
			std::cin.get();
			result = CurveBenchmark(num_iterations, num_knots);
			PrintDeboorResult(result);
			break;
		case '3':

			std::cout << "Spline surface benchmark" << std::endl << std::endl;
			std::cout << "Enter number of iterations: " << std::endl;
			std::cin >> num_iterations;
			std::cout << "Enter number of knots: " << std::endl;
			std::cin >> num_knots;
			std::cin.get();

			result = SurfaceBenchmark(num_iterations, num_knots);
			PrintDeboorResult(result);
			break;
		case '4':
			std::cout << "Parallel spline surface benchmark" << std::endl << std::endl;
			std::cout << "Enter number of iterations: " << std::endl;
			std::cin >> num_iterations;
			std::cout << "Enter number of knots: " << std::endl;
			std::cin >> num_knots;
			std::cin.get();
			result = SurfaceBenchmark(num_iterations, num_knots,true);
			PrintDeboorResult(result);
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
