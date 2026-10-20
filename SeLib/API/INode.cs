namespace SeLib.API;

public interface INode<out T> : INode where T : INode
{
    T Parent { get; }
}

public interface INode
{
    BaseRoR2Plugin Owner { get; }
}