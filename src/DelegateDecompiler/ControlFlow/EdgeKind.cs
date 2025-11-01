namespace DelegateDecompiler.ControlFlow
{
    public enum EdgeKind
    {
        FallThrough,
        UnconditionalBranch,
        ConditionalTrue,
        ConditionalFalse,
        Switch,
        Exception,
        Finally,
        Filter
    }
}