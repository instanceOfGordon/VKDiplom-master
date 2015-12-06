#pragma once
#include "KnotMatrix.h"
#include "MathFunction.h"
#include "SurfaceDimension.h"

namespace splineknots
{
	class KnotsGenerator
	{
		InterpolativeMathFunction function_;
	public:
		virtual ~KnotsGenerator() = default;
		KnotsGenerator(const KnotsGenerator& other) = default;
		KnotsGenerator(KnotsGenerator&& other) = default;
		KnotsGenerator& operator=(const KnotsGenerator& other) = default;
		KnotsGenerator& operator=(KnotsGenerator&& other) = default;
		const InterpolativeMathFunction& Function() const;
		virtual KnotMatrix GenerateKnots(const SurfaceDimension& udimension, const SurfaceDimension& vdimension) = 0;
	protected:
		KnotsGenerator(MathFunction function);
		KnotsGenerator(InterpolativeMathFunction function);
	};
}
