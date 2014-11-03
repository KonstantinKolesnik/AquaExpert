
namespace MySensors.Core
{
    public class Setting : ObservableObject
    {
        #region Fields
        private string name = "";
        private string value = "";
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
        #endregion

        #region Constructors
        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }
        #endregion
    }
}
