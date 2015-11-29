#include "stdafx.h"
#include "MulVsDiv.h"
#include <ostream>
#include <iostream>
#include <locale>
#include <vector>
#include <algorithm>
#include <random>



void MulVsDiv::Loop()
{
	const int length = 64;
	const int loops = 10e7;
	std::cout << "Loop:\n---" << std::endl;
	double a[length], b[length], c[length];
	for (size_t i = 0; i < length; i++)
	{
		a[i] = (rand() % 256) / 16.0 + 1.3;
		b[i] = (rand() % 256) / 16.0 + 1.6;
		c[i] = (rand() % 256) / 16.0 + 1.1;
	}
	std::random_device rd; // obtain a random number from hardware
	std::mt19937 eng(rd()); // seed the generator
	std::uniform_int_distribution<> distr(0, length-1); // define the range
	double ignoreit = 0;
	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		// MSVC cannot vectorize this loop (message 1300).
		// However, if this loop will not be nested in, autovectorization will happen.
		// Same condition apply for mul/div/rcp loops

		// ICL does not have this issue, but to provide both vectorized and nonvectorized comparison 
		// i specifically disabled vectorization in this method
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = a[i] + b[i];
		}
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = a[i] * b[i];
		}
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = a[i] / b[i];
		}
	}
	auto div_time = clock() - start;
	std::cout << "Division: " << div_time << std::endl;

	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl ;
	//std::cout << "Multiplication faster than reciprocal: " << static_cast<double>(rcp_time) / static_cast<double>(mul_time) << std::endl << std::endl;
	std::cout << "Just ignore it: " <<ignoreit << std::endl << std::endl;
}

void MulVsDiv::LoopVectorized()
{
	const int length = 64;
	const int loops = 10e7;
	std::cout << "Vectorized loop:\n---" << std::endl;
	double a[length], b[length], c[length];
	for (size_t i = 0; i < length; i++)
	{
		a[i] = (rand() % 256) / 16.0 + 1.3;
		b[i] = (rand() % 256) / 16.0 + 1.6;
		c[i] = (rand() % 256) / 16.0 + 1.1;
	}
	std::random_device rd; // obtain a random number from hardware
	std::mt19937 eng(rd()); // seed the generator
	std::uniform_int_distribution<> distr(0, length - 1); // define the range
	auto ignoreit = 0;
	int l = 0;
	auto start = clock();
add:
	for (int i = 0; i < length; i++)
	{
		a[i] = a[i] + b[i];
	}
	// MSVC doesn't vectorize nested loops (message 1300 - too little computation to vectorize) as mentioned on line 23 in function 'Loop'.
	// However if nested loop is replaced with this nasty workaround SIMD vectorization will happen.
	// Same condition apply for mul/div/rcp loops
	while (l < loops)
	{
		++l;
		goto add;
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	l = 0;
	start = clock();
mul:
	for (int i = 0; i < length; ++i)
	{
		a[i] = a[i] * b[i];
	}
	while (l < loops)
	{
		++l;
		goto mul;
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	l = 0;
	start = clock();
div:
	for (int i = 0; i < length; ++i)
	{
		a[i] = a[i] / b[i];
	}
	while (l < loops)
	{
		++l;
		goto div;
	}
	auto div_time = clock() - start;

	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DynamicArrayLoop()
{
	const int length = 1024;
	const int loops = 5 * 10e5;
	std::cout << "Loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length), cv(length);
	double *a = &av.front(), *b = &bv.front(), *c = &cv.front();
	for (size_t i = 0; i < length; i++)
	{
		a[i] = (rand() % 256) / 16.0 + 1.3;
		b[i] = (rand() % 256) / 16.0 + 1.6;
		c[i] = (rand() % 256) / 16.0 + 1.1;
	}
	std::random_device rd; // obtain a random number from hardware
	std::mt19937 eng(rd()); // seed the generator
	std::uniform_int_distribution<> distr(0, length - 1); // define the range
	auto ignoreit = 0;
	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		// MSVC cannot vectorize this loop (message 1300).
		// However, if this loop will not be nested in, autovectorization will happen.
		// Same condition apply for mul/div/rcp loops

		// ICL does not have this issue, but to provide both vectorized and nonvectorized comparison 
		// i specifically disabled vectorization in this method
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = a[i] + b[i];
		}
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = a[i] * b[i];
		}
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = a[i] / b[i];
		}
	}
	auto div_time = clock() - start;

	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DynamicArrayLoopVectorized()
{
	const int length = 1024;
	const int loops = 5 * 10e5;
	std::cout << "Vectorized loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length), cv(length);
	double *a = &av.front(), *b = &bv.front(), *c = &cv.front();
	for (size_t i = 0; i < length; i++)
	{
		a[i] = (rand() % 256) / 16.0 + 1.3;
		b[i] = (rand() % 256) / 16.0 + 1.6;
		c[i] = (rand() % 256) / 16.0 + 1.1;
	}
	std::random_device rd; // obtain a random number from hardware
	std::mt19937 eng(rd()); // seed the generator
	std::uniform_int_distribution<> distr(0, length - 1); // define the range
	auto ignoreit = 0;
	int l = 0;
	auto start = clock();
add:
	for (int i = 0; i < length; i++)
	{
		a[i] = a[i] + b[i];
	}
	// MSVC doesn't vectorize nested loops (message 1300 - too little computation to vectorize) in function 'DynamicArrayLoop'.
	// However if nested loop is replaced with this nasty workaround SIMD vectorization will happen.
	// Same condition apply for mul/div/rcp loops
	while (l < loops)
	{
		++l;
		goto add;
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	l = 0;
	start = clock();
mul:
	for (int i = 0; i < length; ++i)
	{
		a[i] = a[i] * b[i];
	}
	while (l < loops)
	{
		++l;
		goto mul;
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;
	ignoreit += c[distr(eng)] + a[distr(eng)] + b[distr(eng)];
	l = 0;
	start = clock();
div:
	for (int i = 0; i < length; ++i)
	{
		a[i] = a[i] / b[i];
	}
	while (l < loops)
	{
		++l;
		goto div;
	}
	auto div_time = clock() - start;
	std::cout << "Division: " << div_time << std::endl;

	l = 0;
	start = clock();

	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}


void MulVsDiv::BenchAll()
{
	std::cout << "--- Static arrays ---" << std::endl;
	Loop();
	LoopVectorized();
	std::cout << "--- Dynamic arrays ---" << std::endl;
	DynamicArrayLoop();
	DynamicArrayLoopVectorized();
}

MulVsDiv::MulVsDiv()
{
}


MulVsDiv::~MulVsDiv()
{
}
