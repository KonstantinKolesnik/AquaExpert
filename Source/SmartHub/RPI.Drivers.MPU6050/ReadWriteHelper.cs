using System;
using Windows.Devices.I2c;

namespace RPI.Drivers.MPU6050
{
    public class ReadWriteHelper
    {
        private I2cDevice device;

        public ReadWriteHelper(I2cDevice device)
        {
            this.device = device;
        }

        public byte ReadByte(byte regAddr)
        {
            byte[] buffer = new byte[1];
            buffer[0] = regAddr;
            byte[] value = new byte[1];
            device.WriteRead(buffer, value);
            return value[0];
        }
        public byte[] ReadBytes(byte regAddr, int length)
        {
            byte[] values = new byte[length];
            byte[] buffer = new byte[1];
            buffer[0] = regAddr;
            device.WriteRead(buffer, values);
            return values;
        }
        public ushort ReadWord(byte address)
        {
            byte[] buffer = ReadBytes(Constants.FifoCount, 2);
            return (ushort)(((int)buffer[0] << 8) | (int)buffer[1]);
        }

        public void WriteByte(byte regAddr, byte data)
        {
            byte[] buffer = new byte[2];
            buffer[0] = regAddr;
            buffer[1] = data;
            device.Write(buffer);
        }
        private void WriteBytes(byte regAddr, byte[] values)
        {
            byte[] buffer = new byte[1 + values.Length];
            buffer[0] = regAddr;
            Array.Copy(values, 0, buffer, 1, values.Length);
            device.Write(buffer);
        }
    }
}
