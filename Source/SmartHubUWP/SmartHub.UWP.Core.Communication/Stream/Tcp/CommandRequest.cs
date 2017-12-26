namespace SmartHub.UWP.Core.Communication.Stream.Tcp
{
    class CommandRequest
    {
        public string Name
        {
            get; set;
        }
        public object[] Parameters
        {
            get; set;
        }

        public CommandRequest(string name, object[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}
