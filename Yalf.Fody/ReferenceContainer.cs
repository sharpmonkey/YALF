using System;
using System.Linq;
using Mono.Cecil;

namespace Yalf.Fody
{
    public class ReferenceContainer
    {
        public readonly TypeReference LogType;
        public readonly TypeReference IContextType;
        public readonly TypeReference ExceptionType;
        public readonly MethodReference MethodContextMethod;
        public readonly MethodReference TraceExceptionMethod;
        public readonly MethodReference DisposeMethod;
        public readonly Func<TypeReference, MethodReference> CreateRecordReturnMethod;

        public ReferenceContainer(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver)
        {
            var systemDefinition = assemblyResolver.Resolve("mscorlib");
            var yalfDefinition = assemblyResolver.Resolve("Yalf");
            var yalfTypes = yalfDefinition.MainModule.Types;


            var logType = yalfTypes.Single(x => x.Name == "Log");
            var iContextType = yalfTypes.Single(x => x.Name == "IContext");

            var iDisposableType = systemDefinition.MainModule.Types.Single(x => x.Name == "IDisposable");
            var exceptionType = systemDefinition.MainModule.Types.Single(x => x.Name == "Exception");

            MethodContextMethod = moduleDefinition.Import(logType.Methods.Single(m => m.Name == "MethodContext"));
            TraceExceptionMethod = moduleDefinition.Import(logType.Methods.Single(m => m.Name == "TraceException"));
            CreateRecordReturnMethod = retType => {

                var recordReturn = moduleDefinition.Import(iContextType.Methods.Single(m => m.Name == "RecordReturn"));
                if (retType.IsGenericInstance)
                {
                    return recordReturn.MakeGeneric(retType);
                }
                else
                {
                    return recordReturn.MakeGeneric(retType);
                }
            };
            DisposeMethod = moduleDefinition.Import(iDisposableType.Methods.Single(m => m.Name == "Dispose"));
            LogType = moduleDefinition.Import(logType);
            IContextType = moduleDefinition.Import(iContextType);
            ExceptionType = moduleDefinition.Import(exceptionType);
        }
    }
}