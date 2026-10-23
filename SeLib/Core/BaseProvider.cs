using System.Collections.Generic;

namespace SeLib.Core;

public abstract class BaseProvider<T, V> : BaseNode where T : BaseProvider<T, V> where V : BaseNode<V, T>
{
    public IReadOnlyList<V> Children { get; internal set; }

    internal BaseProvider()
    {
    }
    
    public abstract void Init();
}