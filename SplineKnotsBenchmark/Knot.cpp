#include "stdafx.h"
#include "Knot.h"

namespace splineknots {
	Knot::Knot(double x, double y, double z, double dx, double dy, double dxy)
		: x_(x),
		y_(y),
		z_(z),
		dx_(dx),
		dy_(dy),
		dxy_(dxy)
	{
	}

	Knot::Knot(double x, double y, double z)
		: x_(x),
		y_(y),
		z_(z),
		dx_(0),
		dy_(0),
		dxy_(0)
	{
	}

	Knot::Knot()
		: x_(0),
		y_(0),
		z_(0),
		dx_(0),
		dy_(0),
		dxy_(0)
	{
	}

	Knot::~Knot()
	{
	}

	double Knot::X() const
	{
		return x_;
	}

	void Knot::SetX(const double x)
	{
		x_ = x;
	}

	double Knot::Y() const
	{
		return y_;
	}

	void Knot::SetY(const double y)
	{
		y_ = y;
	}

	double Knot::Z() const
	{
		return z_;
	}

	void Knot::SetZ(const double z)
	{
		z_ = z;
	}

	double Knot::Dx() const
	{
		return dx_;
	}

	void Knot::SetDx(const double dx)
	{
		dx_ = dx;
	}

	double Knot::Dy() const
	{
		return dy_;
	}

	void Knot::SetDy(const double dy)
	{
		dy_ = dy;
	}

	double Knot::Dxy() const
	{
		return dxy_;
	}

	void Knot::SetDxy(const double dxy)
	{
		dxy_ = dxy;
	}

	/*std::string Knot::toString()
	{
		return
	}*/

	Knot Knot::operator+(const Knot& other)
	{
		Knot temp = *this;
		temp.SetZ(temp.Z() + other.Z());
		temp.SetDx(temp.Dx() + other.Dx());
		temp.SetDy(temp.Dy() + other.Dy());
		temp.SetDxy(temp.Dxy() + other.Dxy());
		return temp;
	}

	Knot Knot::operator-(const Knot& other)
	{
		Knot temp = *this;
		temp.SetZ(temp.Z() - other.Z());
		temp.SetDx(temp.Dx() - other.Dx());
		temp.SetDy(temp.Dy() - other.Dy());
		temp.SetDxy(temp.Dxy() - other.Dxy());
		return temp;
	}

	Knot Knot::operator*(const Knot & other)
	{
		Knot temp = *this;
		temp.SetZ(temp.Z() * other.Z());
		temp.SetDx(temp.Dx() * other.Dx());
		temp.SetDy(temp.Dy() * other.Dy());
		temp.SetDxy(temp.Dxy() * other.Dxy());
		return temp;
	}

	Knot Knot::operator/(const Knot& other)
	{
		Knot temp = *this;
		temp.SetZ(temp.Z() / other.Z());
		temp.SetDx(temp.Dx() / other.Dx());
		temp.SetDy(temp.Dy() / other.Dy());
		temp.SetDxy(temp.Dxy() / other.Dxy());
		return temp;
	}

	bool Knot::operator==(const Knot& other)
	{
		auto result = this->X() == other.X();
		result = result && this->Y() == other.Y();
		result = result && this->Dx() == other.Dx();
		result = result && this->Dy() == other.Dy();
		result = result && this->Dxy() == other.Dxy();
		return result;
	}
}
