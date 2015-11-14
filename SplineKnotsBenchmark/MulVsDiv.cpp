#include "stdafx.h"
#include "MulVsDiv.h"
#include <ostream>
#include <iostream>
#include <locale>


void MulVsDiv::NoLoop()
{
	const int length = 16;
	const int loops = 10e7;
	std::cout << "No loop: " << std::endl;
	double a[length], b[length];

	for (int i = 0; i < length; i++)
	{
		a[i] = rand() % 2048;
		b[i] = rand() % 2048;
	}

	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		a[0] *= b[0];
		a[1] *= b[1];
		a[2] *= b[2];
		a[3] *= b[3];
		a[4] *= b[4];
		a[5] *= b[5];
		a[6] *= b[6];
		a[7] *= b[7];
		a[8] *= b[8];
		a[9] *= b[9];
		a[10] *= b[10];
		a[11] *= b[11];
		a[12] *= b[12];
		a[13] *= b[13];
		a[14] *= b[14];
		a[15] *= b[15];

	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		a[0] /= b[0];
		a[1] /= b[1];
		a[2] /= b[2];
		a[3] /= b[3];
		a[4] /= b[4];
		a[5] /= b[5];
		a[6] /= b[6];
		a[7] /= b[7];
		a[8] /= b[8];
		a[9] /= b[9];
		a[10] /= b[10];
		a[11] /= b[11];
		a[12] /= b[12];
		a[13] /= b[13];
		a[14] /= b[14];
		a[15] /= b[15];
	}
	auto div_time = clock() - start;
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl << std::endl;
}

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
	std::cout << "Division: " << div_time << std::endl;
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
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl << std::endl;
}

void MulVsDiv::BenchAll()
{	
	NoLoop();
	Loop();
	LoopInlined();

}

MulVsDiv::MulVsDiv()
{
}


MulVsDiv::~MulVsDiv()
{
	
}
