using System;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SeLibNode(Type parentType) : Attribute
{
    public Type Parent => _parent as Type;
    private readonly object _parent = parentType;
}