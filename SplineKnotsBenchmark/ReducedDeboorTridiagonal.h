#pragma once
#include "Tridiagonal.h"

namespace splineknots
{
	class ReducedDeBoorTridiagonal : public Tridiagonal
	{
	public:
		ReducedDeBoorTridiagonal* Clone() const override;
		ReducedDeBoorTridiagonal();
		virtual ~ReducedDeBoorTridiagonal();
		void Solve(size_t num_unknowns) override;
	};
}
