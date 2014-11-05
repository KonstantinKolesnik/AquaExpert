#include <MySensor.h>
#include <SPI.h>
#include <DallasTemperature.h>
#include <OneWire.h>
#include <NewPing.h>
//#include <DS3232RTC.h>  // DS3231/DS3232 library
//#include <Time.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
// Temperature section
#define ONE_WIRE_BUS			A5 // pin where dallase sensor is connected 
#define MAX_ATTACHED_DS18B20	1
OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);
float lastTemperature[MAX_ATTACHED_DS18B20];
int numTemperatureSensors = 0;
MyMessage msgTemperature(0, V_TEMP);
unsigned long prevMsTemperature = 0;  // stores last time temperature was sent
const long intervalTemperature = 3000;		   // interval at which to send measurements (milliseconds)

// Relays section
#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
#define NUMBER_OF_RELAYS		8  // Total number of attached relays
uint8_t relayPins[7] = { 2, 3, 4, 5, 6, 7, 8 };
MyMessage msgRelay(0, V_LIGHT);

// Sonar section
#define TRIGGER_PIN  A1  // Arduino pin tied to trigger pin on the ultrasonic sensor.
#define ECHO_PIN     A0  // Arduino pin tied to echo pin on the ultrasonic sensor.
#define MAX_DISTANCE 100 // Maximum distance we want to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.
NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
MyMessage msgSonar(9, V_DISTANCE);
int lastDist;
unsigned long prevMsSonar = 0;
const long intervalSonar = 1000;


// common section
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
bool isMetric = true;
//unsigned long SLEEP_TIME = 3000;	// Sleep time between reads (in milliseconds)
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	sensors.begin();

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Aquarium Controller", "1.0");

	// Fetch relay status (sensorID = 0...7)
	for (int sensorID = 0; sensorID < NUMBER_OF_RELAYS; sensorID++)
	{
		// Register all sensors to gw (they will be created as child devices)
		gw.present(sensorID, S_LIGHT);
		// Then set relay pins in output mode
		pinMode(relayPins[sensorID], OUTPUT);

		uint8_t lastState = gw.loadState(sensorID);

		// Set relay to last known state (using eeprom storage) 
		digitalWrite(relayPins[sensorID], lastState ? RELAY_ON : RELAY_OFF);

		gw.send(msgRelay.setSensor(sensorID).set(lastState ? 1 : 0));
	}

	// Fetch the number of attached temperature sensors  
	numTemperatureSensors = sensors.getDeviceCount();
	// Present all sensors to controller (sensorID = 8)
	for (int sensorID = 0; sensorID < numTemperatureSensors && sensorID < MAX_ATTACHED_DS18B20; sensorID++)
		gw.present(NUMBER_OF_RELAYS + sensorID, S_TEMP);


	//(sensorID = 8)
	gw.present(9, S_DISTANCE);




	isMetric = gw.getConfig().isMetric;
}
void loop()
{
	gw.process();

	unsigned long currentMs = millis();

	if (currentMs - prevMsTemperature >= intervalTemperature)
	{
		prevMsTemperature = currentMs;

		// Fetch temperatures from Dallas sensors
		sensors.requestTemperatures();

		// Read temperatures and send them to controller 
		for (int sensorID = 0; sensorID < numTemperatureSensors && sensorID < MAX_ATTACHED_DS18B20; sensorID++)
		{
			// Fetch and round temperature to one decimal
			float temperature = static_cast<float>(static_cast<int>((isMetric ? sensors.getTempCByIndex(sensorID) : sensors.getTempFByIndex(sensorID)) * 10.)) / 10.;

			// Only send data if temperature has changed and no error
			if (lastTemperature[sensorID] != temperature && temperature != -127.00)
			{
				// Send in the new temperature
				gw.send(msgTemperature.setSensor(NUMBER_OF_RELAYS + sensorID).set(temperature, 1));
				lastTemperature[sensorID] = temperature;
			}
		}
	}

	if (currentMs - prevMsSonar >= intervalSonar)
	{
		prevMsSonar = currentMs;

		//delay(50);                      // Wait 50ms between pings (about 20 pings/sec). 29ms should be the shortest delay between pings.

		//unsigned int uS = sonar.ping(); // Send ping, get ping time in microseconds (uS).
		//Serial.print("Distance: ");
		//Serial.print(uS / US_ROUNDTRIP_CM); // Convert ping time to distance in cm and print result (0 = outside set distance range)
		//Serial.println("cm");

		int dist = isMetric ? sonar.ping_cm() : sonar.ping_in();
		Serial.print("Distance: ");
		Serial.print(dist); // Convert ping time to distance in cm and print result (0 = outside set distance range)
		Serial.println(isMetric ? " cm" : " in");

		if (dist != lastDist) {
			gw.send(msgSonar.set(dist));
			lastDist = dist;
		}
	}




	//gw.requestTime(onTimeReceived);

	//gw.sleep(SLEEP_TIME);
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	//Serial.println("onMessageReceived");

	if (message.type == V_LIGHT)
	{
		bool value = message.getBool();

		// Change relay state
		digitalWrite(relayPins[message.sensor], value ? RELAY_ON : RELAY_OFF);
		// Store state in eeprom
		gw.saveState(message.sensor, value);

		gw.send(msgRelay.setSensor(message.sensor).set(value));

		// Write some debug info
		Serial.print("Incoming change for sensor:");
		Serial.print(message.sensor);
		Serial.print(", New status: ");
		Serial.println(value);
	}
}
void onTimeReceived(unsigned long time) //Incoming argument is seconds since 1970.
{
	//setTime(time);
	//Serial.print("Time: ");
	//Serial.println(time);
}
void printTime() {
	//sprintf(timeBuf, "%02d:%02d:%02d", hour(), minute(), second());
	//myGLCD.print(timeBuf, 60, 7);
}
