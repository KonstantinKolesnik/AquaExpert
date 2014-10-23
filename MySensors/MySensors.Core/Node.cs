
namespace MySensors.Core
{
    public class Node
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public bool IsAckNeeded { get; private set; }

        public Node(string name, string version, bool isAckNeeded)
        {
            Name = name;
            Version = version;
            IsAckNeeded = isAckNeeded;
        }
    }
}
