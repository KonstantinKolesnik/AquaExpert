using MySensors.Controllers.Core;
using MySensors.Controllers.Scripting;
using MySensors.Controllers.Scripting.Compilers;
using System;

namespace MySensors.Controllers.Automation
{
    class AutomationModule : ObservableObject
    {
        #region Fields
        private Guid id;
        private string name = "";
        private string description = "";
        private string script = null;
        private IAutomationService service = null;
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

        public string RunService()
        {
            //var asm = Assembly.LoadFile(@"C:\myDll.dll");
            //var type = asm.GetType("TestRunner");
            //var runnable = Activator.CreateInstance(type) as IRunnable;
            //if (runnable == null) throw new Exception("broke");
            //runnable.Run();

            //var domain = AppDomain.CreateDomain("NewDomainName");
            //var pathToDll = @"C:\myDll.dll";
            //var t = typeof(TypeIWantToLoad);
            //var runnable = domain.CreateInstanceFromAndUnwrap(pathToDll, t.FullName) as IRunnable;
            //if (runnable == null) throw new Exception("broke");
            //runnable.Run();



            service = null;

            if (!string.IsNullOrEmpty(Script))
            {
                MySensors.Controllers.Scripting.Script script = new MySensors.Controllers.Scripting.Script(Language.CSharp, Script);
                script.AddReference("System.dll");
                script.AddReference("MySensors.Controllers.dll");

                ScriptingEngine scriptEngine = new ScriptingEngine(new CSharpCompiler());
                scriptEngine.Compile(script, null);
                if (script.IsCompiled)
                {
                    service = scriptEngine.CreateObject(script, "AutomationService") as IAutomationService;
                    if (service != null)
                    {
                        int a = service.Test();

                        return null;
                    }
                    else
                        return "Error getting service of Automation module " + Name;
                }
                else
                    return "Error compiling script of Automation module " + Name;
            }
            else
                return null;
        }
    }
}
