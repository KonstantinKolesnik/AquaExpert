using MySensors.Core;
using System;

namespace MySensors.Controllers.Automation
{
    public class AutomationModule : ObservableObject
    {
        #region Fields
        private Guid id;
        private string name = "";
        private string description = "";
        private string script = null;
        #endregion

        #region Properties
        public Guid ID
        {
            get { return id; }
            private set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
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
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }
        public string Script
        {
            get { return script; }
            set
            {
                if (script != value)
                {
                    script = value;
                    NotifyPropertyChanged("Script");
                }
            }
        }
        #endregion

        #region Constructors
        public AutomationModule(string name, string description, string script)
            : this(Guid.NewGuid(), name, description, script)
        {
        }
        public AutomationModule(Guid id, string name, string description, string script)
        {
            ID = id;
            Name = name;
            Description = description;
            Script = script;
        }
        #endregion
    }
}
