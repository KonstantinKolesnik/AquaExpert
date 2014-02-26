#ifndef DCCOUT_H_
#define DCCOUT_H_
//****************************************************************************************
/*
typedef enum {is_void,      // message with no special handling (like functions)
              is_stop,      // broadcast
              is_loco,      // standard dcc speed command
              is_acc,       // accessory command
              is_feedback,  // accessory command with feedback
              is_prog,      // service mode - longer preambles
              is_prog_ack}  MessageType;
*/

#define MAX_DCC_SIZE 5 // (up to 2) for address + (up to 3) for data

typedef struct 
{
	uint8_t Repeats;
    uint8_t Size;
    uint8_t Dcc[MAX_DCC_SIZE];
    //MessageType   Type;
} Message_t;
//****************************************************************************************
extern volatile uint8_t Repeats;
//****************************************************************************************
void InitDCCOut();
//****************************************************************************************
void DCCOutEnableRailcom();
void DCCOutDisableRailcom();
uint8_t DCCOutQueryRailcom();
//****************************************************************************************
#endif /* DCCOUT_H_ */