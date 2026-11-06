using RoR2;
using UnityEngine;
using Path = System.IO.Path;

namespace SeLib.API.Assets;

public abstract class BaseAssetProvider<T> : BaseAssetProvider where T : BaseAssetProvider<T>
{
    
}

public abstract class BaseAssetProvider
{
    public BaseRoR2Plugin Owner { get; internal set; }
    
    public virtual string LanguageDirectory => Path.Combine(Owner.Directory, LanguageFolder);
    public virtual string LanguageFolder => "Language";
    
    public void Init()
    {
        Debug.Log("Initialized");
        Language.collectLanguageRootFolders += list =>
        {
            list.Add(LanguageDirectory);
            Debug.Log("Collected");
        };
    }
}