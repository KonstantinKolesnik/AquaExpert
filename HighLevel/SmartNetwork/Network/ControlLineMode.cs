using System;

namespace SmartNetwork.Network
{
    [Flags]
    public enum ControlLineMode : byte
    {
        // states: 0, 1;
        DigitalInput = 0,
        
        // states: 0, 1
        DigitalOutput = 1,
        
        // ADC; states: 0...1024
        AnalogInput = 2,
        
        // states: 0...255
        PWM = 4,
        
        // state: float?
        OneWire = 8, // ?
        
        SPI = 16, // ?
        
        I2C = 32 // ?
    }
}
