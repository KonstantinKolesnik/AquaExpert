using System;
using System.Collections.Generic;

namespace MySensors.Controllers.Communication
{
    public class NetworkMessage
    {
        #region Fields
        private string id;
        private List<string> parameters = new List<string>();
        #endregion

        #region Properties
        public string ID
        {
            get { return id; }
        }
        public List<string> Parameters
        {
            get { return parameters; }
        }
        public string this[int idx]
        {
            get
            {
                return idx < parameters.Count ? parameters[idx] : null;
            }
        }
        #endregion

        #region Constructor
        public NetworkMessage(string id, params string[] parameters)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("NetworkMessage ID not specified");

            this.id = id;
            this.parameters.AddRange(parameters);
        }
        #endregion

        #region Public Methods
        public static NetworkMessage FromString(string str)
        {
            string[] parts = str.Split(new Char[] { ';' });
            if (parts.Length < 1)
                return null;

            List<string> pp = new List<string>(parts);
            pp.RemoveAt(0);

            return new NetworkMessage(parts[0], pp.ToArray());
        }
        public string PackToString()
        {
            string pp = string.Join(";", parameters.ToArray());
            return string.Join(";", id, pp) + "\n";
        }
        #endregion
    }
}
