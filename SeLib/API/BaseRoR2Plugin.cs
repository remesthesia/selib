using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BepInEx;
using SeLib.API.Assets;
using SeLib.Core;

namespace SeLib.API;

/// <inheritdoc/>
/// <typeparam name="T">The plugin's own type.</typeparam>
[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    /// <summary> Gets the registered instance of this plugin. </summary>
    public static T Instance { get; private set; }

    internal override void Awake()
    {
        Instance = this as T;
        base.Awake();
    }
}

/// <summary> Base class for plugins deriving from SeLib. Provides methods to integrate with BepInEx and RoR2's lifecycle. </summary>
public abstract class BaseRoR2Plugin : BaseUnityPlugin
{
    internal BaseRoR2Plugin()
    {
    }
    
    /// <summary> Gets the GUID of this plugin. </summary>
    public string GUID => Info.Metadata.GUID;
    /// <summary> Gets the name of this plugin. </summary>
    public string Name => Info.Metadata.Name;
    /// <summary> Gets the version of this plugin. </summary>
    public Version Version => Info.Metadata.Version;
    
    /// <summary> Gets whether this plugin was built with a debug configuration. </summary>
    public bool IsDebug { get; internal set; }
    
    public string Directory => System.IO.Directory.GetParent(Info.Location)?.FullName;
    
    public BaseAssetProvider Assets { get; internal set; }

    /// <summary> SeLib calls OnRegister after the plugin is first initialized. </summary>
    public virtual void OnRegister() { }
    /// <summary> SeLib calls OnPostRegister after all plugins are first initialized. </summary>
    public virtual void OnPostRegister() { }

    /// <summary> SeLib calls OnStart after RoR2's start is called. </summary>
    public virtual void OnStart() { }
    /// <summary> SeLib calls OnLoad after all content has been loaded and SystemInitializers have run. </summary>
    public virtual void OnLoad() { }

    internal virtual void Start() => OnPostRegister();
    internal virtual void Awake()
    {
        RoR2PluginManager.Register(this);
        OnRegister();
    }
}