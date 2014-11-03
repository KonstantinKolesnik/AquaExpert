
namespace MySensors.Controllers.Scripting
{
    public interface IScriptCompiler
    {
        Language Language { get; }
        void Compile(Script script, ScriptCompilerOutput output);
    }
}
