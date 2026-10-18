using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class ExtendedUnityPlugin<T> : ExtendedUnityPlugin where T : ExtendedUnityPlugin<T>
{
    public static T Instance { get; private set; }
}

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class ExtendedUnityPlugin : BaseUnityPlugin
{
    public string GUID => Info.Metadata.GUID;
    public string Name => Info.Metadata.Name;
    public Version Version => Info.Metadata.Version;
}