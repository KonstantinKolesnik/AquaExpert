using System;

namespace SmartHub.UWP.Plugins.Speech
{
    public class VoiceCommand
    {
        public virtual Guid Id { get; set; }
        public virtual string CommandText { get; set; }
        //public virtual UserScript UserScript { get; set; }
    }
}
