using System.Collections.Generic;
using System.Linq;
using RoR2;
using SeLib.API;
using UnityEngine;

namespace SeLib.Core;

public static class PluginManager
{
    private static List<BaseSeLibPlugin> _plugins;
    
    internal static void AddPlugin<T>(BaseSeLibPlugin<T> plugin) where T : BaseSeLibPlugin<T>
    {
        //plugin.
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init()
    {
        On.RoR2.RoR2Application.OnLoad += (orig, self) =>
        {
            foreach (var plugin in _plugins)
            {
                
            }
            var queue = new Queue<INode>(_plugins);
            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                var attributes = SeLibNode.GetNodes(parent.GetType());
                
                foreach (var attribute in attributes)
                {
                    attribute.Parent
                }
            }
            
             return orig(self);
        };
    }
}