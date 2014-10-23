using MySensors.Core.Nodes;
using System;
using System.Text;

namespace MySensors.Core.Messaging
{
    public class Message
    {
        public byte NodeID { get; set; }
        public byte SensorID { get; set; }
        public MessageType MessageType { get; set; }
        public bool Ack { get; set; }
        public byte SubType { get; set; }
        public float Payload { get; set; }

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
                MessageType = (MessageType)byte.Parse(parts[2]),
                Ack = byte.Parse(parts[3]) == 1,
                SubType = byte.Parse(parts[4]),
                Payload = float.Parse(parts[5].Replace(".", ",")),
            };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("NodeID: \t\t{0:d3}", NodeID));
            sb.AppendLine(string.Format("SensorID: \t\t{0:d3}", SensorID));
            sb.AppendLine(string.Format("MessageType: \t\t{0}", MessageType));
            sb.AppendLine(string.Format("Ack: \t\t\t{0}", Ack));

            string propertyName = "SubType";
            object propertyValue = SubType;
            switch (MessageType)
            {
                case MessageType.Presentation:
                    propertyName = "SensorType";
                    propertyValue = (SensorType)SubType;
                    break;
                case MessageType.Set:
                case MessageType.Request:
                    propertyName = "ValueType";
                    propertyValue = (SensorValueType)SubType;
                    break;
                case MessageType.Internal:
                    propertyName = "InternalValueType";
                    propertyValue = (InternalValueType)SubType;
                    break;
                default:
                    propertyName = "SubType";
                    propertyValue = SubType;
                    break;
            }
            sb.AppendLine(string.Format("{0}: \t\t{1}", propertyName, propertyValue));

            sb.AppendLine(string.Format("Value: \t\t\t{0}", Payload));

            return sb.ToString();
        }
    }
}
