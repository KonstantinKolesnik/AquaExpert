using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace MySensors.Core.Scripting
{
    public class Script
    {
        private Assembly compiledAssembly = null;
        private Language language;
        private string source = null;
        private StringCollection references = null;
        private bool isCompiled = false;

        public Assembly CompiledAssembly
        {
            get { return (isCompiled ? compiledAssembly : null); }
            set { compiledAssembly = value; isCompiled = true; }
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
                if (!isCompiled)
                {
                    references.Clear();
                    references.AddRange(value);
                }
            }
        }
        public bool IsCompiled
        {
            get { return isCompiled; }
        }
        public string Source
        {
            get { return source; }
            set { if (!isCompiled) source = value; }
        }
        public Language Language
        {
            get { return language; }
            set { if (!isCompiled) language = value; }
        }

        public Script(Language language, string sourceCode)
        {
            references = new StringCollection();
            this.language = language;
            source = sourceCode;
        }
        public Script(Language language, Stream stream)
        {
            references = new StringCollection();
            this.language = language;
            source = new StreamReader(stream).ReadToEnd();
        }
        public Script(Assembly compiledAssembly)
        {
            this.compiledAssembly = compiledAssembly;
            isCompiled = true;
        }

        public void AddReference(string reference)
        {
            if (!isCompiled)
                references.Add(reference);
        }
        public void ClearReferences()
        {
            if (!isCompiled)
                references.Clear();
        }
    }
}
