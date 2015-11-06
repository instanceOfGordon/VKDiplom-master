#pragma once
#include <chrono>
template<typename C>
class StopWatch
{
	std::chrono::time_point<C> start_;
public:

	StopWatch();

	template<typename U>
	typename U::rep Elapsed() const;

	void Reset();

	~StopWatch() = default;
};

template <typename C>
StopWatch<C>::StopWatch(): start_(C::now())
{
}

template <typename C>
template <typename U>
typename U::rep StopWatch<C>::Elapsed() const
{
	return std::chrono::duration_cast<U>(C::now() - start_).count();
}

template <typename C>
void StopWatch<C>::Reset()
{
	start_ = C::now();
}