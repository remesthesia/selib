using System;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SeLibInitializerAttribute(Type pluginType) : Attribute
{
    public Type Plugin => _plugin as Type;
    
    private readonly object _plugin = pluginType;
}