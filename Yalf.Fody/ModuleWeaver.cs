using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Yalf.Fody
{
    public class ModuleWeaver
    {
        #region Constants

        public const string NoLogAttributeName = "NoLogAttribute";

        #endregion

        #region Constructors and Destructors

        public ModuleWeaver()
        {
            LogInfo = m => { };
            LogWarning = m => { };
            LogWarningPoint = (m, p) => { };
            LogError = m => { };
            LogErrorPoint = (m, p) => { };

            DefineConstants = new List<string>();
        }

        #endregion

        #region Public Properties

        public IAssemblyResolver AssemblyResolver { get; set; }

        public List<string> DefineConstants { get; set; }

        public Action<string> LogError { get; set; }

        public Action<string, SequencePoint> LogErrorPoint { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public Action<string, SequencePoint> LogWarningPoint { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        #endregion

        #region Properties

        private bool IsDebugBuild { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Execute()
        {
            IsDebugBuild = DefineConstants.Any(x => x.ToLower() == "debug");

            var references = new ReferenceContainer(ModuleDefinition, AssemblyResolver);

            WeaveMethods(references);
        }

        #endregion

        #region Methods

        private IEnumerable<MethodDefinition> SelectMethods(ModuleDefinition moduleDefinition, string excludeAttributeName)
        {
            var noLogAssembly = moduleDefinition.ContainsAttribute(excludeAttributeName);

            if (noLogAssembly)
            {
                LogInfo(string.Format("Skipping '{0}' assembly due to {1} attribute", moduleDefinition.Name, excludeAttributeName));
                return Enumerable.Empty<MethodDefinition>();
            }

            LogInfo(string.Format("Searching for Methods in assembly ({0}).", moduleDefinition.Name));

            var definitions = new HashSet<MethodDefinition>(
                moduleDefinition
                .Types
                .Where(x => x.IsClass && !x.ContainsAttribute(excludeAttributeName))
                .SelectMany(x => x.Methods)
                .Where(x => ValidMethod(moduleDefinition, excludeAttributeName, x))
                );

            // Remove NoLogAttribute
            moduleDefinition.Types.SelectMany(x => x.Methods).ToList().ForEach(x => x.RemoveAttribute(excludeAttributeName));
            moduleDefinition.Types.ToList().ForEach(x => x.RemoveAttribute(excludeAttributeName));
            moduleDefinition.Assembly.RemoveAttribute(excludeAttributeName);

            return definitions;
        }

        private static bool ValidMethod(ModuleDefinition moduleDefinition, string excludeAttributeName, MethodDefinition x)
        {
            return !x.ContainsAttribute(excludeAttributeName)
                   &&
                   x.HasBody
                   &&
                   (
                       (
                            !x.IsSpecialName 
                            || 
                            x.IsConstructor
                       )
                       &&
                       !x.IsGetter
                       &&
                       !x.IsSetter
                       &&
                       !x.ContainsAttribute(moduleDefinition.ImportType<CompilerGeneratedAttribute>())
                   );

        }

        private void WeaveMethod(MethodDefinition methodDefinition, ReferenceContainer references)
        {
            var asyncAttribute = methodDefinition.CustomAttributes.FirstOrDefault(_ => _.AttributeType.Name == "AsyncStateMachineAttribute");
            if (asyncAttribute == null)
            {
                var returnVoid = methodDefinition.ReturnType == ModuleDefinition.TypeSystem.Void;
                var methodProcessor = new MethodProcessor();

                methodProcessor.Process(methodDefinition, returnVoid, references);
                return;
            }
            LogError(string.Format("Could not process '{0}'. async methods are not currently supported. Feel free to do it yourself if you want this feature.", methodDefinition.FullName));
        }

        private void WeaveMethods(ReferenceContainer references)
        {
            var methodDefinitions = SelectMethods(ModuleDefinition, NoLogAttributeName);

            foreach (var methodDefinition in methodDefinitions)
            {
                WeaveMethod(methodDefinition, references);
            }
        }

        #endregion
    }
}
