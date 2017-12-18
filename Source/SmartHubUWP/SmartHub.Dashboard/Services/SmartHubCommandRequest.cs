namespace SmartHub.Dashboard.Services
{
    public class SmartHubCommandRequest
    {
        public string Name
        {
            get; set;
        }
        public object[] Parameters
        {
            get; set;
        }

        public SmartHubCommandRequest(string name, object[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}
