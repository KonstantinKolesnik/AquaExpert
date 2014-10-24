using MySensors.Core.Nodes;
using System;
using System.Text;

namespace MySensors.Core.Messaging
{
    public class Message
    {
        public byte NodeID { get; set; }
        public byte SensorID { get; set; }
        public MessageType Type { get; set; }
        public bool IsAckNeeded { get; set; }
        public byte SubType { get; set; }
        public string Payload { get; set; }

        public static Message FromRawString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string[] parts = str.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 6)
                return null;

            return new Message() {
                NodeID = byte.Parse(parts[0]),
                SensorID = byte.Parse(parts[1]),
                Type = (MessageType)byte.Parse(parts[2]),
                IsAckNeeded = byte.Parse(parts[3]) == 1,
                SubType = byte.Parse(parts[4]),
                Payload = parts[5],
            };
        }
        public string ToRawString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}\n",
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
    }
}
