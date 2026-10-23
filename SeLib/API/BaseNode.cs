namespace SeLib.API;

public abstract class BaseNode<T, TV> : BaseNode where T : BaseNode<T, TV> where TV : BaseProvider<TV, T>
{
    public TV Parent { get; internal set; }
    
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