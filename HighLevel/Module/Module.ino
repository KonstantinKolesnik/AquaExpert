#include <OneWire.h>
#include <EEPROM.h>
#include "Hardware.h"
#include "TemperatureSensors.h"
#include "Digitals.h"


//int RXLED = 17;  // The RX LED has a defined Arduino pin
// The TX LED was not so lucky, we'll need to use pre-defined
// macros (TXLED1, TXLED0) to control that.
// (We could use the same macros for the RX LED too -- RXLED1,
//  and RXLED0.)



Digitals relays(false);
TemperatureSensors temperatureSensors;
uint8_t n = 0;

void setup()
{
	//pinMode(RXLED, OUTPUT);  // Set RX LED as an output
	// TX LED is set as an output behind the scenes

	Serial.begin(9600); //This pipes to the serial monitor
	//Serial1.begin(9600); //This is the UART, pipes to sensors attached to board

	relays.SetActive(false);

}

void loop()
{
	//Serial.println("Hello world");  // Print "Hello World" to the Serial Monitor
	//Serial1.println("Hello!");  // Print "Hello!" over hardware UART

	relays.SetActive(DIGITAL_0, true);
	temperatureSensors.GetTemperature();


	//digitalWrite(RXLED, LOW);   // set the LED on
	//TXLED0; //TX LED is not tied to a normally controlled pin

	delay(1000);



	relays.SetActive(DIGITAL_0, false);

	//Serial.println(EEPROM.read(0));
	//EEPROM.write(0, n);
	//n++;
	//if (n > 255)
	//	n = 0;

	//digitalWrite(RXLED, HIGH);    // set the LED off
	//TXLED1;

	delay(1000);
}

