// This file has been prepared for Doxygen automatic documentation generation.
/*! \file ********************************************************************
*
* Atmel Corporation
*
* \li File:               OWISWBitFunctions.c
* \li Compiler:           IAR EWAAVR 3.20a
* \li Support mail:       avr@atmel.com
*
* \li Supported devices:  All AVRs.
*
* \li Application Note:   AVR318 - Dallas 1-Wire(R) master.
*                         
*
* \li Description:        Polled software only implementation of the basic 
*                         bit-level signaling in the 1-Wire(R) protocol.
*
*                         $Revision: 1.7 $
*                         $Date: Thursday, August 19, 2004 14:27:18 UTC $
****************************************************************************/

#include "..\Hardware.h"
#include "OWIBitFunctions.h"
#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>

#ifdef OWI_SOFTWARE_DRIVER

/*! \brief Initialization of the one wire bus(es). (Software only driver)
 *  
 *  This function initializes the 1-Wire bus(es) by releasing it and
 *  waiting until any presence signals are finished.
 *
 *  \param  pins    A bit mask of the buses to initialize.
 */
void OWI_Init(unsigned char pins)
{
    OWI_RELEASE_BUS(pins);
    // The first rising edge can be interpreted by a slave as the end of a
    // Reset pulse. Delay for the required reset recovery time (H) to be 
    // sure that the real reset is interpreted correctly.
    __delay_cycles(OWI_DELAY_H_STD_MODE);
}

/*! \brief  Write a '1' bit to the bus(es). (Software only driver)
 *
 *  Generates the waveform for transmission of a '1' bit on the 1-Wire
 *  bus.
 *
 *  \param  pins    A bit mask of the buses to write to.
 */
void OWI_WriteBit1(unsigned char pins)
{
    unsigned char intState;
    
    // Disable interrupts.
    intState = __save_interrupt();
    __disable_interrupt();
    
    // Drive bus low and delay.
    OWI_PULL_BUS_LOW(pins);
    __delay_cycles(OWI_DELAY_A_STD_MODE);
    
    // Release bus and delay.
    OWI_RELEASE_BUS(pins);
    __delay_cycles(OWI_DELAY_B_STD_MODE);
    
    // Restore interrupts.
    __restore_interrupt(intState);
}

/*! \brief  Write a '0' to the bus(es). (Software only driver)
 *
 *  Generates the waveform for transmission of a '0' bit on the 1-Wire(R) bus.
 *
 *  \param  pins    A bit mask of the buses to write to.
 */
void OWI_WriteBit0(unsigned char pins)
{
    unsigned char intState;
    
    // Disable interrupts.
    intState = __save_interrupt();
    __disable_interrupt();
    
    // Drive bus low and delay.
    OWI_PULL_BUS_LOW(pins);
    __delay_cycles(OWI_DELAY_C_STD_MODE);
    
    // Release bus and delay.
    OWI_RELEASE_BUS(pins);
    __delay_cycles(OWI_DELAY_D_STD_MODE);
    
    // Restore interrupts.
    __restore_interrupt(intState);
}

/*! \brief  Read a bit from the bus(es). (Software only driver)
 *
 *  Generates the waveform for reception of a bit on the 1-Wire(R) bus(es).
 *
 *  \param  pins    A bitmask of the bus(es) to read from.
 *
 *  \return A bitmask of the buses where a '1' was read.
 */
unsigned char OWI_ReadBit(unsigned char pins)
{
    unsigned char intState;
    unsigned char bitsRead;
    
    // Disable interrupts.
    intState = __save_interrupt();
    __disable_interrupt();
    
    // Drive bus low and delay.
    OWI_PULL_BUS_LOW(pins);
    __delay_cycles(OWI_DELAY_A_STD_MODE);
    
    // Release bus and delay.
    OWI_RELEASE_BUS(pins);
    __delay_cycles(OWI_DELAY_E_STD_MODE);
    
    // Sample bus and delay.
    bitsRead = OWI_PIN & pins;
    __delay_cycles(OWI_DELAY_F_STD_MODE);
    
    // Restore interrupts.
    __restore_interrupt(intState);
    
    return bitsRead;
}

/*! \brief  Send a Reset signal and listen for Presence signal. (software
 *  only driver)
 *
 *  Generates the waveform for transmission of a Reset pulse on the 
 *  1-Wire(R) bus and listens for presence signals.
 *
 *  \param  pins    A bitmask of the buses to send the Reset signal on.
 *
 *  \return A bitmask of the buses where a presence signal was detected.
 */
unsigned char OWI_DetectPresence(unsigned char pins)
{
    unsigned char intState;
    unsigned char presenceDetected;
    
    // Disable interrupts.
    intState = __save_interrupt();
    __disable_interrupt();
    
    // Drive bus low and delay.
    OWI_PULL_BUS_LOW(pins);
    __delay_cycles(OWI_DELAY_H_STD_MODE);
    
    // Release bus and delay.
    OWI_RELEASE_BUS(pins);
    __delay_cycles(OWI_DELAY_I_STD_MODE);
    
    // Sample bus to detect presence signal and delay.
    presenceDetected = ((~OWI_PIN) & pins);
    __delay_cycles(OWI_DELAY_J_STD_MODE);
    
    // Restore interrupts.
    __restore_interrupt(intState);
    
    return presenceDetected;
}

#endif

#ifdef OWI_UART_DRIVER

/*! \brief Initialization of the one wire bus. (Polled UART driver)
 *  
 *  This function initializes the 1-Wire bus by configuring the UART.
 */
void OWI_Init()
{
    // Choose single or double UART speed.
    OWI_UART_STATCTRL_REG_A = (OWI_UART_2X << OWI_U2X);

    // Enable UART transmitter and receiver.
    OWI_UART_STATCTRL_REG_B = (1 << OWI_TXEN) | (1 << OWI_RXEN);
    
    // Set up asynchronous mode, 8 data bits, no parity, 1 stop bit.
    // (Initial value, can be removed)
#ifdef URSEL
    OWI_UART_STATCTRL_REG_C = (1 << OWI_URSEL) | (1 << OWI_UCSZ1) | (1 << OWI_UCSZ0);
#else
    OWI_UART_STATCTRL_REG_C = (1 << OWI_UCSZ1) | (1 << OWI_UCSZ0);
#endif

    OWI_UART_BAUD_RATE_REG_L = OWI_UBRR_115200;    
}

/*! \brief  Write and read one bit to/from the 1-Wire bus. (Polled UART driver)
 *
 *  Writes one bit to the bus and returns the value read from the bus.
 *
 *  \param  outValue    The value to transmit on the bus.
 *
 *  \return The value received by the UART from the bus.
 */
unsigned char OWI_TouchBit(unsigned char outValue)
{
    // Place the output value in the UART transmit buffer, and wait
    // until it is received by the UART receiver.
    OWI_UART_DATA_REGISTER = outValue;
    while(!(OWI_UART_STATCTRL_REG_A & (1 << OWI_RXC)))
    {

    }
    // Set the UART Baud Rate back to 115200kbps when finished.
    OWI_UART_BAUD_RATE_REG_L = OWI_UBRR_115200;
    return OWI_UART_DATA_REGISTER;
}

/*! \brief Write a '1' bit to the bus(es). (Polled UART DRIVER)
 *
 *  Generates the waveform for transmission of a '1' bit on the 1-Wire
 *  bus.
 */
void OWI_WriteBit1()
{
    OWI_TouchBit(OWI_UART_WRITE1);
}

/*! \brief  Write a '0' to the bus(es). (Polled UART DRIVER)
 *
 *  Generates the waveform for transmission of a '0' bit on the 1-Wire(R)
 *  bus.
 */
void OWI_WriteBit0()
{
    OWI_TouchBit(OWI_UART_WRITE0);
}

/*! \brief  Read a bit from the bus(es). (Polled UART DRIVER)
 *
 *  Generates the waveform for reception of a bit on the 1-Wire(R) bus(es).
 *
 *  \return The value read from the bus (0 or 1).
 */
unsigned char OWI_ReadBit()
{
     // Return 1 if the value received matches the value sent.
     // Return 0 else. (A slave held the bus low).
     return (OWI_TouchBit(OWI_UART_READ_BIT) == OWI_UART_READ_BIT);
}

/*! \brief  Send a Reset signal and listen for Presence signal. (Polled 
 *  UART DRIVER)
 *
 *  Generates the waveform for transmission of a Reset pulse on the 
 *  1-Wire(R) bus and listens for presence signals.
 *
 *  \return A bitmask of the buses where a presence signal was detected.
 */
unsigned char OWI_DetectPresence()
{
    // Reset UART receiver to clear RXC register.
    OWI_UART_STATCTRL_REG_B &= ~(1 << OWI_RXEN);
    OWI_UART_STATCTRL_REG_B |= (1 << OWI_RXEN);
    
    // Set UART Baud Rate to 9600 for Reset/Presence signalling.
    OWI_UART_BAUD_RATE_REG_L = OWI_UBRR_9600;
    
    // Return 0 if the value received matches the value sent.
    // return 1 else. (Presence detected)
    return (OWI_TouchBit(OWI_UART_RESET) != OWI_UART_RESET); 
}


#endif
