
namespace SmartNetwork.API
{
    public enum ControlLineMode : byte
    {
        DigitalInput,
        DigitalOutput,
        AnalogInput, // ADC
        PWM,
        OneWire,
        I2C,
        SPI
    }
}
