using System;
using System.Collections.Generic;
using System.Diagnostics;
using RoR2;
using SeLib.API;
using Debug = UnityEngine.Debug;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    private static bool _isDebug;
    private static readonly Dictionary<string, BaseRoR2Plugin> _plugins = [];

    public static void Register(BaseRoR2Plugin plugin)
    {
        plugin.IsDebug = plugin.GetType().IsDefined(typeof(DebuggableAttribute), true);
        
        _isDebug = _isDebug || plugin.IsDebug;
        _plugins.Add(plugin.GUID, plugin);
            
        Debug.Log($"Registered {plugin.Name} ver. {plugin.Version}{(plugin.IsDebug ? "*" : "")} ({plugin.GUID})");
    }
    
    public static void SubscribeToLifecycle()
    {
        RoR2Application.onStart += () => SafeInvokeAll(x => x.OnStart());
        RoR2Application.onLoad += () => SafeInvokeAll(x => x.OnLoad());
        
        if (_isDebug) Debug.Log("Subscribed to RoR2 lifecycle events.");
    }
    
    private static void SafeInvokeAll(Action<BaseRoR2Plugin> action)
    {
        foreach (var (_, plugin) in _plugins)
        {
            try { action(plugin); }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }
}