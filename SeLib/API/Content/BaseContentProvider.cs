using System.Collections;
using RoR2;
using RoR2.ContentManagement;
using SeLib.Core;
using UnityEngine;

namespace SeLib.API.Content;

public abstract class BaseContentProvider<T> : BaseContentProvider where T : BaseContentProvider<T>
{
    public static T Instance { get; private set; }
    private readonly ContentPack _contentPack = new ContentPack();

    public override void Init() => Instance = this as T;

    public override IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
    {
        foreach (var holderName in Owner.Assets.GetAllHolderNames())
        {
            var request = Owner.Assets.LoadAllAssetsFromHolderAsync<GameObject>(holderName);
            while (!request.IsDone) yield return null;
            
            
            Debug.logger.Log(holderName + " Is Done");

            foreach (var asset in request.Assets)
            {
                Debug.logger.Log("Asset: " + asset.name);
                Debug.Log(asset.GetComponent<CharacterBody>());
                if (asset.GetComponent<CharacterBody>())
                {
                    Debug.Log("Body");
                    _contentPack.bodyPrefabs.Add([asset]);
                }
                if (asset.GetComponent<CharacterMaster>()) _contentPack.masterPrefabs.Add([asset]);
            }
        }
        
        
        var loadHelper = AddressablesLoadHelper.CreateUsingDefaultResourceLocator($"ContentPack:{Identifier}");
        loadHelper.AddContentPackLoadOperation(_contentPack);
        
        while (loadHelper.coroutine.MoveNext())
        {
            args.ReportProgress(loadHelper.progress.value);
            yield return loadHelper.coroutine.Current;
        }
    }

    public override IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
    {
        ContentPack.Copy(_contentPack, args.output);
        yield break;
    }

    public override IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
    {
        yield break;
    }
}

public abstract class BaseContentProvider : BaseProvider<BaseContentProvider, BaseContentNode>, IContentPackProvider
{
    protected abstract string Identifier { get; }
    
    internal BaseContentProvider()
    {
    }

    public abstract IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args);

    public abstract IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args);

    public abstract IEnumerator FinalizeAsync(FinalizeAsyncArgs args);
    
    string IContentPackProvider.identifier => Identifier;
}