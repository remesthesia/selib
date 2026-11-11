using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using HG;
using SeLib.API.Assets;
using SeLib.API.Content;
using SeLib.Core;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    public static T Instance { get; private set; }

    public override string GUID => Info.Metadata.GUID;
    public override string Name => Info.Metadata.Name;
    public override Version Version => Info.Metadata.Version;
    
    protected override string Directory => System.IO.Directory.GetParent(Info.Location)!.FullName;
    
    public override string GetPath(string[] paths) => System.IO.Path.Combine([Directory, ..paths]);
    public override string GetVersionText() => $"{Version}{(IsDebug ? "-debug" : string.Empty)}";

    private void Awake() => RoR2PluginManager.Register(Instance = this as T);
}

public abstract class BaseRoR2Plugin : BaseUnityPlugin
{ 
    public BaseAssetProvider Assets { get; internal set; }
    public ReadOnlyArray<BaseContentProvider> Content { get; internal set; }
    
    public bool IsDebug { get; internal set; }
    
    public abstract string GUID { get; }   
    public abstract string Name { get; }   
    public abstract Version Version { get; }    
    
    protected abstract string Directory { get; }
    
    public abstract string GetPath(string[] paths);
    public abstract string GetVersionText();
    
    public virtual void OnRegister() { }
    public virtual void OnInit() { }
}