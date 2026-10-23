using System.Collections.Generic;

namespace SeLib.API;

public abstract class BaseProvider<T, TV> : BaseNode where T : BaseProvider<T, TV> where TV : BaseNode<TV, T>
{
    public IReadOnlyList<TV> Children { get; internal set; }

    internal BaseProvider()
    {
    }
    
    public abstract void Init();
}