using System;
using System.Reflection;
using Mono.Cecil;

namespace Yalf.Tests.Helpers
{
    public class MockAssemblyResolver : IAssemblyResolver
    {
        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            throw new NotImplementedException();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            throw new NotImplementedException();
        }

        public AssemblyDefinition Resolve(string fullName)
        {
            if (fullName.StartsWith("mscorlib"))
            {
                var codeBase = typeof(string).Assembly.CodeBase.Replace("file:///", "");
                return AssemblyDefinition.ReadAssembly(codeBase);
            }
            else
            {

                var codeBase = Assembly.GetAssembly(typeof(Yalf.Log)).CodeBase.Replace("file:///", "");
                return AssemblyDefinition.ReadAssembly(codeBase);
            }

        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
