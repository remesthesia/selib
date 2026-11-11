using System.Collections;
using SeLib.Core;

namespace SeLib.API.Assets;

public abstract class BaseAssetNode : BaseNode<BaseAssetNode, BaseAssetProvider>
{
    public abstract IEnumerator Init();
}