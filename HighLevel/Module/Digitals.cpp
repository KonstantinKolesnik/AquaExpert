#include "Digitals.h"

Digitals::Digitals(bool isActive)
{
	pinMode(DIGITAL_0, OUTPUT);
	pinMode(DIGITAL_1, OUTPUT);
	pinMode(DIGITAL_2, OUTPUT);
	pinMode(DIGITAL_3, OUTPUT);
	pinMode(DIGITAL_4, OUTPUT);
	pinMode(DIGITAL_5, OUTPUT);
	pinMode(DIGITAL_6, OUTPUT);
	pinMode(DIGITAL_7, OUTPUT);

	uint8_t state = EEPROM.read(0);

	SetActive(isActive);
}

void Digitals::SetActive(uint8_t address, bool isActive)
{
	digitalWrite(address, isActive ? DIGITAL_ACTIVE_LEVEL : !DIGITAL_ACTIVE_LEVEL);
}
void Digitals::SetActive(bool isActive)
{
	SetActive(DIGITAL_0, isActive);
	SetActive(DIGITAL_1, isActive);
	SetActive(DIGITAL_2, isActive);
	SetActive(DIGITAL_3, isActive);
	SetActive(DIGITAL_4, isActive);
	SetActive(DIGITAL_5, isActive);
	SetActive(DIGITAL_6, isActive);
	SetActive(DIGITAL_7, isActive);
}

bool Digitals::IsActive(uint8_t address)
{
	int level = digitalRead(address);
	return DIGITAL_ACTIVE_LEVEL ? level : !level;
}
