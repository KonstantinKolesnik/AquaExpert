// qqq.h

#ifndef _QQQ_h
#define _QQQ_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

class QqqClass
{
 protected:


 public:
	void init();
};

extern QqqClass Qqq;

#endif

