using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeLib.API.Meta;

public static class BrandingHelper
{
    private static readonly Dictionary<MiniSceneSpriteType, WeightedSelection<Sprite>> _spriteReplacements = [];
    private static readonly System.Random _random = new();

    public static void AddMiniSceneSpriteReplacement(MiniSceneSpriteType type, Sprite sprite, float weight = 1)
    {
        _spriteReplacements[type].AddChoice(sprite, weight);
    }

    internal static Sprite TryPickMiniSceneSpriteReplacement(MiniSceneSpriteType type)
    {
        return _spriteReplacements[type].Evaluate((float)_random.NextDouble());
    }
    
    [SeLibInitializer(typeof(SeLibPlugin))]
    private static void Init()
    {
        foreach (var obj in Enum.GetValues(typeof(MiniSceneSpriteType)))
        {
            var selection = new  WeightedSelection<Sprite>();
            selection.AddChoice(null, 1);
            
            _spriteReplacements.Add((MiniSceneSpriteType)obj, selection);
        }
    }
}