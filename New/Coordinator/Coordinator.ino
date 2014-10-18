#include <SPI.h>
#include <EthernetUdp.h>
//#include <EthernetServer.h>
//#include <EthernetClient.h>
#include <Ethernet.h>
//#include <Dns.h>
//#include <Dhcp.h>
#include <nRF24L01.h>
#include <RF24.h>
//#include <RF24_config.h>
//****************************************************************************************
/*
Ethernet shield attached to pins:
------------------------------------------------
UNO:
04 - SS for SD card
10 - SS for W5100
11 - MOSI
12 - MISO
13 - SCK
------------------------------------------------
MEGA:
04 - SS for SD card
10 - SS for W5100
50 - MISO
51 - MOSI
52 - SCK
53 - SS (hardware) for W5100; не используется для выбора W5100, но должен быть сконфигурирован как выход, в противном случае SPI не будет работать.
------------------------------------------------
Если одно из устройств в проекте не используется - необходимо явно деактивировать его.
Для этого вывод, отвечающий за активизацию соответствующего устройства (4 - для SD-карты, 10 - для W5100),
необходимо сконфигурировать как выход и подать на него высокий уровень сигнала.
*/
//****************************************************************************************
/*
NRF24LP+:

UNO:
1 -> GND
2 -> VCC 3.3V !!! NOT 5V
3 -> CE to Arduino pin 9
4 -> CSN to Arduino pin 10(8)
5 -> SCK to Arduino pin 13
6 -> MOSI to Arduino pin 11
7 -> MISO to Arduino pin 12
8 -> UNUSED

RF24 radio(9,10(8));
*/
//****************************************************************************************
#define PRINT_DEBUG_INFO_UDP
//#define PRINT_DEBUG_INFO_HTTP
#define PRINT_DEBUG_INFO_RADIO
//****************************************************************************************
byte mac[] = { 0x00, 0xAA, 0xBB, 0xCC, 0xDE, 0x02 };
IPAddress ipAddress(192, 168, 1, 177);

uint16_t udpPort = 8888;
char udpRequestBuffer[UDP_TX_PACKET_MAX_SIZE]; //buffer to hold incoming packet
char udpResponseBuffer[] = "SNCOK"; // a string to send back
EthernetUDP udpServer;

const uint64_t radioAddress = 0xABCDEFABCDLL;
uint32_t rom; // 2^32 = 4294967296 different addresses
uint8_t radioCSPin = 8; //n != 04 (SD), 10 (ethernet);
uint8_t requestBuffer[40];
uint8_t responseBuffer[40];
uint8_t payloadSize = 8; // default for static payload
RF24 radio(9, radioCSPin);

EthernetServer ethernetServer(80);
//****************************************************************************************
void setup()
{
	Serial.begin(9600);
	while (!Serial) { }; // wait for serial port to connect. Needed for Leonardo only

	SetupEthernet(true);
	udpServer.begin(udpPort);
	ethernetServer.begin();
	SetupRadio();
}
void loop()
{
	PollUDP();
	PollEthernet();
	PollRadio2();
}
//****************************************************************************************
void SetupEthernet(bool useStaticIP)
{
	if (useStaticIP)
		Ethernet.begin(mac, ipAddress);
	else
	{
		Serial.println("Get IP address through the DHCP...");
		if (Ethernet.begin(mac) == 0)
		{
			Serial.println("Failed to configure Ethernet using DHCP.");
			// no point in carrying on, so do nothing forevermore:
			for (;;);
		}

		ipAddress = Ethernet.localIP();
	}

	Serial.print("Server started at ");

	//for (byte thisByte = 0; thisByte < 4; thisByte++)
	//{
	//	// print the value of each byte of the IP address:
	//	Serial.print(ipAddress[thisByte], DEC);
	//	Serial.print(".");
	//}
	//Serial.println();

	Serial.println(ipAddress);
}
void SetupRadio()
{
	radio.begin();
	radio.setRetries(15, 15);

	//radio.setPayloadSize(payloadSize); // for static payload size
	radio.enableDynamicPayloads(); // for dynamic payload size

	//radio.setDataRate(RF24_250KBPS);
	radio.setDataRate(RF24_2MBPS);

	radio.openWritingPipe(radioAddress);
	radio.openReadingPipe(1, radioAddress);

#ifdef PRINT_DEBUG_INFO_RADIO
	//radio.printDetails();
#endif
}

void PollEthernet()
{
	// listen for incoming clients
	EthernetClient client = ethernetServer.available();

	if (client)
	{
#ifdef PRINT_DEBUG_INFO_HTTP
		Serial.println("Http client connected.");
#endif

		bool currentLineIsBlank = true; // an http request ends with a blank line

		while (client.connected())
		{
			if (client.available())
			{
				char c = client.read();
				
#ifdef PRINT_DEBUG_INFO_HTTP
				Serial.write(c);
#endif

				// if you've gotten to the end of the line (received a newline
				// character) and the line is blank, the http request has ended,
				// so you can send a reply
				if (c == '\n' && currentLineIsBlank)
				{
					// send a standard http response header
					client.println("HTTP/1.1 200 OK");
					client.println("Content-Type: text/html");
					client.println("Connection: close");  // the connection will be closed after completion of the response
					client.println("Refresh: 2");  // refresh the page automatically every 5 sec
					client.println();
					client.println("<!DOCTYPE HTML>");
					client.println("<html>");
					
					// output the value of each analog input pin
					for (int analogChannel = 0; analogChannel < 6; analogChannel++)
					{
						int sensorReading = analogRead(analogChannel);
						client.print("analog input ");
						client.print(analogChannel);
						client.print(" is ");
						client.print(sensorReading);
						client.println("<br />");
					}
					client.println("</html>");
					break;
				}

				if (c == '\n')
				{
					// you're starting a new line
					currentLineIsBlank = true;
				}
				else if (c != '\r')
				{
					// you've gotten a character on the current line
					currentLineIsBlank = false;
				}
			}
		}
		
		// give the web browser time to receive the data
		delay(1);
		
		// close the connection:
		client.stop();

#ifdef PRINT_DEBUG_INFO_HTTP
		Serial.println("Http client disconnected");
		Serial.println();
#endif
	}
}
void PollUDP()
{
	// if there's data available, read a packet
	int packetSize = udpServer.parsePacket();

	if (packetSize)
	{
#ifdef PRINT_DEBUG_INFO_UDP
		Serial.print("Received UDP packet of size ");
		Serial.print(packetSize);
		Serial.print(" from ");
		Serial.print(udpServer.remoteIP());
		Serial.print(":");
		Serial.println(udpServer.remotePort());
#endif

		// read the packet into packetBuffer
		udpServer.read(udpRequestBuffer, packetSize);// UDP_TX_PACKET_MAX_SIZE);
		udpRequestBuffer[packetSize] = '\0';

#ifdef PRINT_DEBUG_INFO_UDP
		Serial.println("Content:");
		Serial.println(udpRequestBuffer);
		Serial.println();
#endif
		String s(udpRequestBuffer);
		if (s == "SNC")
		{
#ifdef PRINT_DEBUG_INFO_UDP
			Serial.println("Send response:");
			Serial.println(udpResponseBuffer);
			Serial.println();
#endif

			// send a reply, to the IP address and port that sent us the packet we received
			udpServer.beginPacket(udpServer.remoteIP(), udpServer.remotePort());
			udpServer.write(udpResponseBuffer);
			int res = udpServer.endPacket();

#ifdef PRINT_DEBUG_INFO_UDP
			if (res == 0)
				Serial.println("Send fails.");
			else
				Serial.println("Send succeed.");
#endif
		}
	}

	delay(1);
}
void PollRadio()
{
	// Take the time, and send it. This will block until complete.
	unsigned long time = millis();

	Serial.print("Sending ");
	Serial.print(time);
	Serial.print(" ... ");

	bool ok = radio.write(&time, sizeof(time));
	Serial.println(ok ? "OK" : "Failed.");

	radio.startListening();

	// Wait here until we get a response, or timeout (250ms)
	unsigned long started_waiting_at = millis();
	bool timeout = false;
	while (!radio.available() && !timeout)
		if (millis() - started_waiting_at > 1000)
			timeout = true;

	// Describe the results
	if (timeout)
		Serial.println("Failed, response timed out.");
	else
	{
		// Grab the response, compare, and send to debugging spew
		unsigned long got_time;
		radio.read(&got_time, sizeof(got_time));

		Serial.print("Got response ");
		Serial.print(got_time);
		Serial.print("; round-trip delay: ");
		Serial.println(millis() - got_time);
	}

	radio.stopListening();

	delay(500);
}
void PollRadio2()
{
	payloadSize = 3; // comment out for static payload
	requestBuffer[0] = 5;
	requestBuffer[1] = 0;
	requestBuffer[2] = 0;

	bool ok = radio.write(requestBuffer, payloadSize);
#ifdef PRINT_DEBUG_INFO_RADIO
	Serial.print("Sending command... ");
	Serial.println(ok ? "OK" : "Failed.");
#endif

	radio.startListening();

	// Wait here until we get a response, or timeout (250ms)
	unsigned long started_waiting_at = millis();
	bool timeout = false;
	while (!radio.available() && !timeout)
		if (millis() - started_waiting_at > 1000)
			timeout = true;

	if (timeout)
	{
#ifdef PRINT_DEBUG_INFO_RADIO
		Serial.println("Response timed out.");
#endif
	}
	else
	{
		payloadSize = radio.getDynamicPayloadSize(); // comment out for static payload
		radio.read(responseBuffer, payloadSize);

#ifdef PRINT_DEBUG_INFO_RADIO
		Serial.print("Got response ");
		for (int i = 0; i < payloadSize; i++)
		{
			Serial.print("[");
			Serial.print(responseBuffer[i]);
			Serial.print("]");
		}
		Serial.println();
		double t = *((double*)responseBuffer);
		Serial.println(t);
#endif
	}

	radio.stopListening();

	delay(10);
}