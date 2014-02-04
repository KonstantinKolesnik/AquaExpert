#include "TemperatureSensors.h"

OWI_device* ds1820;

void InitTemperatureSensors()
{
	//ds1820 = FindFamily(DS1820_FAMILY_ID, devices, MAX_DEVICES);
}

//signed int GetTemperature(uint8_t channel)
//{
	//// If there is a DS1820 temperature sensor on a bus, read the temperature.
	//// The DS1820 must have Vdd pin connected for this code to work.
	////if (ds1820 != NULL)
		////temperature = DS1820_ReadTemperature((*ds1820).bus, (*ds1820).id);
//
//}
//
/*! \brief  Read the temperature from a DS1820 temperature sensor.
 *
 *  This function will start a conversion and read back the temperature from a DS1820 temperature sensor.
 *
 *  \param  bus A bit mask of the bus where the DS1820 is located.
 *  \param  id  The 64-bit identifier of the DS1820.
 *  \return The 16-bit signed temperature read from the DS1820.
 */
signed int DS1820_ReadTemperature(unsigned char bus, unsigned char* id)
{
    signed int temperature;
    
    // Reset, presence.
    if (!OWI_DetectPresence(bus))
        return DS1820_ERROR;

    // Match the id found earlier.
    OWI_MatchRom(id, bus);
	
    // Send start conversion command.
    OWI_SendByte(DS1820_START_CONVERSION, bus);
	
    // Wait until conversion is finished. Bus line is held low until conversion is finished.
    while (!OWI_ReadBit(bus));

    // Reset, presence.
    if (!OWI_DetectPresence(bus))
        return DS1820_ERROR;

    // Match id again.
    OWI_MatchRom(id, bus);
	
    // Send READ SCRATCHPAD command.
    OWI_SendByte(DS1820_READ_SCRATCHPAD, bus);
	
    // Read only two first bytes (temperature low, temperature high) and place them in the 16 bit temperature variable.
    temperature = OWI_ReceiveByte(bus);
    temperature |= (OWI_ReceiveByte(bus) << 8);
    
    return temperature;
}

/*! \brief  Set the wiper position of a DS2890.
 *
 *  This function initializes the DS2890 by enabling the charge pump. It then changes the wiper position.
 *
 *  \param  position    The new wiper position.
 *  \param  bus         The bus where the DS2890 is connected.
 *  \param  id          The 64 bit identifier of the DS2890.
 */
//void DS2890_SetWiperPosition(unsigned char position, unsigned char bus, unsigned char* id)
//{
    //// Reset, presence.
    //if(!OWI_DetectPresence(bus))
    //{
        //return;
    //}
    ////Match id.
    //OWI_MatchRom(id, bus);
    //
    //// Send Write control register command.
    //OWI_SendByte(DS2890_WRITE_CONTROL_REGISTER, bus);
    //
    //// Write 0x4c to control register to enable charge pump.
    //OWI_SendByte(0x4c, bus);
    //
    //// Check that the value returned matches the value sent.
    //if (OWI_ReceiveByte(bus) != 0x4c)
    //{
        //return;
    //}
    //
    //// Send release code to update control register.
    //OWI_SendByte(DS2890_RELEASE_CODE, bus);
    //
    //// Check that zeros are returned to ensure that the operation was
    //// successful.
    //if (OWI_ReceiveByte(bus) == 0xff)
    //{
        //return;
    //}
    //
    //// Reset, presence.
    //if (!OWI_DetectPresence(bus))
    //{
        //return;
    //}
    //
    //// Match id.
    //OWI_MatchRom(id, bus);
    //
    //// Send the Write Position command.
    //OWI_SendByte(DS2890_WRITE_POSITION, bus);
    //
    //// Send the new position.
    //OWI_SendByte(position, bus);
    //
    //// Check that the value returned matches the value sent.
    //if (OWI_ReceiveByte(bus) != position)
    //{
        //return;
    //}
    //
    //// Send release code to update wiper position.
    //OWI_SendByte(DS2890_RELEASE_CODE, bus);
    //
    //// Check that zeros are returned to ensure that the operation was
    //// successful.
    //if (OWI_ReceiveByte(bus) == 0xff)
    //{   
        //return;
    //}
//}
