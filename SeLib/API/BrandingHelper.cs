using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeLib.API;

public static class BrandingHelper
{
    public static Dictionary<MiniSceneSprite, List<Sprite>> _map = [];
    
    public static void AddLoadingSprite(MiniSceneSprite target, Sprite sprite, int weight = 1)
    {
        if (!_map.TryGetValue(target, out List<Sprite> list))
        {
            list = new List<Sprite>();
            _map[target] = list;
        }

        for (int i = 0; i < weight; i++)
        {
            list.Add(sprite);
        }
    }

    public static Sprite[] GetSprites(MiniSceneSprite sprite)
    {
        return _map.TryGetValue(sprite, out var list) ? list.Concat([null]).ToArray() : [];
    }
}