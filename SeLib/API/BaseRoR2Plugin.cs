using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BepInEx;
using RoR2;
using SeLib.API.Assets;
using SeLib.API.Content;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
    }
}

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class BaseRoR2Plugin : BaseUnityPlugin, INode
{
    public BaseRoR2Plugin Owner => this;
    
    public AssetProvider Assets { get; private set; }
    public ContentProvider[] Content { get; private set; }
    
    internal BaseRoR2Plugin()
    {
    }

    private void Awake()
    {
        
    }
}