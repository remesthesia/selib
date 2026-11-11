using BepInEx;
using HG;
using SeLib.API.Assets;
using SeLib.API.Content;
using SeLib.Core;

namespace SeLib.API;

public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    public static T Instance { get; private set; }

    private void Awake() => RoR2PluginManager.Register(Instance = this as T);
}

public abstract class BaseRoR2Plugin : BaseUnityPlugin
{ 
    public BaseAssetProvider Assets { get; internal set; }
    public ReadOnlyArray<BaseContentProvider> Content { get; internal set; }

    public virtual void OnInit()
    {
    }
}