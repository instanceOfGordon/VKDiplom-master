#pragma once
class MulVsDiv
{
public:
	void ResetArrays(const int length, double* a, double* b, double& ignoreit);
	void Loop();
	void LoopVectorized();
	void DynamicArrayLoop();
	void DynamicArrayLoopVectorized();
	void BenchAll();
	MulVsDiv();
	~MulVsDiv();
};
