using System;

namespace SmartNetwork.Core.Hardware
{
    [Flags]
    public enum ControlLineMode : byte
    {
        // states: 0, 1;
        DigitalInput = 1,
        
        // states: 0, 1
        DigitalOutput = 2,
        
        // ADC; states: 0...1024
        AnalogInput = 4,
        
        // states: 0...255
        PWM = 8,
        
        // state: float?
        OneWireBus = 16, // ?
        
        SPIBus = 32, // ?
        
        I2CBus = 64 // ?
    }
}
