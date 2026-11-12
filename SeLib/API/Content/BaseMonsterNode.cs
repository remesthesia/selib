using System;
using System.Collections;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace SeLib.API.Content;

public abstract class BaseMonsterNode : BaseContentNode
{
    public virtual Type DeathState => typeof(GenericCharacterDeath);
    public virtual Type SpawnState => typeof(GenericCharacterSpawnState);
    public virtual Type MainState => typeof(GenericCharacterMain);

    public override IEnumerator Init()
    {
        var bodyPrefabHandle = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBody.prefab");
        var masterPrefabHandle = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab");
        
        while (!bodyPrefabHandle.IsDone) yield return null;
        while (!masterPrefabHandle.IsDone) yield return null;
        
        var bodyPrefab = Owner.Assets.ClonePrefab(bodyPrefabHandle.Result, $"{Identifier}Body");
        var masterPrefab = Owner.Assets.ClonePrefab(masterPrefabHandle.Result, $"{Identifier}Master");
        
        var characterBody = bodyPrefab.GetComponent<CharacterBody>();
        characterBody.baseNameToken = $"{Identifier.ToUpperInvariant()}_BODY_NAME";
        characterBody.subtitleNameToken = "NULL_SUBTITLE";
        characterBody.portraitIcon = null;
        
        var characterMaster = masterPrefab.GetComponent<CharacterMaster>();
        characterMaster.bodyPrefab = bodyPrefab;

        var modelLocator = bodyPrefab.GetComponent<ModelLocator>();
        Object.Destroy(modelLocator.modelTransform.gameObject);

        var modelRequest = Owner.Assets.LoadAssetFromHolderAsync<GameObject>(Identifier.ToLowerInvariant(), $"mdl{Identifier}");
        while (!modelRequest.IsDone) yield return null;

        var model = modelRequest.Asset;
        
        modelLocator.modelTransform = model.transform;
        modelLocator.modelTransform.parent = modelLocator.modelBaseTransform;
        
        Owner.Assets.SaveAssetToHolder(Identifier.ToLowerInvariant(), bodyPrefab);
        Owner.Assets.SaveAssetToHolder(Identifier.ToLowerInvariant(), masterPrefab);
    }
}