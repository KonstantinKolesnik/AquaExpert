#ifndef DS1307_H
#define DS1307_H

#include <avr/io.h>

#define TRUE 1
#define FALSE 0

#define DS1307_ADDR 0xD0

void DS1307Read(uint8_t address,uint8_t *data);
void DS1307Write(uint8_t address,uint8_t data);
void DS1307ReadASCII(char dateTime[21]);

#endif
