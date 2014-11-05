
namespace MySensors.Controllers.Scripting.Compilers
{
    interface IScriptCompiler
    {
        Language Language { get; }

        void Compile(Script script, ScriptCompilerOutput output);
    }
}
