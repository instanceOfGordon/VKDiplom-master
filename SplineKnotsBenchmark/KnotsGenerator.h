#pragma once
#include "KnotMatrix.h"
#include "MathFunction.h"
struct SurfaceDimension;

namespace splineknots {
	
	//double safeCall(MathFunction function, double x, double y);

	class KnotsGenerator
	{
		InterpolativeMathFunction function_;
	public:
	
		virtual ~KnotsGenerator();// = default;
		const InterpolativeMathFunction& Function() const;

		virtual std::unique_ptr<KnotMatrix> GenerateKnots(SurfaceDimension& udimension, SurfaceDimension& vdimension) = 0;
	protected:
		explicit KnotsGenerator(MathFunction function);
		//virtual ~KnotsGenerator();
		
	};

}

