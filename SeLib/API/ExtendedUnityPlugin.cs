using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using SeLib.Core;
using UnityEngine;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class ExtendedUnityPlugin<T> : ExtendedUnityPlugin where T : ExtendedUnityPlugin<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        PluginManager.AddPlugin(Instance = this as T);
        foreach (var type in typeof(T).Assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(ContentModule)) && type.Assembly != typeof(SeLibPlugin).Assembly)
            {
                Debug.Log(type.Name);
                
                var thing = (Activator.CreateInstance(type) as ContentModule);
                    thing.Test();
                    
            }
                
        }
    }
}

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class ExtendedUnityPlugin : BaseUnityPlugin, INode
{
    public ExtendedUnityPlugin Owner => this;

    // ReSharper disable once InconsistentNaming
    public string GUID => Info.Metadata.GUID;
    public string Name => Info.Metadata.Name;
    public Version Version => Info.Metadata.Version;

    public bool IsDebug { get; internal set; }

    internal ExtendedUnityPlugin()
    {
        
    }
}