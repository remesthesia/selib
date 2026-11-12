using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SeLib.API;
using SeLib.API.Assets;
using SeLib.API.Content;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    private static List<BaseRoR2Plugin> _plugins = [];
    
    public static BaseAssetProvider[] GetAssetProviders()
    {
        var list = new List<BaseAssetProvider>();
        foreach (var plugin in _plugins)
        {
            if (plugin.Assets != null) list.Add(plugin.Assets);
        }
        return list.ToArray();
    }
    
    public static BaseContentProvider[] GetContentProviders()
    {
        var list = new List<BaseContentProvider>();
        foreach (var plugin in _plugins)
        {
            if (plugin.Content.Length > 0) list.AddRange(plugin.Content);
        }
        return list.ToArray();
    }
    
    public static void Register<T>(BaseRoR2Plugin<T> plugin) where T : BaseRoR2Plugin<T>
    {
        _plugins.Add(plugin);
        plugin.OnRegister();
        
        var children = typeof(T).Assembly.GetTypes()
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<SeLibNode>(false)))
            .Where(x => x.Attr != null).ToLookup(x => x.Attr.Parent, x => x.Type);

        var assetProvider = InstantiateChildren<BaseAssetProvider>(typeof(T)).LastOrDefault();
        var contentProviders = InstantiateChildren<BaseContentProvider>(typeof(T));

        if (assetProvider != null)
        {
            plugin.Assets = assetProvider;

            assetProvider.Owner = plugin;
            assetProvider.Children = InstantiateChildren<BaseAssetNode>(assetProvider.GetType());
            foreach (var child in assetProvider.Children)
            {
                child.Parent = assetProvider;
            }
            assetProvider.Init();
        }

        plugin.Content = contentProviders.ToArray();
        foreach (var content in contentProviders)
        {
            content.Owner = plugin;
            content.Children = InstantiateChildren<BaseContentNode>(content.GetType());
            foreach (var child in content.Children)
            {
                child.Parent = content;
            }
            content.Init();
        }
        
        SeLibInitializerAttribute.Run(plugin, SeLibInitializerType.AfterPluginRegister);

        return;
        V[] InstantiateChildren<V>(Type parent) where V : class => children[parent].Select(Activator.CreateInstance).OfType<V>().ToArray();
    }
}