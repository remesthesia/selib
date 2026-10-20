using System;
using RoR2;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SeLibNode(Type parent) : SearchableAttribute
{
    public Type Parent = parent;
}