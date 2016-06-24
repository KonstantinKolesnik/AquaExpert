using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace RPI.Drivers.MPU6050
{
    public class Mpu6050 : IDisposable
    {
        #region Fields
        private bool isDisposed;
        private ReadWriteHelper readWriteHelper;
        private const int InterruptPin = 18;
        private GpioController ioController;
        private GpioPin interruptPin;
        public I2cDevice device;
        #endregion

        #region Events
        public event EventHandler<MpuSensorEventArgs> SensorInterruptEvent;
        #endregion

        #region Constructors
        public Mpu6050()
        {
            readWriteHelper = new ReadWriteHelper(device);
        }
        ~Mpu6050()
        {
            Dispose(false);
        }
        #endregion

        #region Public methods
        public async void InitHardware()
        {
            try
            {
                ioController = GpioController.GetDefault();

                interruptPin = ioController.OpenPin(InterruptPin);
                interruptPin.Write(GpioPinValue.Low);
                interruptPin.SetDriveMode(GpioPinDriveMode.Input);
                interruptPin.ValueChanged += Interrupt;

                var collection = await DeviceInformation.FindAllAsync(I2cDevice.GetDeviceSelector());

                var settings = new I2cConnectionSettings(Constants.Address)
                {
                    BusSpeed = I2cBusSpeed.FastMode,
                    SharingMode = I2cSharingMode.Exclusive
                };
                device = await I2cDevice.FromIdAsync(collection[0].Id, settings);

                await Task.Delay(3); // wait power up sequence

                readWriteHelper.WriteByte(Constants.PwrMgmt1, 0x80); // reset the device
                await Task.Delay(100);
                readWriteHelper.WriteByte(Constants.PwrMgmt1, 0x2);
                readWriteHelper.WriteByte(Constants.UserCtrl, 0x04); //reset fifo

                readWriteHelper.WriteByte(Constants.PwrMgmt1, 1); // clock source = gyro x
                readWriteHelper.WriteByte(Constants.GyroConfig, 0); // +/- 250 degrees sec
                readWriteHelper.WriteByte(Constants.AccelConfig, 0); // +/- 2g

                readWriteHelper.WriteByte(Constants.Config, 1); // 184 Hz, 2ms delay
                readWriteHelper.WriteByte(Constants.SmplrtDiv, 19); // set rate 50Hz
                readWriteHelper.WriteByte(Constants.FifoEn, 0x78); // enable accel and gyro to read into fifo
                readWriteHelper.WriteByte(Constants.UserCtrl, 0x40); // reset and enable fifo
                readWriteHelper.WriteByte(Constants.IntEnable, 0x1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion

        #region Private methods
        private void Interrupt(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (device == null)
                return;

            int interruptStatus = readWriteHelper.ReadByte(Constants.IntStatus);
            if ((interruptStatus & 0x10) != 0)
                readWriteHelper.WriteByte(Constants.UserCtrl, 0x44); // reset and enable fifo

            if ((interruptStatus & 0x1) == 0)
                return;
            var ea = new MpuSensorEventArgs
            {
                Status = (byte)interruptStatus,
                SamplePeriod = 0.02f
            };
            var l = new List<MpuSensorValue>();

            int count = readWriteHelper.ReadWord(Constants.FifoCount);

            while (count >= Constants.SensorBytes)
            {
                var data = readWriteHelper.ReadBytes(Constants.FifoRW, Constants.SensorBytes);
                count -= Constants.SensorBytes;

                var xa = (short)(data[0] << 8 | data[1]);
                var ya = (short)(data[2] << 8 | data[3]);
                var za = (short)(data[4] << 8 | data[5]);

                var xg = (short)(data[6] << 8 | data[7]);
                var yg = (short)(data[8] << 8 | data[9]);
                var zg = (short)(data[10] << 8 | data[11]);

                var sv = new MpuSensorValue
                {
                    AccelerationX = xa / (float)16384,
                    AccelerationY = ya / (float)16384,
                    AccelerationZ = za / (float)16384,
                    GyroX = xg / (float)131,
                    GyroY = yg / (float)131,
                    GyroZ = zg / (float)131
                };
                l.Add(sv);
            }
            ea.Values = l.ToArray();

            if (SensorInterruptEvent == null)
                return;

            if (ea.Values.Length > 0)
                SensorInterruptEvent(this, ea);
        }
        #endregion

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            interruptPin.Dispose();

            if (device != null)
            {
                device.Dispose();
                device = null;
            }

            isDisposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
