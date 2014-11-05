using Microsoft.VisualBasic;
using System.CodeDom.Compiler;

namespace MySensors.Controllers.Scripting.Compilers
{
    class VBCompiler : IScriptCompiler
    {
        public Language Language
        {
            get { return Language.VisualBasic; }
        }

        public void Compile(Script script, ScriptCompilerOutput output)
        {
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = false, //Генерировать библиотеку
                GenerateInMemory = true,    //Создать её в памяти
                IncludeDebugInformation = false, //Создать debug информацию
                TreatWarningsAsErrors = false   //Не принимать сообшения как ошибки
            };
            parameters.ReferencedAssemblies.AddRange(script.ReferencedAssemblies); //Добавить информацию о ссылках

            CompilerResults result = new VBCodeProvider().CompileAssemblyFromSource(parameters, script.Source); //Компилировать
            if (result.Errors.HasErrors) //Если есть ошибки, перечислить их и выйти ...
            {
                if (output != null)
                    for (int i = 0; i < result.Errors.Count; i++)
                        output(result.Errors[i].ToString());
            }
            else //... а если их нет - выйти
                script.CompiledAssembly = result.CompiledAssembly;
        }
    }
}
