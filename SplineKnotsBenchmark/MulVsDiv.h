#pragma once
class MulVsDiv
{
	
public:
	void Loop();
	void LoopVectorized();
	void DynamicArrayLoop();
	void DynamicArrayLoopVectorized();
	void BenchAll();
	MulVsDiv();
	~MulVsDiv();
};

