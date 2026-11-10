using SeLib.API;

namespace SeLib.Core;

public abstract class BaseNode<TNode, TProvider> : INode where TNode : BaseNode<TNode, TProvider> where TProvider : BaseProvider<TProvider, TNode>
{
    public BaseRoR2Plugin Owner => Parent.Owner;
    public TProvider Parent { get; internal set; }
    
    public abstract string Identifier { get; }
    
    internal BaseNode()
    {
    }
}