using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Yalf.Fody
{
    public static class CecilHelper
    {
        #region Public Methods and Operators

        public static int AddVariable<T>(this MethodDefinition method)
        {
            method.Body.Variables.Add(method.Module.ImportVariable<T>());

            return method.Body.Variables.Count - 1;
        }

        public static int AddVariable(this MethodDefinition method, TypeReference typeReference)
        {
            method.Body.Variables.Add(method.Module.ImportVariable(typeReference));

            return method.Body.Variables.Count - 1;
        }

        public static Instruction Append(this Instruction instruction, Instruction instructionAfter, ILProcessor processor)
        {
            processor.InsertAfter(instruction, instructionAfter);

            return instructionAfter;
        }

        public static Instruction AppendBoxAndResolveRefIfNecessary(this Instruction instruction, ILProcessor processor,
            TypeReference typeReference)
        {

            // some of the following code is partially based on IL injection blog post from Girish Jain
            if (typeReference.IsByReference)
            {
                // it might throw an exception, prefer for weaving to fail in such case
                var referencedTypeSpec = (TypeSpecification)typeReference;

                var byRefLoadOpCode = GetByRefLoadOpCode(referencedTypeSpec.ElementType.MetadataType);

                if (byRefLoadOpCode.HasValue)
                {
                    return instruction
                        .Append(processor.Create(byRefLoadOpCode.Value), processor)
                        .Append(processor.Create(OpCodes.Box, referencedTypeSpec.ElementType), processor);
                }
                else if (referencedTypeSpec.ElementType.IsValueType)
                {
                    // no "known" mapping from referenced metadata type
                    // check if we have value type and default to Ldobj
                    return instruction
                        .Append(processor.Create(OpCodes.Ldobj, referencedTypeSpec.ElementType), processor)
                        .Append(processor.Create(OpCodes.Box, referencedTypeSpec.ElementType), processor);
                }

                // It is a ref reference type so just use reference the pointer
                return instruction.Append(processor.Create(OpCodes.Ldind_Ref), processor);
            } 
            else if (typeReference.IsValueType || typeReference.IsGenericParameter)
            {
                // If it is a value type then you need to box the instance as we are going 
                // to add it to an array which is of type object (reference type)
                // ------------------------------------------------------------

                // Box the parameter type
                return instruction.Append(processor.Create(OpCodes.Box, typeReference), processor);
            }

            return instruction;
        }

        private static OpCode? GetByRefLoadOpCode(MetadataType referencedMetadataType)
        {
            switch (referencedMetadataType)
            {
                case MetadataType.Boolean:
                case MetadataType.SByte:
                    return OpCodes.Ldind_I1;

                case MetadataType.Int16:
                    return OpCodes.Ldind_I2;

                case MetadataType.Int32:
                    return OpCodes.Ldind_I4;

                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return OpCodes.Ldind_I8;

                case MetadataType.Byte:
                    return OpCodes.Ldind_U1;

                case MetadataType.UInt16:
                case MetadataType.Char:
                    return OpCodes.Ldind_U2;

                case MetadataType.UInt32:
                    return OpCodes.Ldind_U4;

                case MetadataType.Single:
                    return OpCodes.Ldind_R4;

                case MetadataType.Double:
                    return OpCodes.Ldind_R8;

                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    return OpCodes.Ldind_I;

                default:
                    return null;
            }
        }

        public static Instruction AppendDebugWrite(this Instruction instruction, ILProcessor processor, string message,
            ModuleDefinition module)
        {
            return
                instruction.AppendLdstr(processor, message).Append(
                    processor.Create(OpCodes.Call, module.ImportMethod(typeof(Debug), "WriteLine", new[] { typeof(string) })),
                    processor);
        }

        public static Instruction AppendLdarg(this Instruction instruction, ILProcessor processor, int index)
        {
            return instruction.Append(processor.Create(OpCodes.Ldarg, index), processor);
        }

        public static Instruction AppendLdcI4(this Instruction instruction, ILProcessor processor, int value)
        {
            return instruction.Append(processor.Create(OpCodes.Ldc_I4, value), processor);
        }

        public static Instruction AppendLdloc(this Instruction instruction, ILProcessor processor, int index)
        {
            return instruction.Append(processor.Create(OpCodes.Ldloc, index), processor);
        }

        public static Instruction AppendLdstr(this Instruction instruction, ILProcessor processor, string str)
        {
            return instruction.Append(processor.Create(OpCodes.Ldstr, str), processor);
        }

        public static Instruction AppendStloc(this Instruction instruction, ILProcessor processor, int index)
        {
            return instruction.Append(processor.Create(OpCodes.Stloc, index), processor);
        }

        public static bool ContainsAttribute(this MethodDefinition methodDefinition, MemberReference attributeType)
        {
            return methodDefinition.CustomAttributes.Any(x => x.Constructor.DeclaringType.FullName == attributeType.FullName);
        }

        public static bool ContainsAttribute(this MethodDefinition methodDefinition, string attributeTypeName)
        {
            return methodDefinition.CustomAttributes.Any(x => x.Constructor.DeclaringType.Name == attributeTypeName);
        }

        public static bool ContainsAttribute(this TypeDefinition typeDefinition, string attributeTypeName)
        {
            return typeDefinition.CustomAttributes.Any(x => x.Constructor.DeclaringType.Name == attributeTypeName);
        }

        public static bool ContainsAttribute(this ModuleDefinition moduleDefinition, string attributeTypeName)
        {
            return moduleDefinition.Assembly.HasCustomAttributes &&
                moduleDefinition.Assembly.CustomAttributes.Any(x => x.Constructor.DeclaringType.Name == attributeTypeName);
        }

        public static MethodDefinition GetInheritedPropertyGet(this TypeDefinition baseType, string propertyName)
        {
            MethodDefinition methodDefinition = baseType.GetPropertyGet(propertyName);

            if (methodDefinition == null && baseType.BaseType != null)
            {
                return baseType.BaseType.Resolve().GetInheritedPropertyGet(propertyName);
            }

            if (methodDefinition == null && baseType.BaseType == null)
            {
                return null;
            }

            if (methodDefinition.IsPrivate)
            {
                return null;
            }

            return methodDefinition;
        }

        public static MethodDefinition GetMethod(this TypeDefinition typeDefinition, string methodName,
            MemberReference returnType, ICollection<MemberReference> parameterTypes)
        {
            return
                typeDefinition.Methods.SingleOrDefault(
                    x =>
                        x.Name == methodName && x.ReturnType.FullName == returnType.FullName &&
                            x.Parameters.ToList().Select(y => y.ParameterType.FullName).IsEqualTo(parameterTypes.Select(y => y.FullName)));
        }

        public static MethodDefinition GetPropertyGet(this TypeDefinition typeDefinition, string propertyName)
        {
            return typeDefinition.Properties.Where(x => x.Name == propertyName).Select(x => x.GetMethod).SingleOrDefault();
        }

        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName)
        {
            return module.ImportMethod(typeof(T), methodName);
        }

        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName, Type[] types)
        {
            return module.ImportMethod(typeof(T), methodName, types);
        }

        public static MethodReference ImportMethod(this ModuleDefinition module, Type type, string methodName)
        {
            MethodInfo methodInfo = type.GetMethod(methodName);

            return module.Import(methodInfo);
        }

        public static MethodReference ImportMethod(this ModuleDefinition module, Type type, string methodName, Type[] types)
        {
            MethodInfo methodInfo = type.GetMethod(methodName, types);

            return module.Import(methodInfo);
        }

        public static TypeReference ImportType<T>(this ModuleDefinition module)
        {
            return module.ImportType(typeof(T));
        }

        public static TypeReference ImportType(this ModuleDefinition module, Type type)
        {
            return module.Import(type);
        }

        public static VariableDefinition ImportVariable<T>(this ModuleDefinition module)
        {
            return new VariableDefinition(module.Import(typeof(T)));
        }

        public static VariableDefinition ImportVariable(this ModuleDefinition module, TypeReference typeReference)
        {
            return new VariableDefinition(module.Import(typeReference));
        }

        public static bool IsEqualTo<T>(this IEnumerable<T> collection, IEnumerable<T> collectionToCompare)
        {
            int index = 0;

            IEnumerable<T> list = collection as IList<T> ?? collection.ToList();
            IEnumerable<T> listToCompare = collectionToCompare as IList<T> ?? collectionToCompare.ToList();

            if (list.Count() != listToCompare.Count())
            {
                return false;
            }

            foreach (T item in list)
            {
                if (!EqualityComparer<T>.Default.Equals(item, listToCompare.ElementAt(index)))
                {
                    return false;
                }

                index++;
            }

            return true;
        }

        public static MethodReference MakeGeneric(this MethodReference method, params TypeReference[] arguments)
        {
            if (method.GenericParameters.Count != arguments.Length)
            {
                throw new ArgumentException("Invalid number of generic type arguments supplied");
            }

            if (arguments.Length == 0)
            {
                return method;
            }

            GenericInstanceMethod genericTypeReference = new GenericInstanceMethod(method);

            foreach (TypeReference argument in arguments)
            {
                genericTypeReference.GenericArguments.Add(argument);
            }

            return genericTypeReference;
        }

        public static Instruction Prepend(this Instruction instruction, Instruction instructionBefore, ILProcessor processor)
        {
            processor.InsertBefore(instruction, instructionBefore);

            return instructionBefore;
        }

        public static void RemoveAttribute(this MethodDefinition methodDefinition, string attributeTypeName)
        {
            methodDefinition.CustomAttributes
                .Where(x => x.Constructor.DeclaringType.Name == attributeTypeName).ToList()
                .ForEach(x => methodDefinition.CustomAttributes.Remove(x));
        }

        public static void RemoveAttribute(this TypeDefinition typeDefinition, string attributeTypeName)
        {
            typeDefinition.CustomAttributes
                .Where(x => x.Constructor.DeclaringType.Name == attributeTypeName).ToList()
                .ForEach(x => typeDefinition.CustomAttributes.Remove(x));
        }

        public static void RemoveAttribute(this AssemblyDefinition assembly, string attributeTypeName)
        {
            assembly.CustomAttributes
                .Where(x => x.Constructor.DeclaringType.Name == attributeTypeName).ToList()
                .ForEach(x => assembly.CustomAttributes.Remove(x));
        }

        #endregion
    }
}
