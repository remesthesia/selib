using System.Collections.Generic;
using SeLib.API;
using UnityEngine;

namespace SeLib.Core;

internal static class PluginManager
{
    public static IReadOnlyList<ExtendedUnityPlugin> AllPlugins => Plugins;
    private static readonly List<ExtendedUnityPlugin> Plugins = [];
    
    public static void AddPlugin<T>(ExtendedUnityPlugin<T> plugin) where T : ExtendedUnityPlugin<T>
    {
        plugin.IsDebug = typeof(T).IsDefined(typeof(Debug), false);
        Plugins.Add(plugin);
    }
}