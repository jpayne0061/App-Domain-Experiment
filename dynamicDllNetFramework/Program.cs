using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading.Tasks;

namespace dynamicDllNetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = System.IO.File.ReadAllText(args[0]);

            CreateDllFromText(code);

            //results.Output => will return string collection of output

            AppDomain domain = AppDomain.CreateDomain("New domain name");


            string pathToDll = @"C:\Users\Evan\source\repos\dynamicDllNetFramework\dynamicDllNetFramework\bin\Debug\AutoGen.dll";

            Type type = typeof(Proxy);

            Proxy value = (Proxy)domain.CreateInstanceAndUnwrap(
            type.Assembly.FullName,
            type.FullName);

            var task = Task.Run(() =>
            {
                value.ExecuteMethod(pathToDll, "Main");
            });

            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(3000));
        }

        public static void CreateDllFromText(string code)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = "AutoGen.dll";
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
        }
    }



    public class Proxy : MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath)
        {
            try
            {
                return Assembly.LoadFile(assemblyPath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Execute(string pathToDll)
        {
            Assembly assembly = GetAssembly(pathToDll);
            object obj = assembly.CreateInstance("dynamicCode.Program");

            MethodInfo mi = obj.GetType().GetMethod("Main");

            mi.Invoke(obj, new object[] { new string[] { } });
        }

        public void ExecuteMethod(string pathToDll, string methodName)
        {
            Assembly assembly = GetAssembly(pathToDll);
            object obj = assembly.CreateInstance("dynamicCode.Program");

            MethodInfo mi = obj.GetType().GetMethod(methodName);

            mi.Invoke(obj, new object[] { new string[] { } });
        }

    }
}
