using MySensors.Controllers.Core;
using MySensors.Controllers.Scripting;
using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace MySensors.Controllers.Automation
{
    class AutomationModule : ObservableObject
    {
        #region Fields
        private Guid id;
        private string name = "";
        private string description = "";
        private string script = "";
        private string view = "";
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
            internal set
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
            internal set
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
            internal set
            {
                if (script != value)
                {
                    script = value;
                    NotifyPropertyChanged("Script");
                }
            }
        }
        public string View
        {
            get { return view; }
            internal set
            {
                if (view != value)
                {
                    view = value;
                    NotifyPropertyChanged("View");
                }
            }
        }
        #endregion

        #region Constructors
        public AutomationModule(string name, string description, string script, string view)
            : this(Guid.NewGuid(), name, description, script, view)
        {
        }
        public AutomationModule(Guid id, string name, string description, string script, string view)
        {
            ID = id;
            Name = name;
            Description = description;
            Script = script;
            View = view;
        }
        #endregion

        #region Public methods
        public string StartService(Controller controller)
        {
            #region Info
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
            #endregion

            if (service != null)
                service.Stop();

            service = null;

            if (!string.IsNullOrEmpty(Script))
            {
                MySensors.Controllers.Scripting.Script script = new MySensors.Controllers.Scripting.Script(Language.CSharp, Script);
                script.AddReference("System.dll");
                script.AddReference("MySensors.Controllers.dll");

                CompilerResults cr = script.Compile();
                if (cr.Errors.HasErrors)
                {
                    string res = "Error compiling script of Automation module \"" + Name + "\"\n";
                    for (int i = 0; i < cr.Errors.Count; i++)
                        res += cr.Errors[i].ToString() + "\n";

                    return res;
                }

                if (script.IsCompiled)
                {
                    var serviceType = script.CompiledAssembly.ExportedTypes.FirstOrDefault();
                    if (serviceType == null || !serviceType.IsClass || !serviceType.IsPublic)
                        return "No service found in Automation module \"" + Name + "\"";

                    service = script.CreateObject(serviceType.FullName) as IAutomationService;
                    if (service == null)
                        return "Error getting service of Automation module \"" + Name + "\"";

                    try
                    {
                        service.Start(controller);
                    }
                    catch (Exception ex)
                    {
                        string res = "Error starting service of Automation module \"" + Name + "\"\n";
                        res += ex.Message + "\n" + ex.StackTrace;
                        return res;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
