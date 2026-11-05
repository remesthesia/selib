using BepInEx;
using SeLib.Core;

namespace SeLib.API;

public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    public static T Instance { get; internal set; }

    internal override void Awake()
    {
        Instance = this as T;
        base.Awake();
    }
}

public abstract class BaseRoR2Plugin : BaseUnityPlugin
{
    internal BaseRoR2Plugin()
    {
    }

    public virtual void OnRegister() { }
    public virtual void OnPostRegister() { }

    internal virtual void Start() => OnPostRegister();
    internal virtual void Awake()
    {
        RoR2PluginManager.AddPlugin(this);
        OnRegister();
    }
    
}