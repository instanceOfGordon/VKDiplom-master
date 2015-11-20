#pragma once
class Cloneable
{
public:
	
	virtual ~Cloneable();
	virtual Cloneable* Clone() const = 0;
};

