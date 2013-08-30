using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Yalf.Fody;
using Mono.Cecil;

namespace Yalf.Tests.Helpers
{
    public class WeaverHelper
    {
        #region Public Methods and Operators

        public static dynamic CreateInstance<T>(Assembly assembly)
        {
            Type type = assembly.GetType(typeof(T).FullName);

            return Activator.CreateInstance(type);
        }

        public static dynamic CreateInstance<T>(Assembly assembly, object parameter)
        {
            Type type = assembly.GetType(typeof(T).FullName);

            return Activator.CreateInstance(type, parameter);
        }

        public static dynamic CreateInstance<T>(Assembly assembly, object[] parameters)
        {
            Type type = assembly.GetType(typeof(T).FullName);

            return Activator.CreateInstance(type, parameters);
        }

        public static Assembly WeaveAssembly(string assemblyPath = null)
        {
            if(string.IsNullOrEmpty(assemblyPath))
                assemblyPath = Path.GetFullPath("Yalf.TestAssembly.dll");

            //#if (!DEBUG)
            //            assemblyPath = assemblyPath.Replace("Debug", "Release");
            //#endif


            string newAssembly = assemblyPath.Replace(".dll", "2.dll");
            File.Copy(assemblyPath, newAssembly, true);

            var oldPdb = assemblyPath.Replace(".dll", ".pdb");
            var newPdb = assemblyPath.Replace(".dll", "2.pdb");
            File.Copy(oldPdb, newPdb, true);


            var readerParameters = new ReaderParameters
            {
                ReadSymbols = true,
            };

            ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(newAssembly, readerParameters);
            ModuleWeaver weavingTask = new ModuleWeaver { ModuleDefinition = moduleDefinition };
            weavingTask.AssemblyResolver = new MockAssemblyResolver();

            weavingTask.LogInfo = (message) => Debug.WriteLine(message);
            weavingTask.LogWarning = (message) => Debug.WriteLine(message);
            weavingTask.LogError = (message) => new Exception(message);

#if (DEBUG)
            weavingTask.DefineConstants.Add("DEBUG");
#endif

            weavingTask.Execute();

            var parameters = new WriterParameters 
            {
                WriteSymbols = true
            };
            moduleDefinition.Write(newAssembly, parameters);

            return Assembly.LoadFile(newAssembly);
        }

        #endregion
    }
}
