#include "OWI.h"

//#include <avr/io.h>
//#include "OWIBitFunctions.h"
//

OWI_device devices[MAX_DEVICES];

/*! \brief  Perform a 1-Wire search
 *
 *  This function shows how the OWI_SearchRom function can be used to 
 *  discover all slaves on the bus. It will also CRC check the 64 bit identifiers.
 *
 *  \param  devices Pointer to an array of type OWI_device. The discovered devices will be placed from the beginning of this array.
 *  \param  len     The length of the device array. (Max. number of elements).
 *  \param  buses   Bit mask of the buses to perform search on.
 *
 *  \retval true    Search completed successfully.
 *  \retval false   A CRC error occurred. Probably because of noise during transmission.
 */
bool SearchBuses(OWI_device* devices, unsigned char len, unsigned char buses)
{
    unsigned char i, j;
    unsigned char presence;
    unsigned char* newID;
    unsigned char* currentID;
    unsigned char currentBus;
    unsigned char lastDeviation;
    unsigned char numDevices;
    
    // Initialize all addresses as zero, on bus 0 (does not exist).
    // Do a search on the bus to discover all addresses.    
    for (i = 0; i < len; i++)
    {
        devices[i].bus = 0x00;
        for (j = 0; j < 8; j++)
            devices[i].id[j] = 0x00;
    }
    
    // Find the buses with slave devices.
    presence = OWI_DetectPresence(BUSES);
    
    numDevices = 0;
    newID = devices[0].id;
    
    // Go through all buses with slave devices.
    for (currentBus = 0x01; currentBus; currentBus <<= 1)
    {
        lastDeviation = 0;
        currentID = newID;
        if (currentBus & presence) // Devices available on this bus.
        {
            // Do slave search on each bus, and place identifiers and corresponding
            // bus "addresses" in the array.
            do  
            {
                memcpy(newID, currentID, 8);
                OWI_DetectPresence(currentBus);
                lastDeviation = OWI_SearchRom(newID, lastDeviation, currentBus);
                currentID = newID;
                devices[numDevices].bus = currentBus;
                numDevices++;
                newID=devices[numDevices].id;                
            }  while(lastDeviation != OWI_ROM_SEARCH_FINISHED);            
        }
    }

    // Go through all the devices and do CRC check.
    for (i = 0; i < numDevices; i++)
    {
        // If any id has a CRC error, return error.
        if (OWI_CheckRomCRC(devices[i].id) != OWI_CRC_OK)
            return false;
    }
	
    return true;
}

/*! \brief  Find the first device of a family based on the family id
 *
 *  This function returns a pointer to a device in the device array that matches the specified family.
 *
 *  \param  familyID    The 8 bit family ID to search for.
 *  \param  devices     An array of devices to search through.
 *  \param  size        The size of the array 'devices'
 *
 *  \return A pointer to a device of the family.
 *  \retval NULL if no device of the family was found.
 */
OWI_device* FindFamily(unsigned char familyID, OWI_device* devices, unsigned char size)
{
    unsigned char i = 0;
    
    // Search through the array.
    while (i < size)
    {
        // Return the pointer if there is a family id match.
        if ((*devices).id[0] == familyID)
            return devices;

        devices++;
        i++;
    }
    // Else, return NULL.
    return NULL;
}

void InitOWI()
{
	//OWI_Init(BUSES);
//
	//// Do the bus search until all ids are read without CRC error.
	//while (!SearchBuses(devices, MAX_DEVICES, BUSES));

}


/*! \brief  Sends one byte of data on the 1-Wire(R) bus(es).
 *  
 *  This function automates the task of sending a complete byte
 *  of data on the 1-Wire bus(es).
 *
 *  \param  data    The data to send on the bus(es).
 *  
 *  \param  pins    A bitmask of the buses to send the data to.
 */
void OWI_SendByte(unsigned char data, unsigned char pins)
{
    unsigned char temp;
    unsigned char i;
    
    // Do once for each bit
    for (i = 0; i < 8; i++)
    {
        // Determine if lsb is '0' or '1' and transmit corresponding
        // waveform on the bus.
        temp = data & 0x01;
        if (temp)
        {
            OWI_WriteBit1(pins);
        }
        else
        {
            OWI_WriteBit0(pins);
        }
        // Right shift the data to get next bit.
        data >>= 1;
    }
}

/*! \brief  Receives one byte of data from the 1-Wire(R) bus.
 *
 *  This function automates the task of receiving a complete byte 
 *  of data from the 1-Wire bus.
 *
 *  \param  pin     A bitmask of the bus to read from.
 *  
 *  \return     The byte read from the bus.
 */
unsigned char OWI_ReceiveByte(unsigned char pin)
{
    unsigned char data;
    unsigned char i;

    // Clear the temporary input variable.
    data = 0x00;
    
    // Do once for each bit
    for (i = 0; i < 8; i++)
    {
        // Shift temporary input variable right.
        data >>= 1;
        // Set the msb if a '1' value is read from the bus.
        // Leave as it is ('0') else.
        if (OWI_ReadBit(pin))
        {
            // Set msb
            data |= 0x80;
        }
    }
    return data;
}

/*! \brief  Sends the SKIP ROM command to the 1-Wire bus(es).
 *
 *  \param  pins    A bitmask of the buses to send the SKIP ROM command to.
 */
void OWI_SkipRom(unsigned char pins)
{
    // Send the SKIP ROM command on the bus.
    OWI_SendByte(OWI_ROM_SKIP, pins);
}

/*! \brief  Sends the READ ROM command and reads back the ROM id.
 *
 *  \param  romValue    A pointer where the id will be placed.
 *
 *  \param  pin     A bit mask of the bus to read from.
 */
void OWI_ReadRom(unsigned char * romValue, unsigned char pin)
{
    unsigned char bytesLeft = 8;

    // Send the READ ROM command on the bus.
    OWI_SendByte(OWI_ROM_READ, pin);
    
    // Do 8 times.
    while (bytesLeft > 0)
    {
        // Place the received data in memory.
        *romValue++ = OWI_ReceiveByte(pin);
        bytesLeft--;
    }
}

/*! \brief  Sends the MATCH ROM command and the ROM id to match against.
 *
 *  \param  romValue    A pointer to the ID to match against.
 *
 *  \param  pins    A bitmask of the buses to perform the MATCH ROM command on.
 */
void OWI_MatchRom(unsigned char * romValue, unsigned char pins)
{
    unsigned char bytesLeft = 8;   
    
    // Send the MATCH ROM command.
    OWI_SendByte(OWI_ROM_MATCH, pins);

    // Do once for each byte.
    while (bytesLeft > 0)
    {
        // Transmit 1 byte of the ID to match.
        OWI_SendByte(*romValue++, pins);
        bytesLeft--;
    }
}

/*! \brief  Sends the SEARCH ROM command and returns 1 id found on the 
 *          1-Wire(R) bus.
 *
 *  \param  bitPattern      A pointer to an 8 byte char array where the 
 *                          discovered identifier will be placed. When 
 *                          searching for several slaves, a copy of the 
 *                          last found identifier should be supplied in 
 *                          the array, or the search will fail.
 *
 *  \param  lastDeviation   The bit position where the algorithm made a 
 *                          choice the last time it was run. This argument 
 *                          should be 0 when a search is initiated. Supplying 
 *                          the return argument of this function when calling 
 *                          repeatedly will go through the complete slave 
 *                          search.
 *
 *  \param  pin             A bit-mask of the bus to perform a ROM search on.
 *
 *  \return The last bit position where there was a discrepancy between slave addresses the last time this function was run. Returns OWI_ROM_SEARCH_FAILED if an error was detected (e.g. a device was connected to the bus during the search), or OWI_ROM_SEARCH_FINISHED when there are no more devices to be discovered.
 *
 *  \note   See main.c for an example of how to utilize this function.
 */
unsigned char OWI_SearchRom(unsigned char * bitPattern, unsigned char lastDeviation, unsigned char pin)
{
    unsigned char currentBit = 1;
    unsigned char newDeviation = 0;
    unsigned char bitMask = 0x01;
    unsigned char bitA;
    unsigned char bitB;

    // Send SEARCH ROM command on the bus.
    OWI_SendByte(OWI_ROM_SEARCH, pin);
    
    // Walk through all 64 bits.
    while (currentBit <= 64)
    {
        // Read bit from bus twice.
        bitA = OWI_ReadBit(pin);
        bitB = OWI_ReadBit(pin);

        if (bitA && bitB)
        {
            // Both bits 1 (Error).
            newDeviation = OWI_ROM_SEARCH_FAILED;
            break;
        }
        else if (bitA ^ bitB)
        {
            // Bits A and B are different. All devices have the same bit here.
            // Set the bit in bitPattern to this value.
            if (bitA)
            {
                (*bitPattern) |= bitMask;
            }
            else
            {
                (*bitPattern) &= ~bitMask;
            }
        }
        else // Both bits 0
        {
            // If this is where a choice was made the last time,
            // a '1' bit is selected this time.
            if (currentBit == lastDeviation)
            {
                (*bitPattern) |= bitMask;
            }
            // For the rest of the id, '0' bits are selected when
            // discrepancies occur.
            else if (currentBit > lastDeviation)
            {
                (*bitPattern) &= ~bitMask;
                newDeviation = currentBit;
            }
            // If current bit in bit pattern = 0, then this is
            // out new deviation.
            else if ( !(*bitPattern & bitMask)) 
            {
                newDeviation = currentBit;
            }
            // IF the bit is already 1, do nothing.
            else
            {
            
            }
        }
                
        
        // Send the selected bit to the bus.
        if ((*bitPattern) & bitMask)
        {
            OWI_WriteBit1(pin);
        }
        else
        {
            OWI_WriteBit0(pin);
        }

        // Increment current bit.    
        currentBit++;

        // Adjust bitMask and bitPattern pointer.    
        bitMask <<= 1;
        if (!bitMask)
        {
            bitMask = 0x01;
            bitPattern++;
        }
    }
	
    return newDeviation;
}
