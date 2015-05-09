#include <MySensor.h>
#include <SPI.h>
#include <DHT.h>
#include <eeprom.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_OUTER_SENSOR_ID	0
MyMessage msgTemperatureOuter(TEMPERATURE_OUTER_SENSOR_ID, V_TEMP);
float lastTemperatureOuter;
unsigned long prevMsTemperatureOuter = 0;
const long intervalTemperatureOuter = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_OUTER_SENSOR_ID	1
MyMessage msgHumidityOuter(HUMIDITY_OUTER_SENSOR_ID, V_HUM);
float lastHumidityOuter;
unsigned long prevMsHumidityOuter = 0;
const long intervalHumidityOuter = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define TEMPERATURE_INNER_SENSOR_ID	2
MyMessage msgTemperatureInner(TEMPERATURE_INNER_SENSOR_ID, V_TEMP);
float lastTemperatureInner;
unsigned long prevMsTemperatureInner = 0;
const long intervalTemperatureInner = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
#define HUMIDITY_INNER_SENSOR_ID	3
MyMessage msgHumidityInner(HUMIDITY_INNER_SENSOR_ID, V_HUM);
float lastHumidityInner;
unsigned long prevMsHumidityInner = 0;
const long intervalHumidityInner = 3000;

//--------------------------------------------------------------------------------------------------------------------------------------------
bool isMetric = true;
MySensor gw(DEFAULT_CE_PIN, DEFAULT_CS_PIN);
DHT dhtOuter, dhtInner;
#define DHT_OUTER_PIN	2
#define DHT_INNER_PIN	3
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	//for (int i = 0; i < 512; i++)
	//	EEPROM.write(i, 255);

	gw.begin(onMessageReceived);
	gw.sendSketchInfo("Meteo Station", "1.0");

	isMetric = gw.getConfig().isMetric;

	dhtOuter.setup(DHT_OUTER_PIN);
	dhtInner.setup(DHT_INNER_PIN);

	gw.present(TEMPERATURE_OUTER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_OUTER_SENSOR_ID, S_HUM);
	gw.present(TEMPERATURE_INNER_SENSOR_ID, S_TEMP);
	gw.present(HUMIDITY_INNER_SENSOR_ID, S_HUM);
}

void loop()
{
	gw.process();

	unsigned long ms = millis();

	processTemperatureOuter(ms);
	processHumidityOuter(ms);
	processTemperatureInner(ms);
	processHumidityInner(ms);

}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	if (message.type == V_LIGHT)
	{
		//uint8_t newState = message.getByte();
		//uint8_t lastState = gw.loadState(message.sensor);

		//if (newState != lastState)
		//{
		//	digitalWrite(pins[message.sensor], newState ? RELAY_ON : RELAY_OFF);
		//	gw.saveState(message.sensor, newState);

		//	gw.send(msgRelay.setSensor(message.sensor).set(newState));
		//}
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void processTemperatureOuter(unsigned long ms)
{
	if (ms - prevMsTemperatureOuter >= intervalTemperatureOuter)
	{
		prevMsTemperatureOuter = ms;

		delay(dhtOuter.getMinimumSamplingPeriod());

		float temperature = dhtOuter.getTemperature();
		if (!isnan(temperature))
		{
			//if (lastTemperatureOuter != temperature)
			{
				lastTemperatureOuter = temperature;

				if (!isMetric)
					temperature = dhtOuter.toFahrenheit(temperature);

				Serial.print("Temperature Outer: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");

				gw.send(msgTemperatureOuter.set(temperature, 1));
			}
		}
		else
			Serial.println("Failed reading Temperature Outer");
	}
}
void processHumidityOuter(unsigned long ms)
{
	if (ms - prevMsHumidityOuter >= intervalHumidityOuter)
	{
		prevMsHumidityOuter = ms;

		delay(dhtOuter.getMinimumSamplingPeriod());

		float humidity = dhtOuter.getHumidity();
		if (!isnan(humidity))
		{
			if (lastHumidityOuter != humidity)
			{
				lastHumidityOuter = humidity;

				Serial.print("Humidity Outer: ");
				Serial.print(humidity, 1);
				Serial.println("%");

				gw.send(msgHumidityOuter.set(humidity, 1));
			}
		}
		else
			Serial.println("Failed reading Humidity Outer");
	}
}
void processTemperatureInner(unsigned long ms)
{
	if (ms - prevMsTemperatureInner >= intervalTemperatureInner)
	{
		prevMsTemperatureInner = ms;

		delay(dhtInner.getMinimumSamplingPeriod());

		float temperature = dhtInner.getTemperature();
		if (!isnan(temperature))
		{
			if (lastTemperatureInner != temperature)
			{
				lastTemperatureInner = temperature;

				if (!isMetric)
					temperature = dhtInner.toFahrenheit(temperature);

				Serial.print("Temperature Inner: ");
				Serial.print(temperature, 1);
				Serial.println(isMetric ? " C" : " F");

				gw.send(msgTemperatureInner.set(temperature, 1));
			}
		}
		else
			Serial.println("Failed reading Temperature Inner");
	}
}
void processHumidityInner(unsigned long ms)
{
	if (ms - prevMsHumidityInner >= intervalHumidityInner)
	{
		prevMsHumidityInner = ms;

		delay(dhtInner.getMinimumSamplingPeriod());

		float humidity = dhtInner.getHumidity();
		if (!isnan(humidity))
		{
			if (lastHumidityInner != humidity)
			{
				lastHumidityInner = humidity;

				Serial.print("Humidity Inner: ");
				Serial.print(humidity, 1);
				Serial.println("%");

				gw.send(msgHumidityInner.set(humidity, 1));
			}
		}
		else
			Serial.println("Failed reading Humidity Inner");
	}
}
