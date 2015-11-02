#pragma once

namespace splineknots {
	class Knot 
	{
	private:
		double x_;
		double y_;
		double z_;
		double dx_;
		double dy_;
		double dxy_;
	public:
		Knot(double x, double y, double z, double dx, double dy, double dxy);
		Knot(double x, double y, double z);
		Knot();
		~Knot();


		double X() const;

		void SetX(const double x);

		double Y() const;

		void SetY(const double y);

		double Z() const;

		void SetZ(const double z);

		double Dx() const;

		void SetDx(const double dx);

		double Dy() const;

		void SetDy(const double dy);

		double Dxy() const;

		void SetDxy(const double dxy);

		//std::string toString();

		Knot operator+(const Knot& other);
		Knot operator-(const Knot& other);
		Knot operator*(const Knot& other);
		Knot operator/(const Knot& other);
		bool operator==(const Knot& other);

	};

}

