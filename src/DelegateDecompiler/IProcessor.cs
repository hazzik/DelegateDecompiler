using Mono.Reflection;

namespace DelegateDecompiler
{
    internal interface IProcessor
    {
        bool Process(ProcessorState state, Instruction instruction);
    }
}