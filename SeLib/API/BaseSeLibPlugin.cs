using System.Diagnostics.CodeAnalysis;
using BepInEx;
using SeLib.Core;

namespace SeLib.API;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class BaseSeLibPlugin<T> : BaseSeLibPlugin where T : BaseSeLibPlugin
{
    public static T Instance { get; private set; }
}

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class BaseSeLibPlugin : BaseUnityPlugin, INode
{
    public BaseSeLibPlugin Owner => this;
}