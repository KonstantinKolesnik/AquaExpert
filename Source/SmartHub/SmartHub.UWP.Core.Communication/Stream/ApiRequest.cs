namespace SmartHub.UWP.Core.Communication.Stream
{
    public class ApiRequest
    {
        public string CommandName
        {
            get; set;
        }
        public object[] Parameters
        {
            get; set;
        }

        public ApiRequest()
        {
        }
        public ApiRequest(string commandName, object[] parameters)
        {
            CommandName = commandName;
            Parameters = parameters;
        }
    }
}
