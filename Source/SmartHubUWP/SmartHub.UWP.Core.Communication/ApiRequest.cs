namespace SmartHub.UWP.Core.Communication
{
    public class ApiRequest
    {
        public string Name
        {
            get; set;
        }
        public object[] Parameters
        {
            get; set;
        }

        public ApiRequest(string name, object[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}
