using System.Diagnostics.CodeAnalysis;
using BepInEx;
using SeLib.Core;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
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

    public virtual void OnStart() { }
    public virtual void OnLoad() { }

    internal virtual void Start() => OnPostRegister();
    internal virtual void Awake()
    {
        RoR2PluginManager.AddPlugin(this);
        OnRegister();
    }
}