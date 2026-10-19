using RoR2;
using UnityEngine;

namespace SeLib.API;

public static class RuntimePrefabThing
{
    public static Transform Root { get; private set; }

    [SystemInitializer]
    public static void Init()
    {
        On.RoR2.Util.IsPrefab += (orig, gameObject) =>
        {
            var result = orig(gameObject);
            if (!result) result = gameObject.transform.parent == Root;
            
            return result;
        };
    }
    public static GameObject ClonePrefab(this GameObject prefab, string name)
    {
        if (Root == null)
        {
            Root = new GameObject("Root").transform;
            Root.SetParent(SeLibPlugin.Instance.transform);
            Root.gameObject.SetActive(false);
        }
        
        var instance = GameObject.Instantiate(prefab, Root);
        instance.name = name;
        
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;

        return instance;
    }
}