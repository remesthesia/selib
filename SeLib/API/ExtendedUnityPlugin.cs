using System.Diagnostics.CodeAnalysis;
using BepInEx;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class ExtendedUnityPlugin<T> : ExtendedUnityPlugin where T : ExtendedUnityPlugin<T>
{
    
}

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class ExtendedUnityPlugin : BaseUnityPlugin
{
    
}