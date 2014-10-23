using System;
using System.Text;

namespace MySensors.Controller.Messaging
{
    public class Message
    {
        public byte NodeID { get; set; }
        public byte SensorID { get; set; }
        public MessageType MessageType { get; set; }
        public bool Ack { get; set; }
        public byte SubType { get; set; }
        public float Payload { get; set; }

        static public Message FromString(string str)
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

            sb.AppendLine(string.Format("NodeID: \t{0:d3}", NodeID));
            sb.AppendLine(string.Format("SensorID: \t{0:d3}", SensorID));
            sb.AppendLine(string.Format("MessageType: \t{0}", MessageType));
            sb.AppendLine(string.Format("Ack: \t\t{0}", Ack));
            sb.AppendLine(string.Format("SubType: \t{0}", SubType));
            sb.AppendLine(string.Format("Payload: \t{0}", Payload));

            return sb.ToString();
        }
    }
}
