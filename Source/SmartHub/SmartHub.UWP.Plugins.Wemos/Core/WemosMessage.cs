using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartHub.UWP.Plugins.Wemos.Core
{
    public class WemosMessage
    {
        #region Fields
        private string data = string.Empty;
        #endregion

        #region Properties
        public int NodeID
        {
            get; set;
        }
        public int LineID
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

        public string Data => data;

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
        public WemosMessage(int nodeID, int lineID, WemosMessageType type, int subType)
        {
            NodeID = nodeID;
            LineID = lineID;
            Type = type;
            SubType = subType;
        }
        #endregion

        #region Public methods
        public static List<WemosMessage> FromDto(string str)
        {
            return WemosMessageParser.Parse(str);
        }
        public string ToDto()
        {
            return $"{NodeID};{LineID};{(int)Type};{SubType};{data}\n";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("[{0:d}] ", NodeID));
            sb.Append(string.Format("[{0:d}] ", LineID));
            sb.Append(string.Format("[{0}] ", Type));
            switch (Type)
            {
                case WemosMessageType.Presentation: sb.AppendFormat("[{0}] ", (WemosLineType) SubType); break;
                case WemosMessageType.Set:
                case WemosMessageType.Request: sb.AppendFormat("[{0}] ", (WemosMessageDataType) SubType); break;
                case WemosMessageType.Internal: sb.AppendFormat("[{0}] ", (WemosInternalMessageType) SubType); break;
                case WemosMessageType.Stream: sb.AppendFormat("[{0}] ", (WemosStreamMessageType) SubType); break;
                default: sb.AppendFormat("[{0}] ", SubType); break;
            }
            sb.AppendFormat("[{0}]", data);

            return sb.ToString();
        }

        public WemosMessage Set(string value)
        {
            data = value ?? string.Empty;
            return this;
        }
        public WemosMessage Set(bool value)
        {
            data = value ? "1" : "0";
            return this;
        }
        public WemosMessage Set(long value)
        {
            data = value.ToString();
            return this;
        }
        public WemosMessage Set(float value)
        {
            data = value.ToString();
            return this;
        }

        public string GetString()
        {
            return data;
        }
        public bool GetBoolean()
        {
            bool result = false;
            if (bool.TryParse(data, out result))
                return result;

            return false;
        }
        public long GetInteger()
        {
            long result = 0;
            if (long.TryParse(data, out result))
                return result;

            return 0;
        }
        public float GetFloat()
        {
            if (!string.IsNullOrEmpty(data))
            {
                string ds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string str = data.Replace(",", ds).Replace(".", ds);

                float result = 0;
                if (float.TryParse(str, out result))
                    return result;
            }

            //return float.NaN;
            return 0;
        }
        #endregion
    }
}
