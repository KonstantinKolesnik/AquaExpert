using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace MySensors.Controllers.Scripting
{
    class Script
    {
        #region Fields
        private string source = null;
        private StringCollection references = null;
        
        private Assembly compiledAssembly = null;
        #endregion

        #region Properties
        public Language Language
        {
            get;
            set;
        }
        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                compiledAssembly = null;
            }
        }
        public string[] ReferencedAssemblies
        {
            get
            {
                string[] refs = new string[references.Count];
                references.CopyTo(refs, 0);
                return refs;
            }
            set
            {
                references.Clear();
                references.AddRange(value);
            }
        }

        public Assembly CompiledAssembly
        {
            get { return compiledAssembly; }
            set
            {
                compiledAssembly = value;
                source = null;
            }
        }

        public bool IsCompiled
        {
            get { return compiledAssembly != null; }
        }
        #endregion

        #region Constructors
        public Script(Language language, string sourceCode)
        {
            Language = language;
            source = sourceCode;
            references = new StringCollection();
        }
        public Script(Language language, Stream stream)
            : this(language, new StreamReader(stream).ReadToEnd())
        {
        }

        public Script(Assembly compiledAssembly)
        {
            this.compiledAssembly = compiledAssembly;
        }
        #endregion

        #region Public methods
        public void AddReference(string reference)
        {
            references.Add(reference);
        }
        public void ClearReferences()
        {
            references.Clear();
        }

        public CompilerResults Compile()
        {
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                CompilerOptions = "/optimize",
                TreatWarningsAsErrors = false
            };
            parameters.ReferencedAssemblies.AddRange(ReferencedAssemblies);

            CodeDomProvider cdp = null;
            switch (Language)
            {
                case Language.CSharp: cdp = new CSharpCodeProvider(); break;
                case Language.VisualBasic: cdp = new VBCodeProvider(); break;
            }
            if (cdp == null)
                throw new Exception("Unsupported script language!");

            compiledAssembly = null;
            
            CompilerResults result = cdp.CompileAssemblyFromSource(parameters, source);
            if (!result.Errors.HasErrors)
                compiledAssembly = result.CompiledAssembly;

            return result;
        }
        public object CreateObject(string typeName)
        {
            if (!IsCompiled)
                throw new Exception("Script is not compiled!");

            Type type = compiledAssembly.GetType(typeName);

            if (type != null)
            {
                ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
                return ctor.Invoke(new object[] { });
            }
            else
                return null;
        }
        public object Execute(string typeName, string methodName, params object[] args)
        {
            if (!IsCompiled)
                throw new Exception("Script is not compiled!");

            object obj = CreateObject(typeName);
            return compiledAssembly.GetType(typeName).GetMethod(methodName).Invoke(obj, args);
        }
        public object ExecuteStatic(string typeName, string methodName, params object[] args)
        {
            if (!IsCompiled)
                throw new Exception("Script is not compiled!");

            return compiledAssembly.GetType(typeName).GetMethod(methodName).Invoke(null, args);
        }
        #endregion
    }
}
