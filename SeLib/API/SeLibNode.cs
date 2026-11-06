using System;
using HG.Reflection;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SeLibNode : SearchableAttribute
{
    internal Type Parent => parent as Type;
    internal Type Type => target as Type;
    
    private readonly object parent;
    
    public SeLibNode(Type parentType)
    {
        parent = parentType;
    }
}