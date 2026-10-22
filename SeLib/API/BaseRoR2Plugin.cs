using System.Collections.Generic;
using System.IO;
using BepInEx;
using SeLib.API.Assets;
using SeLib.API.Content;
using SeLib.Core;

namespace SeLib.API;

public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    public static T Instance { get; private set; }
    
    private void Awake() => RoR2PluginManager.AddPlugin(Instance = this as T);
}

public abstract class BaseRoR2Plugin : BaseUnityPlugin
{
    public bool IsDebug { get; internal set; }

    public AssetProvider Assets { get; internal set; }
    public IReadOnlyList<ContentProvider> Content { get; internal set; }

    internal BaseRoR2Plugin()
    {
    }
}