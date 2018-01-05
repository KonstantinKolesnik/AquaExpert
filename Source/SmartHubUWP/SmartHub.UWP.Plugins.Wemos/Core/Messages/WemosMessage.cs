using SmartHub.UWP.Plugins.Things.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartHub.UWP.Plugins.Wemos.Core.Messages
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
            return $"{NodeID};{LineID};{(int)Type};{SubType};{data ?? string.Empty}\n";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("[{0:d}] ", NodeID));
            sb.Append(string.Format("[{0:d}] ", LineID));
            sb.Append(string.Format("[{0}] ", Type));
            switch (Type)
            {
                case WemosMessageType.Presentation: sb.AppendFormat("[{0}] ", (LineType) SubType); break;
                case WemosMessageType.Report:
                case WemosMessageType.Set:
                case WemosMessageType.Get: sb.AppendFormat("[{0}] ", (LineType) SubType); break;
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
            if (bool.TryParse(data, out bool result))
                return result;

            return false;
        }
        public long GetInteger()
        {
            if (long.TryParse(data, out long result))
                return result;

            return 0;
        }
        public float GetFloat()
        {
            if (!string.IsNullOrEmpty(data))
            {
                string ds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string str = data.Replace(",", ds).Replace(".", ds);

                if (float.TryParse(str, out float result))
                    return result;
            }

            //return float.NaN;
            return 0;
        }
        #endregion
    }
}
