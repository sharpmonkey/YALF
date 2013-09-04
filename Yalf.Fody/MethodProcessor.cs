using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;


namespace Yalf.Fody
{
    public class MethodProcessor
    {
        private MethodBody body;
        private VariableDefinition contextVar;
        private TypeReference _returnType;
        private MethodReference _recordReturnMethod;

        public void Process(MethodDefinition method, bool returnVoid, ReferenceContainer references)
        {
            try
            {
                InnerProcess(method, returnVoid, references);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("An error occurred processing '{0}'. Error: {1}", method.FullName, exception.Message));
            }
        }

        private void InnerProcess(MethodDefinition method, bool returnVoid, ReferenceContainer references)
        {
            body = method.Body;
            body.SimplifyMacros();

            if (body.Instructions.Count <= 3)
                return; // nothing to do (empty method)

            HandleReturnType(method, returnVoid, references);

            var ilProcessor = body.GetILProcessor();

            var firstInstruction = FirstInstructionSkipCtor(method);

            firstInstruction = RejigFirstInstruction(ilProcessor, firstInstruction);

            InjectContext(ilProcessor, firstInstruction, method, references);
            var returnInstruction = FixReturns();

            var beforeReturn = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(returnInstruction, beforeReturn);

            // exclude try-catch from constructors (lot's of pain otherwise)
            if (!method.IsConstructor)
            {
                var beginCatch = InjectIlForCatch(ilProcessor, beforeReturn, references);
                var beginFinally = InjectIlForFinaly(ilProcessor, beforeReturn, references);


                var catchHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
                    {
                        TryStart = firstInstruction,
                        TryEnd = beginCatch,
                        CatchType = references.ExceptionType,
                        HandlerStart = beginCatch,
                        HandlerEnd = beginFinally,
                    };

                body.ExceptionHandlers.Add(catchHandler);

                var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
                    {
                        TryStart = firstInstruction,
                        TryEnd = beginFinally,
                        HandlerStart = beginFinally,
                        HandlerEnd = beforeReturn,
                    };

                body.ExceptionHandlers.Add(finallyHandler);
            }
            else
            {
                InjectIlForDispose(ilProcessor, returnInstruction, references);
            }

            body.InitLocals = true;
            body.OptimizeMacros();
        }



        private Instruction InjectIlForCatch(ILProcessor processor, Instruction beforeReturn, ReferenceContainer references)
        {
            /*
                stloc.2 
                nop 
                ldloc.2 
                call void [Yalf]Yalf.Log::TraceException(class [mscorlib]System.Exception)
                nop 
                rethrow  
             */

            var exceptionVariable = new VariableDefinition(references.ExceptionType);
            body.Variables.Add(exceptionVariable);

            var beginCatch = beforeReturn.Prepend(processor.Create(OpCodes.Stloc, exceptionVariable), processor);
            beginCatch.Append(processor.Create(OpCodes.Nop), processor)
                          .AppendLdloc(processor, exceptionVariable.Index)
                          .Append(processor.Create(OpCodes.Call, references.TraceExceptionMethod), processor)
                          .Append(processor.Create(OpCodes.Nop), processor)
                          .Append(processor.Create(OpCodes.Rethrow), processor);

            return beginCatch;
        }

        private void InjectIlForDispose(ILProcessor processor, Instruction returnInstruction, ReferenceContainer references)
        {
            // load context then call dispose
            /*
                nop 
                ldloc.0 
                callvirt instance void [Yalf]Yalf.IContext::Dispose()
                return mumbo jumbo
             */

            returnInstruction.AppendLdloc(processor, contextVar.Index)
                        .Append(processor.Create(OpCodes.Callvirt, references.DisposeMethod), processor);

        }

        private Instruction InjectIlForFinaly(ILProcessor processor, Instruction beforeReturn, ReferenceContainer references)
        {
            // wrapped in nop (for try catch handling)
            // load context then call dispose
            /*
                nop 
                ldloc.0 
                callvirt instance void [Yalf]Yalf.IContext::Dispose()
                nop 
                endfinally 
             */

            var beginFinally = beforeReturn.Prepend(processor.Create(OpCodes.Nop), processor);
            
            beginFinally.AppendLdloc(processor, contextVar.Index)
                .Append(processor.Create(OpCodes.Callvirt, references.DisposeMethod), processor)
                .Append(processor.Create(OpCodes.Nop), processor)
                .Append(processor.Create(OpCodes.Endfinally), processor);

            return beginFinally;

        }

        private void HandleReturnType(MethodDefinition method, bool returnVoid, ReferenceContainer references)
        {
            _returnType = null;
            if (!returnVoid)
            {
                _returnType = method.ReturnType;
                _recordReturnMethod = references.CreateRecordReturnMethod(_returnType);
            }
        }

        Instruction FirstInstructionSkipCtor(MethodDefinition method)
        {
            if (method.IsConstructor && !method.IsStatic)
            {
                return body.Instructions.Skip(2).First();
            }
            return body.Instructions.First();
        }

        Instruction FixReturns()
        {
            // inject last ret statement, preparation for try/catch/finally block
            if (_returnType == null)
            {
                // nothing special here, just redirect all return calls to last (injected one)
                var instructions = body.Instructions;
                var lastRet = Instruction.Create(OpCodes.Nop);
                instructions.Add(lastRet);
                instructions.Add(Instruction.Create(OpCodes.Ret));

                for (var index = 0; index < instructions.Count - 1; index++)
                {
                    var instruction = instructions[index];
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        instructions[index].OpCode = OpCodes.Leave;
                        instructions[index].Operand = lastRet;
                    }
                }
                return lastRet;
            }
            else
            {
                // we have something to return, make sure we store results when jumping to last
                // and just before one and only return now, store the value
                var instructions = body.Instructions;

                var returnVariable = new VariableDefinition(_returnType);
                body.Variables.Add(returnVariable);

                /*
                 * where location 1 is return variable index, and 0 is context
                ldloc.1
                ldloc.1
                callvirt instance !!0 [Yalf]Yalf.IContext::RecordReturn<int32>(!!0)
                ldloc.1
                ret 
                 */

                // load return value
                var lastLd = Instruction.Create(OpCodes.Ldloc, returnVariable);
                instructions.Add(lastLd);
                //// and finally return
                instructions.Add(Instruction.Create(OpCodes.Ret));

                for (var index = 0; index < instructions.Count - 2; index++)
                {
                    var instruction = instructions[index];
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        // overwrite inner ret statement with jump to our last return variable load for return (created above)
                        // and prefix it with store to return variable
                        /*
                         * where location 1 is return variable index
                         * and L_xxxx is beginning of return handling (see above)
                         stloc.1 
                         leave.s L_xxxx
                         */
                        instructions[index].OpCode = OpCodes.Leave;
                        instructions[index].Operand = lastLd;
                        instructions.Insert(index, Instruction.Create(OpCodes.Stloc, returnVariable));
                        instructions.Insert(index + 1, Instruction.Create(OpCodes.Ldloc, contextVar));
                        instructions.Insert(index + 2, Instruction.Create(OpCodes.Ldloc, returnVariable));
                        instructions.Insert(index + 3, Instruction.Create(OpCodes.Callvirt, _recordReturnMethod));
                        index++;
                    }
                }
                return lastLd;
            }
        }

        void InjectContext(ILProcessor processor, Instruction firstInstruction, MethodDefinition method, ReferenceContainer references)
        {
            /*
            nop 
            ldstr "Namespace.Class.Method"
            ldc.i4.1 
            newarr object
            stloc.s CS$0$0001
            ldloc.s CS$0$0001
            ldc.i4.0 
            ldarg.1 
            box int32
            stelem.ref 
            ldloc.s CS$0$0001
            call class [Yalf]Yalf.IContext [Yalf]Yalf.Log::MethodContext(string, object[])
            stloc.0 
            */

            contextVar = new VariableDefinition(references.IContextType);
            body.Variables.Insert(0, contextVar);

            int objectParamsArrayIndex = method.AddVariable<object[]>();

            // Generate MethodContext calling method name
            var builder = new StringBuilder();

            builder.Append(string.Join(".", 
                method.DeclaringType.Namespace, 
                FormatType(method.DeclaringType), 
                FormatMethod(method)));

            var current = firstInstruction
                .Prepend(processor.Create(OpCodes.Ldstr, builder.ToString()), processor);

            // Create object[] for MethodContext
            current = current
                .AppendLdcI4(processor, method.Parameters.Count)
                .Append(processor.Create(OpCodes.Newarr, method.Module.ImportType<object>()), processor)
                .AppendStloc(processor, objectParamsArrayIndex);

            var nonStaticMethodAddOne = method.IsStatic ? 0 : 1;

            // Set object[] values
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                current = current
                    .AppendLdloc(processor, objectParamsArrayIndex)
                    .AppendLdcI4(processor, i);

                var paramType = method.Parameters[i].ParameterType;

                if (paramType.MetadataType == MetadataType.UIntPtr ||
                    paramType.MetadataType == MetadataType.FunctionPointer ||
                    paramType.MetadataType == MetadataType.IntPtr ||
                    paramType.MetadataType == MetadataType.Pointer)
                {
                    // don't store pointer types into object[] (we can't ToString them anyway)
                    // store type name as string instead
                    current = current.AppendLdstr(processor, paramType.FullName);
                }
                else
                {
                    current = current
                        .AppendLdarg(processor, i + nonStaticMethodAddOne)
                        .AppendBoxAndResolveRefIfNecessary(processor, paramType);
                }

                current = current.Append(processor.Create(OpCodes.Stelem_Ref), processor);
            }

            // Call Log.MethodContext
            current
                .AppendLdloc(processor, objectParamsArrayIndex)
                .Append(processor.Create(OpCodes.Call, references.MethodContextMethod), processor)
                .AppendStloc(processor, contextVar.Index);
        }

        /// <summary>
        /// Neatly format type/class output if it's generic.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string FormatType(TypeReference type)
        {
            if (type.HasGenericParameters)
                return string.Format("{0}<{1}>", type.Name.Split('`')[0], string.Join(", ", type.GenericParameters.Select(FormatType)));
            else
                return type.Name;
        }

        private static string FormatMethod(MethodReference m)
        {
            if (m.HasGenericParameters)
                return string.Format("{0}<{1}>", m.Name.Split('`')[0], string.Join(", ", m.GenericParameters.Select(FormatType)));
            else
                return m.Name;
        }

        /// <summary>
        /// Need to do this so we retain pointer from original first instruction to new first instruction (nop)
        /// for dbg file to point to it so VS debugger will step into weaved methods
        /// </summary>
        /// <param name="ilProcessor"></param>
        /// <param name="firstInstruction"></param>
        /// <returns></returns>
        private static Instruction RejigFirstInstruction(ILProcessor ilProcessor, Instruction firstInstruction)
        {
            /*
             From:
             opcode operand <-- pdb first line pointer
              
             To:
             nop <-- pdb first line pointer
             opcode operand <-- cloned second acting as first 
             
             */
            // clone first instruction which will be used as actual 
            var clonedSecond = CloneInstruction(firstInstruction);
            clonedSecond.Offset++;

            var sampleNop = ilProcessor.Create(OpCodes.Nop);

            // change actual first instruction to NOP
            firstInstruction.OpCode = sampleNop.OpCode;
            firstInstruction.Operand = sampleNop.Operand;

            // append second instruction which now is same as first one used to be at the start of this method
            // and actual first one is nop
            firstInstruction.Append(clonedSecond, ilProcessor);

            // return cloned second as new first instruction
            return clonedSecond;
        }

        private static Instruction CloneInstruction(Instruction i)
        {
            var c = Instruction.Create(OpCodes.Nop);
            c.OpCode = i.OpCode;
            c.Operand = i.Operand;
            c.Offset = i.Offset;
            return c;
        }
    }
}
