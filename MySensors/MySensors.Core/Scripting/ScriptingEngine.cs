using System;

namespace MySensors.Core.Scripting
{
    public class ScriptingEngine
    {
        private IScriptCompiler compiler = null;

        public ScriptingEngine(IScriptCompiler comp)
        {
            compiler = comp;
        }

        public void Compile(Script script, ScriptCompilerOutput output)
        {
            if (script.Language != compiler.Language)
                throw new Exception("Different language!");

            compiler.Compile(script, output);
        }
        public object Execute(Script script, string type, string entrypoint, params object[] args)
        {
            if (!script.IsCompiled)
                throw new Exception("Script is not compiled!");

            return script.CompiledAssembly.GetType(type).GetMethod(entrypoint).Invoke(null, args);
        }
    }
}
