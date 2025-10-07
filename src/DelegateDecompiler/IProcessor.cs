using Mono.Reflection;

namespace DelegateDecompiler
{
    internal interface IProcessor
    {
        void Process(ProcessorState state, Instruction instruction);
    }
}