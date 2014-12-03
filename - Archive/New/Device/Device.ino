//#include <StdioStream.h>
//#include <SdVolume.h>
//#include <SdStream.h>
//#include <SdSpi.h>
//#include <SdInfo.h>
//#include <SdFile.h>
//#include <SdFatUtil.h>
//#include <SdFatmainpage.h>
//#include <SdFatConfig.h>
//#include <SdFat.h>
//#include <SdBaseFile.h>
//#include <Sd2Card.h>
//#include <ostream.h>
//#include <MinimumSerial.h>
//#include <istream.h>
//#include <iostream.h>
//#include <ios.h>
//#include <bufstream.h>
//#include <ArduinoStream.h>

#include <SPI.h>
#include <EthernetUdp.h>
//#include <EthernetServer.h>
//#include <EthernetClient.h>
#include <Ethernet.h>
//#include <Dns.h>
//#include <Dhcp.h>
//#include <nRF24L01.h>
//#include <RF24.h>
#include <SD.h>

#include <WebServer.h>
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
NRF24LP+	UNO					MEGA

1 -> GND	GND					GND
2 -> VCC	3.3V, NOT 5V!!!		3.3V, NOT 5V!!!
3 -> CE		pin 9
4 -> CSN	pin 10(8)
5 -> SCK	pin 13
6 -> MOSI	pin 11
7 -> MISO	pin 12
8 -> IRQ	-					-

RF24 radio(CE pin, CSN pin);
*/
//****************************************************************************************
byte mac[] = { 0x00, 0xAA, 0xBB, 0xCC, 0xDE, 0x02 };
IPAddress ipAddress(192, 168, 1, 177);

//uint16_t udpPort = 8888;
//char udpRequestBuffer[UDP_TX_PACKET_MAX_SIZE]; //buffer to hold incoming packet
//char udpResponseBuffer[] = "SNCOK"; // a string to send back
//EthernetUDP udpServer;

//const uint64_t radioAddress = 0xABCDEFABCDLL;
//uint32_t rom; // 2^32 = 4294967296 different addresses
//uint8_t radioCSPin = 8; //n != 04 (SD), 10 (ethernet);
//uint8_t requestBuffer[40];
//uint8_t responseBuffer[40];
//uint8_t payloadSize = 8; // default for static payload
//RF24 radio(9, radioCSPin);

//File webFile;
//EthernetServer ethernetServer(80);
const char* root;

#define PREFIX "/rgb"
WebServer webserver(PREFIX, 80);
void rgbCmd(WebServer &server, WebServer::ConnectionType type, char *, bool);
void urlCmd(WebServer &server, WebServer::ConnectionType type, char **url_path, char *url_tail, bool tail_complete);

#define RED_PIN 5
#define GREEN_PIN 3
#define BLUE_PIN 6

int red = 0;            //integer for red darkness
int blue = 0;           //integer for blue darkness
int green = 0;          //integer for green darkness
//****************************************************************************************
void setup()
{
	Serial.begin(9600);
	while (!Serial) {}; // wait for serial port to connect. Needed for Leonardo only

	SetupEthernet(true);
	//SetupSDCard();
	//SetupUDPServer();
	//ethernetServer.begin();
	//SetupRadio();


	//webserver.setDefaultCommand(&rgbCmd);
	webserver.setUrlPathCommand(&urlCmd);
	//webserver.addCommand("index.html", &rgbCmd);
	webserver.begin();
}
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

	Serial.println(ipAddress);
}
void SetupSDCard()
{
	//Serial.print("Initializing SD card... ");

	//if (!SD.begin(4)) {
	//	Serial.println("Failed!");
	//	for (;;);
	//}
	//Serial.println("Success!");

	//// check for index.htm file
	//if (!SD.exists("AquaExpert\desktop.html")) {
	//	Serial.println("ERROR - Can't find desktop.html file!");
	//	for (;;);
	//}
	//Serial.println("SUCCESS - Found desktop.html file.");
}
void SetupUDPServer()
{
	//udpServer.begin(udpPort);
}
void SetupRadio()
{
	//radio.begin();
	//radio.setRetries(15, 15);

	////radio.setPayloadSize(payloadSize); // for static payload size
	//radio.enableDynamicPayloads(); // for dynamic payload size

	////radio.setDataRate(RF24_250KBPS);
	//radio.setDataRate(RF24_2MBPS);

	//radio.openWritingPipe(radioAddress);
	//radio.openReadingPipe(1, radioAddress);

#ifdef PRINT_DEBUG_INFO_RADIO
	//radio.printDetails();
#endif
}
//****************************************************************************************
void loop()
{
	webserver.processConnection();
}
void PollEthernet()
{
	//EthernetClient client = ethernetServer.available();

	//if (client)
	//{
	//	boolean currentLineIsBlank = true;
	//	while (client.connected())
	//	{
	//		if (client.available()) // client data available to read
	//		{
	//			char c = client.read(); // read 1 byte (character) from client
	//			// last line of client request is blank and ends with \n
	//			// respond to client only after last line received
	//			if (c == '\n' && currentLineIsBlank)
	//			{
	//				// send a standard http response header
	//				client.println("HTTP/1.1 200 OK");
	//				client.println("Content-Type: text/html");
	//				client.println("Connection: close");
	//				client.println();

	//				// send web page
	//				//webFile = SD.open("AquaExpert/desktop.html");
	//				//if (webFile)
	//				//{
	//				//	while (webFile.available())
	//				//		client.write(webFile.read()); // send web page to client

	//				//	webFile.close();
	//				//}
	//				break;
	//			}
	//			// every line of text received from the client ends with \r\n
	//			if (c == '\n')
	//			{
	//				// last character on line of received text starting new line with next character read
	//				currentLineIsBlank = true;
	//			}
	//			else if (c != '\r')
	//			{
	//				// a text character was received from client
	//				currentLineIsBlank = false;
	//			}
	//		}
	//	}

	//	delay(1);      // give the web browser time to receive the data
	//	client.stop(); // close the connection
	//}
}

/* This command is set as the default command for the server.  It
* handles both GET and POST requests.  For a GET, it returns a simple
* page with some buttons.  For a POST, it saves the value posted to
* the red/green/blue variable, affecting the output of the speaker */
void rgbCmd(WebServer &server, WebServer::ConnectionType type, char *, bool)
{
	if (type == WebServer::POST)
	{
		bool repeat;
		char name[16], value[16];
		do
		{
			/* readPOSTparam returns false when there are no more parameters
			* to read from the input.  We pass in buffers for it to store
			* the name and value strings along with the length of those
			* buffers. */
			repeat = server.readPOSTparam(name, 16, value, 16);

			/* this is a standard string comparison function.  It returns 0
			* when there's an exact match.  We're looking for a parameter
			* named red/green/blue here. */
			if (strcmp(name, "red") == 0)
			{
				/* use the STRing TO Unsigned Long function to turn the string
				* version of the color strength value into our integer red/green/blue
				* variable */
				red = strtoul(value, NULL, 10);
			}
			if (strcmp(name, "green") == 0)
			{
				green = strtoul(value, NULL, 10);
			}
			if (strcmp(name, "blue") == 0)
			{
				blue = strtoul(value, NULL, 10);
			}
		} while (repeat);

		// after procesing the POST data, tell the web browser to reload
		// the page using a GET method. 
		server.httpSeeOther(PREFIX);
		//    Serial.print(name);
		//    Serial.println(value);

		return;
	}

	/* for a GET or HEAD, send the standard "it's all OK headers" */
	server.httpSuccess();

	/* we don't output the body for a HEAD request */
	if (type == WebServer::GET)
	{
		/* store the HTML in program memory using the P macro */
		P(message) =
			"<!DOCTYPE html><html><head>"
			"<meta charset=\"utf-8\"><meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />"
			"<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\"><meta name=\"viewport\" content=\"width=device-width, user-scalable=no\">"
			"<title>Webduino RGB</title>"
			"<link rel=\"stylesheet\" href=\"http://code.jquery.com/mobile/1.0/jquery.mobile-1.0.min.css\" />"
			"<script src=\"http://code.jquery.com/jquery-1.6.4.min.js\"></script>"
			"<script src=\"http://code.jquery.com/mobile/1.0/jquery.mobile-1.0.min.js\"></script>"
			"<style> body, .ui-page { background: black; } .ui-body { padding-bottom: 1.5em; } div.ui-slider { width: 88%; } #red, #green, #blue { display: block; margin: 10px; } #red { background: #f00; } #green { background: #0f0; } #blue { background: #00f; } </style>"
			"<script>"
			// causes the Arduino to hang quite frequently (more often than Web_AjaxRGB.pde), probably due to the different event triggering the ajax requests
			"$(document).ready(function(){ $('#red, #green, #blue').slider; $('#red, #green, #blue').bind( 'change', function(event, ui) { jQuery.ajaxSetup({timeout: 110}); /*not to DDoS the Arduino, you might have to change this to some threshold value that fits your setup*/ var id = $(this).attr('id'); var strength = $(this).val(); if (id == 'red') $.post('/rgb', { red: strength } ); if (id == 'green') $.post('/rgb', { green: strength } ); if (id == 'blue') $.post('/rgb', { blue: strength } ); });});"
			"</script>"
			"</head>"
			"<body>"
			"<div data-role=\"header\" data-position=\"inline\"><h1>Webduino RGB</h1></div>"
			"<div class=\"ui-body ui-body-a\">"
			"<input type=\"range\" name=\"slider\" id=\"red\" value=\"0\" min=\"0\" max=\"255\"  />"
			"<input type=\"range\" name=\"slider\" id=\"green\" value=\"0\" min=\"0\" max=\"255\"  />"
			"<input type=\"range\" name=\"slider\" id=\"blue\" value=\"0\" min=\"0\" max=\"255\"  />"
			"</div>"
			"</body>"
			"</html>";

		server.printP(message);
	}
}
void httpNotFound(WebServer &server){
	P(failMsg) =
		"HTTP/1.0 404 Bad Request" CRLF
		WEBDUINO_SERVER_HEADER
		"Content-Type: text/html" CRLF
		CRLF
		"<h2>File Not Found !</h2>";
	server.printP(failMsg);
}

void urlCmd(WebServer &server, WebServer::ConnectionType type, char **url_path, char *url_tail, bool tail_complete)
{
	//Serial.println(url_tail);
	/*
	Use the following to test
	curl "192.168.1.80/fs"
	curl "192.168.1.80/fs/TESTFILE.INO"
	curl -X DELETE "192.168.1.80/fs/TESTFILE.INO"
	Sources
	- http://www.ladyada.net/learn/arduino/ethfiles.html
	Improvements
	- Expose a WebDav interface http://blog.coralbits.com/2011/07/webdav-protocol-for-dummies.html
	*/
	if (!tail_complete)
		server.httpServerError();

	//Only serve files under the "/fs" path
	if (strncmp(url_path[0], "fs", 3) != 0)
	{
		DEBUG_PRINT_PL("Path not found 404");
		httpNotFound(server);
		return;
	}
	if (url_path[1] == 0)
	{
		// do an ls
		server.httpSuccess();
		dir_t p;
		root.rewind();
		while (root.readDir(p) > 0)
		{
			// done if past last used entry
			if (p.name[0] == DIR_NAME_FREE) break;
			// skip deleted entry and entries for . and  ..
			if (p.name[0] == DIR_NAME_DELETED || p.name[0] == '.') continue;
			for (uint8_t i = 0; i < 11; i++) {
				if (p.name[i] == ' ') continue;
				if (i == 8) {
					server.print('.');
				}
				server.print((char)p.name[i]);
			}
			server.print(CRLF);
		}
	}
	else
	{
		// access a file
		SdFile file;
		if (!file.open(&root, url_path[1], O_READ)) {
			httpNotFound(server);
		}
		else {
			if (type == WebServer::GET){
				server.httpSuccess("text/plain");
				char buf[32];
				int16_t  readed;
				readed = file.read(buf, 30);
				while (readed > 0) {
					webserver.write(buf, readed);
					readed = file.read(buf, 30);
				}
			}
			else if (type == WebServer::DELETE){
				DEBUG_PRINT_PL("DELETE");
				server.httpSuccess();
				SD.remove(url_path[1]);
			}
			file.close();
		}
	}
}