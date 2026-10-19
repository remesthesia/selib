using SeLib.Core;

namespace SeLib.API;

public class BaseNode : INode
{
    public BaseSeLibPlugin Owner { get; internal set; }
}