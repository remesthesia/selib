using BepInEx;

namespace SeLib.API;

public abstract class BaseRoR2Plugin<T> : BaseRoR2Plugin where T : BaseRoR2Plugin<T>
{
    public static T Instance { get; internal set; }
}

public abstract class BaseRoR2Plugin : BaseUnityPlugin
{
    internal BaseRoR2Plugin()
    {
    }
}