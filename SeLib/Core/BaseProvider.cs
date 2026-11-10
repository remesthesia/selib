using HG;
using SeLib.API;

namespace SeLib.Core;

public abstract class BaseProvider<TProvider, TNode> : INode where TProvider : BaseProvider<TProvider, TNode> where TNode : BaseNode<TNode, TProvider>
{
    public BaseRoR2Plugin Owner { get; internal set; }
    public ReadOnlyArray<TNode> Children { get; internal set; }

    public abstract void Init();

    internal BaseProvider()
    {
    }
}