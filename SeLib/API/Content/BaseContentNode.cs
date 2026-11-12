using System.Collections;
using SeLib.Core;

namespace SeLib.API.Content;

public abstract class BaseContentNode : BaseNode<BaseContentNode, BaseContentProvider>
{
    internal BaseContentNode()
    {
    }
    
    public abstract IEnumerator Init();
}