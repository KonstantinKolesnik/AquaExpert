using System;
using System.Globalization;
using System.Text;

namespace SmartHub.Plugins.MySensors.Core
{
    public class SensorMessage
    {
        #region Properties
        public byte NodeNo { get; set; }
        public byte SensorNo { get; set; }
        public SensorMessageType Type { get; set; }
        public bool IsAckNeeded { get; set; }
        public byte SubType { get; set; }
        public string Payload { get; private set; }
        
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
        #endregion

        #region Constructor
        public SensorMessage(byte nodeID, byte sensorID, SensorMessageType type, bool isAckNeeded, byte subType, string payload)
        {
            NodeNo = nodeID;
            SensorNo = sensorID;
            Type = type;
            IsAckNeeded = isAckNeeded;
            SubType = subType;
            Payload = payload;
        }
        public SensorMessage(byte nodeID, byte sensorID, SensorMessageType type, bool isAckNeeded, byte subType, float payload)
        {
            NodeNo = nodeID;
            SensorNo = sensorID;
            Type = type;
            IsAckNeeded = isAckNeeded;
            SubType = subType;
            PayloadFloat = payload;
        }
        #endregion

        #region Public methods
        public static SensorMessage FromRawMessage(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string[] parts = str.Split(new char[] { ';' }, StringSplitOptions.None);
            if (parts.Length != 6)
                return null;

            //string rawPayload = parts[5].Trim();

            //var payload;
            //if ((MessageType)byte.Parse(parts[2]) == MessageType.Stream)
            //{
            //    payload = [];
            //    for (var i = 0; i < rawpayload.length; i+=2)
            //        payload.push(parseInt(rawpayload.substring(i, i + 2), 16));
            //}
            //else
            //    payload = rawpayload;

            SensorMessage msg = null;

            try
            {
                msg = new SensorMessage(
                    byte.Parse(parts[0]),
                    byte.Parse(parts[1]),
                    (SensorMessageType)byte.Parse(parts[2]),
                    byte.Parse(parts[3]) == 1,
                    byte.Parse(parts[4]),
                    parts[5]);
            }
            catch (Exception) { }

            return msg;
        }
        public string ToRawMessage()
        {
            //if (command == SensorMessageType.Stream)
            //{
            //    for (var i = 0; i < payload.length; i++)
            //    {
            //        if (payload[i] < 16)
            //            msg += "0";
            //        msg += payload[i].toString(16);
            //    }
            //}
            //else
            //{
            //    msg += payload;
            //}


            return string.Format("{0};{1};{2};{3};{4};{5}",
                NodeNo,
                SensorNo,
                (byte)Type,
                IsAckNeeded ? "1" : "0",
                (byte)SubType,
                Payload);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine(string.Format("Node ID: \t\t{0:d3}", NodeID));
            //sb.AppendLine(string.Format("Sensor ID: \t\t{0:d3}", SensorID));
            //sb.AppendLine(string.Format("Type: \t\t\t{0}", Type));
            ////sb.AppendLine(string.Format("Is ack needed: \t\t{0}", IsAckNeeded));

            //string propertyName = "Sub-type";
            //object propertyValue = SubType;
            //switch (Type)
            //{
            //    case SensorMessageType.Presentation:
            //        propertyName = "Sensor type";
            //        propertyValue = (SensorType)SubType;
            //        break;
            //    case SensorMessageType.Set:
            //    case SensorMessageType.Request:
            //        propertyName = "Value type";
            //        propertyValue = (SensorValueType)SubType;
            //        break;
            //    case SensorMessageType.Internal:
            //        propertyName = "Data type";
            //        propertyValue = (InternalValueType)SubType;
            //        break;
            //    case SensorMessageType.Stream:
            //        propertyName = "Stream data type";
            //        propertyValue = (StreamValueType)SubType;
            //        break;
            //    default:
            //        propertyName = "Sub-type";
            //        propertyValue = SubType;
            //        break;
            //}
            //sb.AppendLine(string.Format("{0}: \t\t{1}", propertyName, propertyValue));

            //sb.AppendLine(string.Format("Value: \t\t\t{0}", Payload));





            sb.Append(string.Format("[{0:d3}] ", NodeNo));
            sb.Append(string.Format("[{0:d3}] ", SensorNo));
            sb.Append(string.Format("[{0}] ", Type));
            //sb.Append(string.Format("[Ack: {0}] ", IsAckNeeded));
            switch (Type)
            {
                case SensorMessageType.Presentation:
                    sb.Append(string.Format("[{0}] ", (SensorType)SubType));
                    break;
                case SensorMessageType.Set:
                case SensorMessageType.Request:
                    sb.Append(string.Format("[{0}] ", (SensorValueType)SubType));
                    break;
                case SensorMessageType.Internal:
                    sb.Append(string.Format("[{0}] ", (InternalValueType)SubType));
                    break;
                case SensorMessageType.Stream:
                    sb.Append(string.Format("[{0}] ", (StreamValueType)SubType));
                    break;
                default:
                    sb.Append(string.Format("[{0}] ", SubType));
                    break;
            }
            sb.Append(string.Format("[{0}]", Payload));

            return sb.ToString();
        }
        #endregion
    }
}
