using System;
using System.Collections.Generic;
using RoR2;
using SeLib.API;
using UnityEngine;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    private static readonly List<BaseRoR2Plugin> _plugins = [];
    
    public static void AddPlugin(BaseRoR2Plugin plugin)
    {
        _plugins.Add(plugin);
    }
    
    public static void Init()
    {
        RoR2Application.onStart += () => Dispatch(x => x.OnStart());
        RoR2Application.onLoad += () => Dispatch(x => x.OnLoad());
    }
    
    private static void Dispatch(Action<BaseRoR2Plugin> action)
    {
        foreach (var plugin in _plugins)
        {
            try
            {
                action(plugin);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}