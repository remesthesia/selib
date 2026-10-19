using System;
using System.Collections.Generic;
using RoR2;

namespace SeLib.API;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SeLibNode : SearchableAttribute
{
    private static readonly Dictionary<Type, List<SeLibNode>> _nodesByParent = [];

    public Type Parent
    {
        get; private set;
    }

    public SeLibNode(Type parent)
    {
        Parent = parent;

        List<SeLibNode> nodes = null;
        if (!_nodesByParent.TryGetValue(parent, out nodes))
        {
            nodes = new List<SeLibNode>();
            _nodesByParent.Add(parent, nodes);
        }
        
        _nodesByParent.Add(parent, nodes);
    }

    public static List<SeLibNode> GetNodes(Type parent)
    {
        return _nodesByParent.TryGetValue(parent, out var list) ? list : [];
    }
}