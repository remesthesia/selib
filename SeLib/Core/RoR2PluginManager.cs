using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RoR2;
using SeLib.API;
using SeLib.API.Assets;
using SeLib.API.Content;
using SearchableAttribute = HG.Reflection.SearchableAttribute;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    public static IReadOnlyList<BaseRoR2Plugin> Plugins => _plugins;
    public static bool IsDebug { get; private set; }

    private static Dictionary<Type, List<SeLibNode>> _nodes;
    private static readonly List<BaseRoR2Plugin> _plugins = [];

    public static void AddPlugin<T>(BaseRoR2Plugin<T> plugin) where T : BaseRoR2Plugin<T>
    {
        IsDebug = IsDebug || (plugin.IsDebug = typeof(T).IsDefined(typeof(DebuggableAttribute), true));
        _plugins.Add(plugin);
        
        InitializePlugin(plugin);
    }

    private static void InitializePlugin<T>(BaseRoR2Plugin<T> plugin) where T : BaseRoR2Plugin<T>
    {
        RoR2Application.onFixedUpdate += plugin.OnFixedUpdate;
        RoR2Application.onUpdate += plugin.OnUpdate;
        RoR2Application.onLateUpdate += plugin.OnLateUpdate;

        if (_nodes == null) CollectNodes();
        if (!_nodes!.TryGetValue(typeof(T), out var nodes)) return;

        List<ContentProvider> contentProviders = [];
        foreach (var node in nodes)
        {
            if (node.Type.IsSubclassOf(typeof(ContentProvider))) contentProviders.Add(Activator.CreateInstance(node.Type) as ContentProvider);
            else if (node.Type.IsSubclassOf(typeof(AssetProvider))) plugin.Assets = Activator.CreateInstance(node.Type) as AssetProvider;
        }
        
        plugin.Content = contentProviders;
        
        if (plugin.Assets != null) InitializeProvider(plugin.Assets);
        foreach (var provider in plugin.Content) InitializeProvider(provider);
    }

    private static void InitializeProvider<T, V>(BaseProvider<T, V> provider) where T : BaseProvider<T, V> where V : BaseNode<V, T>
    {
        if (!_nodes.TryGetValue(provider.GetType(), out var nodes)) return;

        provider.Children = [..nodes.Select(attribute => Activator.CreateInstance(attribute.Type) as V)];
        provider.Init();
    }

    private static void CollectNodes()
    {
        _nodes = new Dictionary<Type, List<SeLibNode>>();
        foreach (var node in SearchableAttribute.GetInstances<SeLibNode>().Cast<SeLibNode>())
        {
            if (!_nodes.TryGetValue(node.Parent, out var list)) list = [];
            
            list.Add(node);
        }
    }
}
