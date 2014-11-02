#include <MySensor.h>
#include <SPI.h>
#include <DallasTemperature.h>
#include <OneWire.h>
//#include <DS3232RTC.h>  // DS3231/DS3232 library
//#include <Time.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
// Temperature section
#define ONE_WIRE_BUS			A5 // pin where dallase sensor is connected 
#define MAX_ATTACHED_DS18B20	1
OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);
float lastTemperature[MAX_ATTACHED_DS18B20];
int numSensors = 0;
MyMessage msgTemperature(0, V_TEMP);

// Relays section
#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
#define FIRST_RELAY_PIN			2  // Arduino Digital I/O pin number for first relay (second on pin+1, etc.)
#define NUMBER_OF_RELAYS		7  // Total number of attached relays
MyMessage msgRelay(0, V_LIGHT);


MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
unsigned long SLEEP_TIME = 3000;	// Sleep time between reads (in milliseconds)
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	sensors.begin();

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Aquarium Controller", "1.0");

	// Fetch relay status (sensorID = 0...6)
	for (int sensorID = 0, pin = FIRST_RELAY_PIN; sensorID < NUMBER_OF_RELAYS; sensorID++, pin++)
	{
		// Register all sensors to gw (they will be created as child devices)
		gw.present(sensorID, S_LIGHT);
		// Then set relay pins in output mode
		pinMode(pin, OUTPUT);

		uint8_t lastState = gw.loadState(sensorID);

		// Set relay to last known state (using eeprom storage) 
		digitalWrite(pin, lastState ? RELAY_ON : RELAY_OFF);

		gw.send(msgRelay.setSensor(sensorID).set(lastState ? 1 : 0));
	}




	// Fetch the number of attached temperature sensors  
	numSensors = sensors.getDeviceCount();
	// Present all sensors to controller
	for (int sensorID = 0; sensorID < numSensors && sensorID < MAX_ATTACHED_DS18B20; sensorID++)
		gw.present(NUMBER_OF_RELAYS + sensorID, S_TEMP);


	Serial.println("begin done");
}
void loop()
{
	gw.process();

	//// Fetch temperatures from Dallas sensors
	//sensors.requestTemperatures();

	//// Read temperatures and send them to controller 
	//for (int sensorID = 0; sensorID < numSensors && sensorID < MAX_ATTACHED_DS18B20; sensorID++)
	//{
	//	// Fetch and round temperature to one decimal
	//	float temperature = static_cast<float>(static_cast<int>((gw.getConfig().isMetric ? sensors.getTempCByIndex(sensorID) : sensors.getTempFByIndex(sensorID)) * 10.)) / 10.;

	//	// Only send data if temperature has changed and no error
	//	if (lastTemperature[sensorID] != temperature && temperature != -127.00)
	//	{
	//		// Send in the new temperature
	//		gw.send(msgTemperature.setSensor(NUMBER_OF_RELAYS + sensorID).set(temperature, 1));
	//		lastTemperature[sensorID] = temperature;
	//	}
	//}

	//gw.requestTime(onTimeReceived);

	//gw.sleep(SLEEP_TIME);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	Serial.println("onMessageReceived");

	if (message.type == V_LIGHT)
	{
		// Change relay state
		digitalWrite(message.sensor + FIRST_RELAY_PIN, message.getBool() ? RELAY_ON : RELAY_OFF);
		// Store state in eeprom
		gw.saveState(message.sensor, message.getBool());

		// Write some debug info
		//Serial.print("Incoming change for sensor:");
		//Serial.print(message.sensor);
		//Serial.print(", New status: ");
		//Serial.println(message.getBool());
	}
}
void onTimeReceived(unsigned long time) //Incoming argument is seconds since 1970.
{
	//setTime(time);
}
void printTime() {
	//sprintf(timeBuf, "%02d:%02d:%02d", hour(), minute(), second());
	//myGLCD.print(timeBuf, 60, 7);
}