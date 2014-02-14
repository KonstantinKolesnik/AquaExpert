#ifndef I2C_H_
#define I2C_H_
//****************************************************************************************
#include "..\Hardware.h"
//****************************************************************************************
#define TWI_BUFFER_LENGTH 10
//****************************************************************************************
void I2C_begin();
void I2C_beginWithAddress(uint8_t address);

void I2C_beginTransmission(uint8_t address);
//uint8_t I2C_endTransmission(void);
uint8_t I2C_endTransmission(uint8_t sendStop);

//uint8_t I2C_requestFrom(uint8_t, uint8_t);
uint8_t I2C_requestFrom(uint8_t address, uint8_t quantity, uint8_t sendStop);

size_t I2C_writeByte(uint8_t data);
size_t I2C_write(const uint8_t *data, size_t quantity);

int I2C_available(void);

int I2C_read(void);

int I2C_peek(void);

void I2C_onReceive( void (*)(int) );
void I2C_onRequest( void (*)(void) );

//inline size_t write(unsigned long n) { return write((uint8_t)n); }
//inline size_t write(long n) { return write((uint8_t)n); }
//inline size_t write(unsigned int n) { return write((uint8_t)n); }
//inline size_t write(int n) { return write((uint8_t)n); }
//using Print::write;
//****************************************************************************************
#endif


/*
// Demonstrates use of the Wire library
// Receives data as an I2C/TWI slave device
// Refer to the "Wire Master Writer" example for use with this

// Created 29 March 2006

// This example code is in the public domain.


#include <Wire.h>

void setup()
{
	Wire.begin(4);                // join i2c bus with address #4
	Wire.onReceive(receiveEvent); // register event
	Serial.begin(9600);           // start serial for output
}

void loop()
{
	delay(100);
}

// function that executes whenever data is received from master
// this function is registered as an event, see setup()
void receiveEvent(int howMany)
{
	while(1 < Wire.available()) // loop through all but the last
	{
		char c = Wire.read(); // receive byte as a character
		Serial.print(c);         // print the character
	}
	int x = Wire.read();    // receive byte as an integer
	Serial.println(x);         // print the integer
}
*/

/*
// Demonstrates use of the Wire library
// Sends data as an I2C/TWI slave device
// Refer to the "Wire Master Reader" example for use with this

// Created 29 March 2006

// This example code is in the public domain.


#include <Wire.h>

void setup()
{
	Wire.begin(2);                // join i2c bus with address #2
	Wire.onRequest(requestEvent); // register event
}

void loop()
{
	delay(100);
}

// function that executes whenever data is requested by master
// this function is registered as an event, see setup()
void requestEvent()
{
	Wire.write("hello "); // respond with message of 6 bytes
	// as expected by master
}
*/

/*
// Demonstrates use of the Wire library
// Writes data to an I2C/TWI slave device
// Refer to the "Wire Slave Receiver" example for use with this

// Created 29 March 2006

// This example code is in the public domain.


#include <Wire.h>

void setup()
{
	Wire.begin(); // join i2c bus (address optional for master)
}

byte x = 0;

void loop()
{
	Wire.beginTransmission(4); // transmit to device #4
	Wire.write("x is ");        // sends five bytes
	Wire.write(x);              // sends one byte
	Wire.endTransmission();    // stop transmitting

	x++;
	delay(500);
}
*/

/*
// Demonstrates use of the Wire library
// Reads data from an I2C/TWI slave device
// Refer to the "Wire Slave Sender" example for use with this

// Created 29 March 2006

// This example code is in the public domain.


#include <Wire.h>

void setup()
{
	Wire.begin();        // join i2c bus (address optional for master)
	Serial.begin(9600);  // start serial for output
}

void loop()
{
	Wire.requestFrom(2, 6);    // request 6 bytes from slave device #2

	while(Wire.available())    // slave may send less than requested
	{
		char c = Wire.read(); // receive a byte as character
		Serial.print(c);         // print the character
	}

	delay(500);
}
*/