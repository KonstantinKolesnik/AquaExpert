// Example sketch showing how to control ir devices.
// An IR LED must be connected to Arduino PWM pin 3.
// An IR receiver can be connected to PWM pin 8.
// All receied IR signals will be sent to gateway device stored in V_IR_RECEIVE.
//--------------------------------------------------------------------------------------------------------------------------------------------
#include <SPI.h>
#include <MySensor.h>
#include <IRLib.h>
//--------------------------------------------------------------------------------------------------------------------------------------------
#define IR_RECEIVER_SENSOR_ID		0
#define IR_TRANSMITTER_SENSOR_ID	1

#define RECEIVER_PIN				8
IRrecv irrecv(RECEIVER_PIN);
IRdecode decoder;
IRdecodeHash hashDecoder;
//decode_results results;
//unsigned int Buffer[RAWBUF];

MyMessage msgReceive(IR_RECEIVER_SENSOR_ID, V_IR_RECEIVE);

IRsend irsend;
//--------------------------------------------------------------------------------------------------------------------------------------------
MySensor gw;
//--------------------------------------------------------------------------------------------------------------------------------------------
void setup()
{
	Serial.begin(115200);

	irrecv.enableIRIn();
	//decoder.UseExtnBuf(Buffer);
	
	Serial.println("Initialized.");
	
	gw.begin(onMessageReceived);
	gw.sendSketchInfo("IR Transceiver", "1.0");

	gw.present(IR_RECEIVER_SENSOR_ID, S_IR, "IR receiver");
	gw.present(IR_TRANSMITTER_SENSOR_ID, S_IR, "IR transmitter");
}
void loop()
{
	gw.process();

	if (irrecv.GetResults(&decoder))
	{
		irrecv.resume();

		if (decoder.decode())
		{
			//decoder.DumpResults();

			char buffer[10];
			sprintf(buffer, "%08lx", decoder.value);
			Serial.println(buffer);

			Serial.println(decoder.value, HEX);

			//gw.send(msg.set(buffer));
		}
	}


	if (irrecv.GetResults(&decoder)) //Puts results into decoder
	{
		hashDecoder.copyBuf(&decoder); //copy the results to the hash decoder
		irrecv.resume();
		
		decoder.decode();
		Serial.print("Protocol type: ");
		Serial.print(Pnames(decoder.decode_type));
		Serial.print(", value: 0x");
		Serial.println(decoder.value, HEX);

		hashDecoder.decode();
		Serial.print("Hash: 0x");
		Serial.println(hashDecoder.hash, HEX); // Do something interesting with this value

		Serial.println();
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------
void onMessageReceived(const MyMessage &message)
{
	uint8_t cmd = mGetCommand(message);

	if (cmd == C_SET && message.type == V_IR_SEND)
	{
		//irsend.send(NEC, message.getULong(), 32);


		//int incomingRelayStatus = message.getInt();
		//if (incomingRelayStatus == 1)
		//	irsend.send(NEC, 0x1EE17887, 32); // Vol up yamaha ysp-900
		//else
		//	irsend.send(NEC, 0x1EE1F807, 32); // Vol down yamaha ysp-900

		irrecv.enableIRIn();
	}
}
//--------------------------------------------------------------------------------------------------------------------------------------------


// Dumps out the decode_results structure.
// Call this after IRrecv::decode()
// void * to work around compiler issue
//void dump(void *v) {
//  decode_results *results = (decode_results *)v

//void dump(decode_results *results)
//{
//	int count = results->rawlen;
//	if (results->decode_type == UNKNOWN)
//		Serial.print("Unknown encoding: ");
//	else if (results->decode_type == NEC)
//		Serial.print("Decoded NEC: ");
//	else if (results->decode_type == SONY)
//		Serial.print("Decoded SONY: ");
//	else if (results->decode_type == RC5)
//		Serial.print("Decoded RC5: ");
//	else if (results->decode_type == RC6)
//		Serial.print("Decoded RC6: ");
//	else if (results->decode_type == PANASONIC)
//	{
//		Serial.print("Decoded PANASONIC - Address: ");
//		Serial.print(results->panasonicAddress,HEX);
//		Serial.print(" Value: ");
//	}
//	else if (results->decode_type == JVC)
//		Serial.print("Decoded JVC: ");
//
//	Serial.print(results->value, HEX);
//	Serial.print(" (");
//	Serial.print(results->bits, DEC);
//	Serial.println(" bits)");
//
//	Serial.print("Raw (");
//	Serial.print(count, DEC);
//	Serial.print("): ");
//
//	for (int i = 0; i < count; i++)
//	{
//		if ((i % 2) == 1)
//			Serial.print(results->rawbuf[i]*USECPERTICK, DEC);
//		else
//			Serial.print(-(int)results->rawbuf[i]*USECPERTICK, DEC);
//
//		Serial.print(" ");
//	}
//	Serial.println("");
//}

