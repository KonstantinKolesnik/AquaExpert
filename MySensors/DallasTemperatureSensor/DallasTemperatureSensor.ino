#include <MySensor.h>  
#include <SPI.h>
#include <DallasTemperature.h>
#include <OneWire.h>

#define ONE_WIRE_BUS			A5 // pin where dallase sensor is connected 
#define MAX_ATTACHED_DS18B20	16

unsigned long SLEEP_TIME = 1000; // Sleep time between reads (in milliseconds)

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);
float lastTemperature[MAX_ATTACHED_DS18B20];
int numSensors = 0;
boolean receivedConfig = false;
//boolean metric = true;

MyMessage msg(0, V_TEMP); // Initialize temperature message (sensorID is temporary 0)
MySensor gw(9, 10);

void setup()
{
	// Startup OneWire 
	sensors.begin();

	// Startup and initialize MySensors library. Set callback for incoming messages. 
	gw.begin();

	// Send the sketch version information to the gateway and Controller
	gw.sendSketchInfo("Temperature Sensor", "1.0");

	// Fetch the number of attached temperature sensors  
	numSensors = sensors.getDeviceCount();

	// Present all sensors to controller
	for (int sensorID = 0; sensorID < numSensors && sensorID < MAX_ATTACHED_DS18B20; sensorID++)
		gw.present(sensorID, S_TEMP);
}

void loop()
{
	// Process incoming messages (like config from server)
	gw.process();

	// Fetch temperatures from Dallas sensors
	sensors.requestTemperatures();

	// Read temperatures and send them to controller 
	for (int sensorID = 0; sensorID < numSensors && sensorID < MAX_ATTACHED_DS18B20; sensorID++)
	{
		// Fetch and round temperature to one decimal
		float temperature = static_cast<float>(static_cast<int>((gw.getConfig().isMetric ? sensors.getTempCByIndex(sensorID) : sensors.getTempFByIndex(sensorID)) * 10.)) / 10.;

		// Only send data if temperature has changed and no error
		if (lastTemperature[sensorID] != temperature && temperature != -127.00)
		{
			// Send in the new temperature
			gw.send(msg.setSensor(sensorID).set(temperature, 1));
			lastTemperature[sensorID] = temperature;
		}
	}

	gw.sleep(SLEEP_TIME);
}
