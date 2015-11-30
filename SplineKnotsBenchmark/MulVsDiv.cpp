#include "stdafx.h"
#include "MulVsDiv.h"
#include <ostream>
#include <iostream>
#include <locale>
#include <vector>

void MulVsDiv::ResetArrays(const int length, double* a, double* b, double& ignoreit)
{
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	for (size_t i = 0; i < length; i++)
	{
		a[i] = 6 * ((double)rand() / (RAND_MAX)) + 2;
		b[i] = 2 * ((double)rand() / (RAND_MAX)) + 1;
	}
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	ignoreit = 1/ignoreit;
}

void MulVsDiv::Loop()
{
	const int length = 256;
	const int loops = 10e7/4;
	std::cout << "Loop:\n---" << std::endl;
	double a[length], b[length];
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);


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

	ResetArrays(length, a, b, ignoreit);

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

	ResetArrays(length, a, b, ignoreit);

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
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::LoopVectorized()
{
	const int length = 256;
	const int loops = 10e7/4;
	std::cout << "Vectorized loop:\n---" << std::endl;
	double a[length], b[length];
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);

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

	ResetArrays(length, a, b, ignoreit);

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

	ResetArrays(length, a, b, ignoreit);

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
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DynamicArrayLoop()
{
	const int length = 1024*1024*8;
	const int loops = 1e3/2;
	std::cout << "Loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length);
	double *a = &av.front(), *b = &bv.front();
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);

	
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

	ResetArrays(length, a, b, ignoreit);

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

	ResetArrays(length, a, b, ignoreit);

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
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DynamicArrayLoopVectorized()
{
	const int length = 1024 *1024* 8;
	const int loops = 1e3/2;
	std::cout << "Vectorized loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length);
	double *a = &av.front(), *b = &bv.front();
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);

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

	ResetArrays(length, a, b, ignoreit);

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

	ResetArrays(length, a, b, ignoreit);

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
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;

	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}


void MulVsDiv::BenchAll()
{
	/*std::cout << "--- Static arrays ---" << std::endl;
	Loop();
	LoopVectorized();
	std::cout << "--- Dynamic arrays ---" << std::endl;*/
	DynamicArrayLoop();
	//DynamicArrayLoopVectorized();
}

MulVsDiv::MulVsDiv()
{
}


MulVsDiv::~MulVsDiv()
{
}
