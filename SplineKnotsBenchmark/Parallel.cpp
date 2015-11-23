#include "stdafx.h"
#include "Parallel.h"


utils::Loop::Loop()
{
}

size_t utils::Loop::SequentialThreshold()
{
	return seq_treshold_;
}

void utils::Loop::SetSequentialTreshold(size_t value)
{
	seq_treshold_ = value;
}

utils::Loop::Parallelization utils::Loop::ParallelizationType()
{
	return parallelization_;
}

void utils::Loop::SetParallelizationType(Parallelization partype)
{
	parallelization_ = partype;
}