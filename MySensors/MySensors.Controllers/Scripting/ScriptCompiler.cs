using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySensors.Controllers.Scripting
{
    class ScriptCompiler
    {
        public void Compile(Script script, ScriptCompilerOutput output)
        {
            if (script == null)
                throw new Exception("Script is null!");

            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = false, //Генерировать библиотеку
                GenerateInMemory = true,    //Создать её в памяти
                IncludeDebugInformation = false, //Создать debug информацию
                CompilerOptions = "/optimize",
                TreatWarningsAsErrors = false   //Не принимать предупреждения как ошибки
            };
            parameters.ReferencedAssemblies.AddRange(script.ReferencedAssemblies); //Добавить информацию о ссылках

            CodeDomProvider cdp = null;
            switch (script.Language)
            {
                case Language.CSharp: cdp = new CSharpCodeProvider(); break;
                case Language.VisualBasic: cdp = new VBCodeProvider(); break;
            }

            if (cdp == null)
                throw new Exception("Unsupported script language!");

            CompilerResults result = cdp.CompileAssemblyFromSource(parameters, script.Source); //Компилировать

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
