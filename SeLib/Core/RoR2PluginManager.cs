using System;
using System.Collections.Generic;
using System.Diagnostics;
using RoR2;
using SeLib.API;
using SeLib.API.Assets;
using Debug = UnityEngine.Debug;
using SearchableAttribute = HG.Reflection.SearchableAttribute;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    private static bool _isDebug;
    private static readonly Dictionary<string, BaseRoR2Plugin> _plugins = [];
    
    public static void Register(BaseRoR2Plugin plugin)
    {
        plugin.IsDebug = plugin.GetType().Assembly.IsDefined(typeof(DebuggableAttribute), true);
        
        var debugChanged = plugin.IsDebug && !_isDebug;
        if (debugChanged) _isDebug = true;
        
        _plugins.Add(plugin.GUID, plugin);
            
        Debug.Log($"Registered {plugin.Name} ver. {plugin.Version}{(plugin.IsDebug ? "-debug" : "")} ({plugin.GUID})");
        if (debugChanged) Debug.Log("RoR2PluginManager is now in debug mode.");
    }
    
    public static void SubscribeToLifecycle()
    {    
        RoR2Application.onStart += () => SafeInvokeAll(x => x.OnStart());
        RoR2Application.onLoad += () => SafeInvokeAll(x => x.OnLoad());
        
        if (_isDebug) Debug.Log($"Subscribed {_plugins.Count} plugins to the RoR2 lifecycle.");
    }

    public static void Init()
    {
        foreach (var plugin in _plugins.Values)
        {
            var attributes = SearchableAttribute.GetInstances<SeLibNode>();
            foreach (var attribute in attributes)
            {
                var node =  attribute as SeLibNode;
                if (node != null)
                {
                    if (node.Parent == plugin.GetType())
                    {
                        var type = node.Type;
                        var instance = Activator.CreateInstance(type);
                        if (instance is BaseAssetProvider)
                        {
                            var assetProvider = instance as BaseAssetProvider;
                            if (assetProvider != null)
                            {
                                plugin.Assets = assetProvider;
                                
                                assetProvider.Owner = plugin;
                                assetProvider.Init();
                            }
                        }
                    }
                }
            }
        }
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