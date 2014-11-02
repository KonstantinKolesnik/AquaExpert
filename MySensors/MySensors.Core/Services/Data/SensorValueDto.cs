using MySensors.Core.Sensors;
using SQLite;
using System;

namespace MySensors.Core.Services.Data
{
    [Table("SensorValues")]
    class SensorValueDto
    {
        [PrimaryKey]
        public string PK { get; set; }

        //[ForeignKey(typeof(NodeDto))]
        public byte NodeID { get; set; }
        //[ForeignKey(typeof(SensorDto))]
        public byte ID { get; set; }
        public DateTime Time { get; set; }
        public byte Type { get; set; }
        public float Value { get; set; }

        public static SensorValueDto FromModel(SensorValue item)
        {
            if (item == null)
                return null;

            return new SensorValueDto()
            {
                PK = GetPK(item.Time),
                NodeID = item.NodeID,
                ID = item.ID,
                Time = item.Time,
                Type = (byte)item.Type,
                Value = item.Value
            };
        }
        public SensorValue ToModel()
        {
            return new SensorValue(NodeID, ID, Time, (SensorValueType)Type, Value);
        }

        private static string GetPK(DateTime dt)
        {
            long ticks = dt.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            return Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
        }


        //public string GetUniqueKey(int maxSize)
        //{
        //    char[] chars = new char[62];
        //    chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        //    byte[] data = new byte[1];
        //    RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        //    crypto.GetNonZeroBytes(data);
        //    data = new byte[maxSize];
        //    crypto.GetNonZeroBytes(data);
        //    StringBuilder result = new StringBuilder(maxSize);
        //    foreach (byte b in data)
        //    {
        //        result.Append(chars[b % (chars.Length)]);
        //    }
        //    return result.ToString();
        //}

        //static string uniqueCode()
        //{
        //    string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#";
        //    string ticks = DateTime.UtcNow.Ticks.ToString();
        //    var code = "";
        //    for (var i = 0; i < characters.Length; i += 2)
        //    {
        //        if ((i + 2) <= ticks.Length)
        //        {
        //            var number = int.Parse(ticks.Substring(i, 2));
        //            if (number > characters.Length - 1)
        //            {
        //                var one = double.Parse(number.ToString().Substring(0, 1));
        //                var two = double.Parse(number.ToString().Substring(1, 1));
        //                code += characters[Convert.ToInt32(one)];
        //                code += characters[Convert.ToInt32(two)];
        //            }
        //            else
        //                code += characters[number];
        //        }
        //    }
        //    return code;
        //}


        //private ulong GetPK()
        //{
        //    //ulong kind = (ulong)(int)Time.Kind;
        //    //return (kind << 62) | (ulong)Time.Ticks;

        //    RNGCryptoServiceProvider

        //    ulong kind = (int)Time.Kind;
        //    return (kind << 62) | (ulong)Time.Ticks;
        //}

        //public override int GetHashCode()
        //{
        //    long internalTicks = this.InternalTicks;
        //    return (((int)internalTicks) ^ ((int)(internalTicks >> 0x20)));
        //}
    }
}
