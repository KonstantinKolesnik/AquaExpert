
namespace MySensors.Core.Scripting
{
    public interface IScriptCompiler
    {
        Language Language { get; }
        void Compile(Script script, ScriptCompilerOutput output);
    }
}
