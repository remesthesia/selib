using SeLib.API;

namespace SeLib.Core;

public abstract class BaseNode<T, V> : BaseNode where T : BaseNode<T, V> where V : BaseProvider<V, T>
{
    public V Parent { get; internal set; }
    
    internal BaseNode()
    {
    }
}

public abstract class BaseNode
{
    public BaseRoR2Plugin Owner { get; internal set; }

    internal BaseNode()
    {
    }
}