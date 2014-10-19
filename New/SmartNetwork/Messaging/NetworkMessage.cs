using System;
using System.Collections.Generic;
using System.Text;

namespace SmartNetwork.Core.Messaging
{
    public class NetworkMessage
    {
        #region Fields
        private string id;
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        #endregion

        #region Properties
        public string ID
        {
            get { return id; }
        }
        public string this[string parameterName]
        {
            get
            {
                return parameters.ContainsKey(parameterName) ? parameters[parameterName] : null;
            }
            set
            {
                if (parameters.ContainsKey(parameterName))
                    parameters[parameterName] = value;
                else
                    parameters.Add(parameterName, value);
            }
        }
        public int ParametersCount
        {
            get { return parameters.Count; }
        }
        #endregion

        #region Constructor
        public NetworkMessage(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("NetworkMessage ID not specified");

            this.id = id;
        }
        #endregion

        #region Public Methods
        public static NetworkMessage FromString(string str)
        {
            return FromText(str);
        }
        public byte[] Pack()
        {
            return Encoding.UTF8.GetBytes(PackToString());
        }
        public string PackToString()
        {
            return NetworkMessageDelimiters.BOM + ToText() + NetworkMessageDelimiters.EOM;
        }
        #endregion

        #region Private methods
        private string ToText()
        {
            string res = "ID=" + id + ";";
            foreach (string key in parameters.Keys)
                res += key + "=" + (string)parameters[key] + ";";

            return res;
        }
        private static NetworkMessage FromText(string txt)
        {
            NetworkMessage msg = null;

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            try
            {
                string[] pairs = txt.Split(new Char[] { ';' });
                foreach (string pair in pairs)
                {
                    if (!string.IsNullOrEmpty(pair))
                    {
                        string[] s = pair.Split(new Char[] { '=' });
                        parameters.Add(s[0], s[1]);
                    }
                }

                if (parameters.ContainsKey("ID"))
                {
                    msg = new NetworkMessage((string)parameters["ID"]);
                    foreach (string key in parameters.Keys)
                        if (key != "ID")
                            msg[key] = (string)parameters[key];
                }
            }
            catch { }

            return msg;
        }
        #endregion
    }
}
