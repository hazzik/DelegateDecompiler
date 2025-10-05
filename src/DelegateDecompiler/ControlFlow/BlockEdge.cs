namespace DelegateDecompiler.ControlFlow
{
    public sealed class BlockEdge
    {
        internal BlockEdge(Block from, Block to, EdgeKind kind, bool isException = false)
        {
            From = from;
            To = to;
            Kind = kind;
            IsException = isException || kind is EdgeKind.Exception or EdgeKind.Finally or EdgeKind.Filter;
        }

        public Block From { get; }
        public Block To { get; }
        public EdgeKind Kind { get; }
        public bool IsException { get; }

        public override string ToString() => $"{From.Id} -({Kind}{(IsException ? ",ex" : string.Empty)})-> {To.Id}";
    }
}