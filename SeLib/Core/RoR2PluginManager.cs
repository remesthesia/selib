using System;
using System.Collections.Generic;
using RoR2;
using SeLib.API;
using UnityEngine;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    private static readonly Dictionary<string, BaseRoR2Plugin> _plugins = [];

    public static void Register(BaseRoR2Plugin plugin)
    {
        _plugins.Add(plugin.GUID, plugin);
        
        Debug.Log($"Registered {plugin.Name} ver. {plugin.Version} ({plugin.GUID})");
    }
    
    // TODO: onUpdate events
    public static void SubscribeToLifecycle()
    {
        RoR2Application.onStart += () => SafeInvokeAll(x => x.OnStart());
        RoR2Application.onLoad += () => SafeInvokeAll(x => x.OnLoad());
    }
    
    // TODO: ????
    private static void SafeInvokeAll(Action<BaseRoR2Plugin> action)
    {
        foreach (var plugin in _plugins)
        {
            try
            {
                action(plugin.Value);
            }
            catch (Exception ex)
            {
                // TODO: Replace this with plugin specific logging at some point
                Debug.LogException(ex);
            }
        }
    }
}