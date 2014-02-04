#ifndef OWI_H_
#define OWI_H_
//****************************************************************************************
#include "..\Hardware.h"
#include "common_files\OWIdefs.h"
//#include "polled\OWIPolled.h"
//#include "polled\OWIHighLevelFunctions.h"
#include "polled\OWIBitFunctions.h"
#include "common_files\OWIcrc.h"
//****************************************************************************************
#define MAX_DEVICES		8						// Max number of devices to search for.
#define BUSES			(OWI_PIN_0 | OWI_PIN_1) // Buses to search.
















//****************************************************************************************
typedef struct
{
	unsigned char bus;      // A bit mask of the bus the device is connected to.
	unsigned char id[8];    // The 64 bit identifier.
} OWI_device;
//****************************************************************************************
void InitOWI();
//bool SearchBuses(OWI_device* devices, unsigned char len, unsigned char buses);
OWI_device* FindFamily(unsigned char familyID, OWI_device* devices, unsigned char size);


void OWI_SendByte(unsigned char data, unsigned char pins);
unsigned char OWI_ReceiveByte(unsigned char pin);
void OWI_SkipRom(unsigned char pins);
void OWI_ReadRom(unsigned char * romValue, unsigned char pins);
void OWI_MatchRom(unsigned char * romValue, unsigned char pins);
unsigned char OWI_SearchRom(unsigned char * bitPattern, unsigned char lastDeviation, unsigned char pins);







//****************************************************************************************
#endif /* OWI_H_ */