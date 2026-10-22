using System.Collections.Generic;

namespace SeLib.API;

public abstract class BaseModule<T, TV> : BaseNode where T : BaseModule<T, TV> where TV : BaseNode<TV, T>
{
    public IReadOnlyList<TV> Children { get; internal set; }

    internal BaseModule()
    {
    }
    
    public abstract void Init();
}