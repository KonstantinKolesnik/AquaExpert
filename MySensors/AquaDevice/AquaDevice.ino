#include <MySensor.h>
#include <SPI.h>
#include <DallasTemperature.h>
#include <OneWire.h>
#include <NewPing.h>
//#include <DS3232RTC.h>  // DS3231/DS3232 library
//#include <Time.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
// Relays section
#define RELAY_ON				0  // GPIO value to write to turn on attached relay
#define RELAY_OFF				1  // GPIO value to write to turn off attached relay
#define NUMBER_OF_RELAYS		8  // Total number of attached relays
uint8_t relayPins[7] = { 2, 3, 4, 5, 6, 7, 8 };
MyMessage msgRelay(0, V_LIGHT);
//--------------------------------------------------------------------------------------------------------------------------------------------
// Sonar section
#define TRIGGER_PIN  A1  // Arduino pin tied to trigger pin on the ultrasonic sensor.
#define ECHO_PIN     A0  // Arduino pin tied to echo pin on the ultrasonic sensor.
#define MAX_DISTANCE 100 // Maximum distance we want to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.
NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);
MyMessage msgSonar(9, V_DISTANCE);
int lastDist;
unsigned long prevMsSonar = 0;
const long intervalSonar = 1000;
//--------------------------------------------------------------------------------------------------------------------------------------------
// Moisture section
#define MOISTURE_PIN  A2


//--------------------------------------------------------------------------------------------------------------------------------------------
// Ph section
#define PH_PIN  A3
#define PH_OFFSET 0.0//-0.12
MyMessage msgPh(10, V_VAR1);
float lastPh;
unsigned long prevMsPh = 0;
const long intervalPh = 1000;
//--------------------------------------------------------------------------------------------------------------------------------------------
// ORP section
#define ORP_PIN  A4

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

//--------------------------------------------------------------------------------------------------------------------------------------------
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


	//(sensorID = 9)
	gw.present(9, S_DISTANCE);

	//(sensorID = 10)
	gw.present(10, S_PH);


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

	// analogRead: moisture sensor:
	// transistor: 524 for water;  838 for short circuit; (100/100/KT3102)
	// Yusupov:    ~650 for water; ~1000 for short circuit; ~1 for air; (2k / 100k)


	// Ph:
	if (currentMs - prevMsPh >= intervalPh)
	{
		prevMsPh = currentMs;

		float ph = readPh();
		Serial.print("Ph: ");
		Serial.println(ph);

		if (ph != lastPh)
		{
			gw.send(msgPh.set(ph, 1));
			lastPh = ph;
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

		digitalWrite(relayPins[message.sensor], value ? RELAY_ON : RELAY_OFF);
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

float readPh()
{
	/*
	(1) Connect equipments according to the graphic, that is, the pH electrode is connected to the BNC connector on the pH meter board,
	and then use the connection lines, the pH meter board is connected to the ananlong port 0 of the Arduino controller.
	When the Arduino controller gets power, you will see the blue LED on board is on.
	(2) Upload the sample code to the Arduino controller.
	(3) Put the pH electrode into the standard solution whose pH value is 7.00, or directly shorten the input of the BNC connector.
	Open the serial monitor of the Arduino IDE, you can see the pH value printed on it, and the error does not exceed 0.3.
	Record the pH value printed, then compared with 7.00, and the difference should be assifned to the "Offset".
	For example, the pH value printed is 6.88, so the difference is 0.12. You should change the "# define Offset 0.00" into "# define Offset 0.12" in your program.
	(4) Put the pH electrode into the pH standard solution whose value is 4.00. Then wait about one minute, adjust the gain potential device,
	let the value stabilise at around 4.00. At this time the acidic calibration has been completed and you can measure the pH value of an acidic solution.
	Note: If you want to measure the pH value of other solution, you must wash the pH electrode first!
	(5) According to the linear characteristics of pH electrode itself, after the above calibration, you can directly measure the pH value of the alkaline solution,
	but if you want to get better accuracy, you can recalibrate it. Alkaline calibration use the standard solution whose pH value is 9.18.
	Also adjust the gain potential device, let the value stabilise at around 9.18. After this calibration, you can measure the pH value of the alkaline solution.
	*/

	// get 10 sample value from the sensor for smooth the value:
	int buf[10];
	for (int i = 0; i < 10; i++)
	{
		buf[i] = analogRead(PH_PIN);
		delay(10);
	}

	// sort the analog from small to large:
	for (int i = 0; i < 9; i++)
	{
		for (int j = i + 1; j < 10; j++)
		{
			if (buf[i] > buf[j])
			{
				int temp = buf[i];
				buf[i] = buf[j];
				buf[j] = temp;
			}
		}
	}

	// store the average value of the sensor feedback:
	unsigned long int avgValue = 0;
	for (int i = 2; i < 8; i++) // take the average value of 6 center sample
		avgValue += buf[i];
	float phValue = (float)(avgValue * 5.0 / 1024 / 6); // convert the analog into millivolt
	phValue = 3.5 * phValue + PH_OFFSET; // convert the millivolt into pH value

	//Serial.print("    pH:");
	//Serial.print(phValue,2);
	//Serial.println("");

	return phValue;
}
