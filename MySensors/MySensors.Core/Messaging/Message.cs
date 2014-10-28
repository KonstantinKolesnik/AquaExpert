using MySensors.Core.Nodes;
using System;
using System.Text;

namespace MySensors.Core.Messaging
{
    public class Message
    {
        #region Properties
        public byte NodeID { get; set; }
        public byte SensorID { get; set; }
        public MessageType Type { get; set; }
        public bool IsAckNeeded { get; set; }
        public byte SubType { get; set; }
        public string Payload { get; set; }
        #endregion

        #region Constructor
        public Message(byte nodeID, byte sensorID, MessageType type, bool isAckNeeded, byte subType, string payload)
        {
            NodeID = nodeID;
            SensorID = sensorID;
            Type = type;
            IsAckNeeded = isAckNeeded;
            SubType = subType;
            Payload = payload;
        }
        #endregion

        #region Public methods
        public static Message FromRawString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string[] parts = str.Split(new char[] {';'}, StringSplitOptions.None);
            if (parts.Length != 6)
                return null;
            
            string rawPayload = parts[5].Trim();

            //var payload;
            //if ((MessageType)byte.Parse(parts[2]) == MessageType.Stream)
            //{
            //    payload = [];
            //    for (var i = 0; i < rawpayload.length; i+=2)
            //        payload.push(parseInt(rawpayload.substring(i, i + 2), 16));
            //}
            //else
            //    payload = rawpayload;

            Message msg = null;

            try
            {
                msg = new Message(
                    byte.Parse(parts[0]),
                    byte.Parse(parts[1]),
                    (MessageType)byte.Parse(parts[2]),
                    byte.Parse(parts[3]) == 1,
                    byte.Parse(parts[4]),
                    parts[5]);
            }
            catch (Exception) { }

            return msg;
        }
        public string ToRawString()
        {
            //if (command == 4)
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
                NodeID,
                SensorID,
                (byte)Type,
                IsAckNeeded ? "1" : "0",
                (byte)SubType,
                Payload);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("Node ID: \t\t{0:d3}", NodeID));
            sb.AppendLine(string.Format("Sensor ID: \t\t{0:d3}", SensorID));
            sb.AppendLine(string.Format("Type: \t\t\t{0}", Type));
            //sb.AppendLine(string.Format("Is ack needed: \t\t{0}", IsAckNeeded));

            string propertyName = "Sub-type";
            object propertyValue = SubType;
            switch (Type)
            {
                case MessageType.Presentation:
                    propertyName = "Sensor type";
                    propertyValue = (SensorType)SubType;
                    break;
                case MessageType.Set:
                case MessageType.Request:
                    propertyName = "Value type";
                    propertyValue = (SensorValueType)SubType;
                    break;
                case MessageType.Internal:
                    propertyName = "Data type";
                    propertyValue = (InternalValueType)SubType;
                    break;
                default:
                    propertyName = "Sub-type";
                    propertyValue = SubType;
                    break;
            }
            sb.AppendLine(string.Format("{0}: \t\t{1}", propertyName, propertyValue));

            sb.AppendLine(string.Format("Value: \t\t\t{0}", Payload));

            return sb.ToString();
        }
        #endregion
    }
}
