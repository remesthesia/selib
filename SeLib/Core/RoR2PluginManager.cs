using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HG.Reflection;
using SeLib.API;
using SeLib.API.Assets;
using SeLib.API.Content;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    private static Dictionary<Type, List<SeLibNode>> _plugins;
    
    public static void AddPlugin<T>(BaseRoR2Plugin<T> plugin) where T : BaseRoR2Plugin<T>
    {
        plugin.IsDebug = typeof(T).IsDefined(typeof(DebuggableAttribute), true);

        if (_plugins == null)
        {
            _plugins = new Dictionary<Type, List<SeLibNode>>();
            var test = SearchableAttribute.GetInstances<SeLibNode>();
            foreach (var node in test.Cast<SeLibNode>())
            {
                if (!_plugins.TryGetValue(node.Parent, out List<SeLibNode> list)) list = new List<SeLibNode>();
                list.Add(node);
            }
        }

        if (!_plugins.TryGetValue(typeof(T), out var attributes)) return;
        
        var c = new List<ContentProvider>();
        foreach (var attribute in attributes)
        {
            if (attribute.Type.IsSubclassOf(typeof(AssetProvider))) 
                plugin.Assets = Activator.CreateInstance(attribute.Type) as AssetProvider;
            
            if (attribute.Type.IsSubclassOf(typeof(ContentProvider))) 
                c.Add(Activator.CreateInstance(attribute.Type) as ContentProvider);
        }
        
        if (plugin.Assets != null)
        {
            if (!_plugins.TryGetValue(plugin.Assets.GetType(), out attributes)) return;
            var idk = new List<AssetNode>();
            foreach (var attribute in attributes)
            {
                idk.Add(Activator.CreateInstance(attribute.Type) as AssetNode);
            }

            plugin.Assets.Children = idk;
        }
        
        

        plugin.Content = c.AsReadOnly();
        foreach (var content in plugin.Content)
        {
            if (!_plugins.TryGetValue(content.GetType(), out attributes)) return;

            var idk = new List<ContentNode>();
            foreach (var attribute in attributes)
            {
                idk.Add(Activator.CreateInstance(attribute.Type) as ContentNode);
            }

            content.Children = idk;
        }
        
        plugin.Assets?.Init();
        foreach (var content in plugin.Content)
        {
            content.Init();
        }

      
    }
}