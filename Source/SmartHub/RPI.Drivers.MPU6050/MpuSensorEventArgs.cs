namespace RPI.Drivers.MPU6050
{
    public class MpuSensorEventArgs
    {
        public byte Status { get; set; }
        public float SamplePeriod { get; set; }
        public MpuSensorValue[] Values { get; set; }
    }
}
