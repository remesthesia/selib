using System;
using HG.Reflection;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SeLibNode : SearchableAttribute
{
    public Type Parent { get; private set; }
    public Type Type { get; private set; }
    
    public SeLibNode(string target) : this(Type.GetType(target))
    {
    }
    
    public SeLibNode(Type parent)
    {
        Parent = parent;
        Type = target as Type;
    }
}