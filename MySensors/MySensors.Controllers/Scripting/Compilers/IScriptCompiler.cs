
namespace MySensors.Controllers.Scripting.Compilers
{
    public interface IScriptCompiler
    {
        Language Language { get; }

        void Compile(Script script, ScriptCompilerOutput output);
    }
}
