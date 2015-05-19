using SmartHub.Plugins.Scripts.Data;
using System;

namespace SmartHub.Plugins.Speech.Data
{
    public class VoiceCommand
    {
        public virtual Guid Id { get; set; }

        public virtual string CommandText { get; set; }

        public virtual UserScript UserScript { get; set; }
    }
}
