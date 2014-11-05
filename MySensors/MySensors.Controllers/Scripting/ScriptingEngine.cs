using MySensors.Controllers.Scripting.Compilers;
using System;
using System.Reflection;

namespace MySensors.Controllers.Scripting
{
    class ScriptingEngine
    {
        private IScriptCompiler compiler = null;

        public ScriptingEngine(IScriptCompiler compiler)
        {
            this.compiler = compiler;
        }

        public void Compile(Script script, ScriptCompilerOutput output)
        {
            if (script.Language != compiler.Language)
                throw new Exception("Different language!");

            compiler.Compile(script, output);
        }
        public object Execute(Script script, string typeName, string methodName, params object[] args)
        {
            if (!script.IsCompiled)
                throw new Exception("Script is not compiled!");

            object obj = CreateObject(script, typeName);

            Type type = script.CompiledAssembly.GetType(typeName);
            MethodInfo method = type.GetMethod(methodName);

            return method.Invoke(obj, args);//new object[] { 100 });
            //return method.Invoke(null, args);// for static methods
        }
        public object CreateObject(Script script, string typeName)
        {
            if (!script.IsCompiled)
                throw new Exception("Script is not compiled!");

            Type type = script.CompiledAssembly.GetType(typeName);

            if (type != null)
            {
                ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
                return ctor.Invoke(new object[] { });
            }
            else
                return null;
        }
    }
}
