using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartHub.UWP.Plugins.Wemos.Core
{
    public class WemosMessage
    {
        #region Properties
        public int NodeNo
        {
            get; set;
        }
        public int SensorNo
        {
            get; set;
        }
        public WemosMessageType Type
        {
            get; set;
        }
        public int SubType
        {
            get; set;
        }

        public string Payload
        {
            get; private set;
        }
        public float PayloadFloat
        {
            get
            {
                if (!string.IsNullOrEmpty(Payload))
                {
                    string ds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string p = Payload.Replace(",", ds).Replace(".", ds);

                    float v;
                    return float.TryParse(p, out v) ? v : float.NaN;
                }

                return float.NaN;
            }
            set
            {
                if (PayloadFloat != value)
                    Payload = value.ToString();
            }
        }
        //public List<int> PayloadFirmware
        //{
        //    get
        //    {
        //        var result = new List<int>();

        //        if (Type == WemosMessageType.Stream)
        //            for (var i = 0; i < Payload.Length; i += 2)
        //                result.Add(int.Parse(Payload.Substring(i, 2), NumberStyles.HexNumber));

        //        return result;
        //    }
        //    set
        //    {
        //        if (Type == WemosMessageType.Stream)
        //        {
        //            string result = "";

        //            for (var i = 0; i < value.Count; i++)
        //            {
        //                if (value[i] < 16) // ??????
        //                    result += "0";
        //                result += value[i].ToString("x");
        //            }

        //            Payload = result;
        //        }
        //    }
        //}
        #endregion

        #region Constructors
        public WemosMessage(int nodeID, int sensorID, WemosMessageType type, int subType, string payload)
        {
            NodeNo = nodeID;
            SensorNo = sensorID;
            Type = type;
            SubType = subType;
            Payload = payload;
        }
        public WemosMessage(int nodeID, int sensorID, WemosMessageType type, int subType, float payload)
        {
            NodeNo = nodeID;
            SensorNo = sensorID;
            Type = type;
            SubType = subType;
            PayloadFloat = payload;
        }
        #endregion

        #region Public methods
        public static List<WemosMessage> FromDto(string str)
        {
            return WemosMessageParser.Parse(str);
        }
        public string ToDto()
        {
            return $"{NodeNo};{SensorNo};{(int)Type};{SubType};{Payload}\n";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("[{0:d3}] [{0:d3}] [{0}] ", NodeNo, SensorNo, Type);
            switch (Type)
            {
                case WemosMessageType.Presentation: sb.AppendFormat("[{0}] ", (WemosLineType) SubType); break;
                case WemosMessageType.Set:
                case WemosMessageType.Request: sb.AppendFormat("[{0}] ", (WemosLineValueType) SubType); break;
                case WemosMessageType.Internal: sb.AppendFormat("[{0}] ", (WemosInternalMessageType) SubType); break;
                case WemosMessageType.Stream: sb.AppendFormat("[{0}] ", (WemosStreamMessageType) SubType); break;
                default: sb.AppendFormat("[{0}] ", SubType); break;
            }
            sb.AppendFormat("[{0}]", Payload);

            return sb.ToString();
        }
        #endregion
    }
}
