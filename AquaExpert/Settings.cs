using MFE.Core;
using MFE.Storage;
using System.Ext.Xml;
using System.IO;
using System.Text;
using System.Xml;

namespace AquaExpert
{
    class Settings
    {
        #region Constants
        public const int IPPort = 7777;
        public const int WSPort = 12000;
        private const string fileName = @"\AquaExpert\options.xml";
        #endregion

        #region Settings
        public bool UseWiFi = true;
        public string WiFiSSID = "GothicMaestro";
        public string WiFiPassword = "kotyara75";
        //public string WiFiSSID = "TW";
        //public string WiFiPassword = "techwiregreyc";

        public int LightOnHour = 10;
        public int LightOffHour = 20;

        #endregion

        #region Serialization / deserialization
        public static Settings FromXml(string xml)
        {
            return !Utils.StringIsNullOrEmpty(xml) ? Settings.FromByteArray(Encoding.UTF8.GetBytes(xml)) : null;
        }
        public static Settings FromByteArray(byte[] data)
        {
            Settings res = null;

            if (data != null)
            {
                using (MemoryStream xmlStream = new MemoryStream(data))
                {
                    XmlReaderSettings ss = new XmlReaderSettings();
                    ss.IgnoreWhitespace = true;
                    ss.IgnoreComments = true;
                    using (XmlReader reader = XmlReader.Create(xmlStream, ss))
                    {
                        while (!reader.EOF)
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Options")
                            {
                                res = new Settings();

                                if (!Utils.StringIsNullOrEmpty(reader.GetAttribute("UseWiFi")))
                                    res.UseWiFi = reader.GetAttribute("UseWiFi") == bool.TrueString;
                                if (!Utils.StringIsNullOrEmpty(reader.GetAttribute("WiFiSSID")))
                                    res.WiFiSSID = reader.GetAttribute("WiFiSSID");
                                if (!Utils.StringIsNullOrEmpty(reader.GetAttribute("WiFiPassword")))
                                    res.WiFiPassword = reader.GetAttribute("WiFiPassword");

                                //if (!Utils.StringIsNullOrEmpty(reader.GetAttribute("MainBridgeCurrentThreshould")))
                                //    res.MainBridgeCurrentThreshould = int.Parse(reader.GetAttribute("MainBridgeCurrentThreshould"));
                                //if (!Utils.StringIsNullOrEmpty(reader.GetAttribute("ProgBridgeCurrentThreshould")))
                                //    res.ProgBridgeCurrentThreshould = int.Parse(reader.GetAttribute("ProgBridgeCurrentThreshould"));
                                //if (!Utils.StringIsNullOrEmpty(reader.GetAttribute("BroadcastBoostersCurrent")))
                                //    res.BroadcastBoostersCurrent = reader.GetAttribute("BroadcastBoostersCurrent") == bool.TrueString;
                            }
                        }
                    }
                }
            }

            return res;
        }
        public string ToXml()
        {
            byte[] res = ToByteArray();
            return res != null ? new string(Encoding.UTF8.GetChars(res)) : null;
        }
        public byte[] ToByteArray()
        {
            byte[] res = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(ms))
                {
                    //<?xml version="1.0" encoding="utf-8" ?>
                    //writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");

                    writer.WriteStartElement("Options");

                    writer.WriteAttributeString("UseWiFi", UseWiFi ? bool.TrueString : bool.FalseString);
                    writer.WriteAttributeString("WiFiSSID", WiFiSSID);
                    writer.WriteAttributeString("WiFiPassword", WiFiPassword);

                    //writer.WriteAttributeString("MainBridgeCurrentThreshould", MainBridgeCurrentThreshould.ToString());
                    //writer.WriteAttributeString("ProgBridgeCurrentThreshould", ProgBridgeCurrentThreshould.ToString());
                    //writer.WriteAttributeString("BroadcastBoostersCurrent", BroadcastBoostersCurrent ? bool.TrueString : bool.FalseString);

                    writer.WriteEndElement();
                    writer.Flush();
                    //writer.Close();

                    res = ms.ToArray();
                }
            }

            return res;
        }
        #endregion

        #region Load / save
        public static Settings LoadFromFlash(uint id)
        {
            Settings res = new Settings();
            object data = FlashManager.LoadFromFlash(typeof(string), 0);
            if (data != null)
                res = Settings.FromXml((string)data);

            return res;
        }
        public void SaveToFlash()
        {
            FlashManager.SaveToFlash(ToXml());
        }

        public static Settings LoadFromSD(string root)
        {
            Settings res = new Settings();

            byte[] data = DriveManager.LoadFromSD(root + fileName);
            if (data == null)
                res.SaveToSD(root);
            else
                res = Settings.FromByteArray(data);

            return res;
        }
        public void SaveToSD(string root)
        {
            DriveManager.SaveToSD(ToByteArray(), root + fileName);
        }
        #endregion
    }
}
