using System;
using System.Collections.Generic;
using System.Reflection;
using SeLib.Core;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SeLibInitializerAttribute(Type pluginType, SeLibInitializerType type = SeLibInitializerType.AfterPluginRegister) : Attribute
{
    private static Dictionary<Type, Dictionary<SeLibInitializerType, List<MethodInfo>>> _pluginInitializers = [];
    
    public Type Plugin => _plugin as Type;
    public SeLibInitializerType Type { get; } = type;

    private readonly object _plugin = pluginType;

    public static void Run(BaseRoR2Plugin plugin, SeLibInitializerType type)
    {
        if (_pluginInitializers.TryGetValue(plugin.GetType(), out var dict))
        {
            if (dict.TryGetValue(type, out var methods))
            {
                foreach (var method in methods)
                {
                    method.Invoke(plugin, null);
                }
            }
          
        }
    }

    internal static void Scan()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods((BindingFlags)(-1)))
                    {
                        var attributes = methodInfo.GetCustomAttributes(typeof(SeLibInitializerAttribute), false);
                        if (attributes.Length <= 0) continue;

                        var attribute = attributes[0] as SeLibInitializerAttribute;
                        if (attribute == null) continue;

                        if (!_pluginInitializers.TryGetValue(attribute.Plugin, out var dict))
                        {
                            _pluginInitializers[attribute.Plugin] =
                                dict = new Dictionary<SeLibInitializerType, List<MethodInfo>>();
                        }

                        if (!dict.TryGetValue(attribute.Type, out var methods))
                        {
                            dict[attribute.Type] = methods = new List<MethodInfo>();
                        }

                        methods.Add(methodInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}