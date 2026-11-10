using System;
using System.Linq;
using System.Reflection;
using SeLib.API;
using SeLib.API.Assets;
using SeLib.API.Content;

namespace SeLib.Core;

internal static class RoR2PluginManager
{
    public static void Register<T>(BaseRoR2Plugin<T> plugin) where T : BaseRoR2Plugin<T>
    {
        var children = typeof(T).Assembly.GetTypes()
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<SeLibNode>(false)))
            .Where(x => x.Attr != null).ToLookup(x => x.Attr.Parent, x => x.Type);

        var assetProvider = InstantiateChildren<BaseAssetProvider>(typeof(T)).LastOrDefault();
        var contentProviders = InstantiateChildren<BaseContentProvider>(typeof(T));

        if (assetProvider != null)
        {
            plugin.Assets = assetProvider;
            assetProvider.Children = InstantiateChildren<BaseAssetNode>(assetProvider.GetType());
            assetProvider.Init();
        }

        plugin.Content = contentProviders.ToArray();
        foreach (var content in contentProviders)
        {
            content.Children = InstantiateChildren<BaseContentNode>(content.GetType());
            content.Init();
        }

        return;
        V[] InstantiateChildren<V>(Type parent) where V : class => children[parent].Select(Activator.CreateInstance).OfType<V>().ToArray();
    }
}