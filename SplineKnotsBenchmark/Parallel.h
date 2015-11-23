#pragma once
#include <ppl.h>

namespace utils
{
	
	class Loop
	{
	public:
		enum Parallelization
		{
			None,
			Automatic,
			Forced
		};
		template <typename Index_type, typename Function>
		void For(Index_type from, Index_type to, Function& function)
		{
			using namespace concurrency;
			switch(parallelization_)
			{
			case None: none:
				
				for (Index_type i = from; i < to; ++i)
				{
					function(i);
				}
				break;
			case Automatic: 
				if (from - to <= seq_treshold_)
					goto none;
				else
					goto forced;
				break;
			case Forced: forced:			
				parallel_for(from, to, function);
				break;
			}
		}
		Loop();
		
		size_t SequentialThreshold();
		void SetSequentialTreshold(size_t value);
		Parallelization ParallelizationType();
		void SetParallelizationType(Parallelization partype);
	private:
		Parallelization parallelization_ = Automatic;
		size_t seq_treshold_ = 10000;

	};
}
