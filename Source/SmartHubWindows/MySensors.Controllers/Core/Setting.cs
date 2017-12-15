
namespace MySensors.Controllers.Core
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
            internal set
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
            internal set
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
        internal Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }
        #endregion
    }
}
