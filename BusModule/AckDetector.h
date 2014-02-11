#ifndef ACKDETECTOR_H_
#define ACKDETECTOR_H_
//---------------------------------------------------------------------------------------------
volatile uint8_t isProgPhase;
volatile uint8_t isAckDetected;
//---------------------------------------------------------------------------------------------
void ResetAckFlag();
uint8_t QueryAckFlag();
void CheckAcknowledgement();
//---------------------------------------------------------------------------------------------
#endif /* ACKDETECTOR_H_ */