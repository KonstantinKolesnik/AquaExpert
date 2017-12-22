using SmartHub.UWP.Plugins.Scripts.Models;
using System;

namespace SmartHub.UWP.Plugins.Speech
{
    public class VoiceCommand
    {
        public Guid ID
        {
            get; set;
        }
        public string CommandText
        {
            get; set;
        }
        public UserScript UserScript
        {
            get; set;
        }
    }
}
