using SeLib.Core;

namespace SeLib.API.Content;

public abstract class BaseContentProvider<T> : BaseContentProvider where T : BaseContentProvider<T>
{
    public static T Instance { get; private set; }

    public override void Init() => Instance = this as T;
}

public abstract class BaseContentProvider : BaseProvider<BaseContentProvider, BaseContentNode>
{
    internal BaseContentProvider()
    {
    }
}