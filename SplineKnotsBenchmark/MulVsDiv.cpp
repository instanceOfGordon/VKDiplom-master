#include "stdafx.h"
#include "MulVsDiv.h"
#include <ostream>
#include <iostream>
#include <locale>

void MulVsDiv::Loop()
{
	const int length = 16;
	const int loops = 10e7;
	std::cout << "Loop: " << std::endl;
	double a[length], b[length];

	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		for (int i = 0; i < length; i++)
		{
			a[i] *= b[i];

		}
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		for (int i = 0; i < length; i++)
		{
			a[i] *= b[i];

		}
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		for (int i = 0; i < length; i++)
		{
			a[i] /= b[i];

		}
	}
	auto div_time = clock() - start;
	std::cout << "Division: " << div_time << std::endl << std::endl;
	std::cout << "Addition faster than division: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl << std::endl;
}

void MulVsDiv::LoopInlined()
{
	const int length = 16;
	const int loops = 10e7;
	std::cout << "Inlined loop: " << std::endl;
	double a[length], b[length];
	int lDiv4 = length / 4;

	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		for (int i = 0; i < lDiv4; i = +4)
		{
			a[i] += b[i];
			a[i + 1] += b[i + 1];
			a[i + 2] += b[i + 2];
			a[i + 3] += b[i + 3];

		}
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		for (int i = 0; i < lDiv4; i = +4)
		{
			a[i] *= b[i];
			a[i + 1] *= b[i + 1];
			a[i + 2] *= b[i + 2];
			a[i + 3] *= b[i + 3];

		}
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		for (int i = 0; i < lDiv4; i = +4)
		{
			a[i] /= b[i];
			a[i + 1] /= b[i + 1];
			a[i + 2] /= b[i + 2];
			a[i + 3] /= b[i + 3];

		}
	}
	auto div_time = clock() - start;
	std::cout << "Division: " << div_time << std::endl << std::endl;
	std::cout << "Addition faster than division: " << static_cast<double>(mul_time) / static_cast<double>(add_time) <<  std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl << std::endl;
}

void MulVsDiv::BenchAll()
{	
	Loop();
	LoopInlined();

}

MulVsDiv::MulVsDiv()
{
}


MulVsDiv::~MulVsDiv()
{
	
}
