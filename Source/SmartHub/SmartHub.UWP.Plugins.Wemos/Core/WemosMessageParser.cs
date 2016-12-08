using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SmartHub.UWP.Plugins.Wemos.Core
{
    static class WemosMessageParser
    {
        #region Fields
        private static string buffer = "";
        #endregion

        #region Public methods
        public static List<WemosMessage> Parse(string data)
        {
            List<WemosMessage> result = new List<WemosMessage>();

            if (!string.IsNullOrEmpty(data))
            {
                buffer += data;
                result.AddRange(ParseBuffer());
            }

            return result;
        }
        #endregion

        #region Private methods
        private static List<WemosMessage> ParseBuffer()
        {
            List<WemosMessage> result = new List<WemosMessage>();

            //string pattern = @"(?<v0>[\s\S]*?);(?<v1>[\s\S]*?);(?<v2>[\s\S]*?);(?<v3>[\s\S]*?);(?<v4>[\s\S]*?)\n";
            string pattern = @"(?<v0>[\s\S]*);(?<v1>[\s\S]*);(?<v2>[\s\S]*);(?<v3>[\s\S]*);(?<v4>[\s\S]*)\n";

            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection entries = r.Matches(buffer);

            if (entries.Count > 0)
            {
                buffer = r.Replace(buffer, "");

                foreach (Match entry in entries)
                {
                    WemosMessage msg = null;
                    try
                    {
                        msg = new WemosMessage(
                            int.Parse(entry.Groups["v0"].Value),
                            int.Parse(entry.Groups["v1"].Value),
                            (WemosMessageType) int.Parse(entry.Groups["v2"].Value),
                            int.Parse(entry.Groups["v3"].Value),
                            entry.Groups["v4"].Value.Trim());
                    }
                    catch (Exception) { }

                    if (msg != null)
                        result.Add(msg);
                }
            }

            return result;
        }
        #endregion
    }
}
