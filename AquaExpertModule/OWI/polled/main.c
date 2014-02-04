// This file has been prepared for Doxygen automatic documentation generation.
/*! \file ********************************************************************
*
* Atmel Corporation
*
* \li File:               main.c
* \li Compiler:           IAR EWAAVR 3.20a
* \li Support mail:       avr@atmel.com
*
* \li Supported devices:  All AVRs.
*
* \li Application Note:   AVR318 - Dallas 1-Wire(R) master.
*                         
*
* \li Description:        Example on how to use the 1-Wire(R) interface
*                         master.
*
*                         $Revision: 1.7 $
*                         $Date: Thursday, August 19, 2004 14:27:18 UTC $
****************************************************************************/

//#include "OWIPolled.h"
//#include "OWIHighLevelFunctions.h"
//#include "OWIBitFunctions.h"
//#include "..\common_files\OWIcrc.h"
//#include "..\..\Hardware.h"







/*! \brief  Example application for the polled drivers.
 *
 *  Example application for the software only and polled UART driver.
 *  This example application will find all devices (upper bounded by MAX_DEVICES) 
 *  on the buses defined by BUSES. It then tries to find either a DS1820 or DS2890 
 *  device on a bus, and communicate with them to read temperature (DS1820) or set wiper position (DS2890).
 *  This example is not written in a very optimal way. It is merely intended to show how the polled 1-Wire(R) driver can be used.
 */
void ___main(void)
{
    //static OWI_device devices[MAX_DEVICES];
    //OWI_device* ds1820;
    //OWI_device* ds2890;
    //signed int temperature = 0;
    //unsigned char wiperPos = 0;
    //
    //// Initialize PORTB as output. Can be used to display values on
    //// the LEDs on a STK500 development board.
    //DDRB = 0xff;
//
    //OWI_Init(BUSES);
    //
    //// Do the bus search until all ids are read without crc error.    
    //while (!SearchBuses(devices, MAX_DEVICES, BUSES));
    //
    //// See if there is a DS1820 or DS2890 on a bus.
    //ds1820 = FindFamily(DS1820_FAMILY_ID, devices, MAX_DEVICES);
    //ds2890 = FindFamily(DS2890_FAMILY_ID, devices, MAX_DEVICES);
    //
    //// Do something useful with the slave devices in an eternal loop.
    //for (;;)
    //{
        //// If there is a DS1820 temperature sensor on a bus, read the temperature.
        //// The DS1820 must have Vdd pin connected for this code to work.
        //if (ds1820 != NULL)
            //temperature = DS1820_ReadTemperature((*ds1820).bus, (*ds1820).id);
//
        //// If there is a DS2890 digital potentiometer, increment the wiper value.
        //if (ds2890 != NULL)
            //DS2890_SetWiperPosition(wiperPos++, (*ds2890).bus, (*ds2890).id);
        //
        //// Discard lsb of temperature and output to PORTB.
        //PORTB = ~(temperature >> 1);
    //}
}


