using RoR2;
using RoR2.CharacterAI;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeLib.API;

public abstract class MonsterNode
{
    public abstract string Identifier { get; }
    
    public virtual bool IsChampion => false;
    
    //250 HP + 
    
    public void Test(ContentPack contentPack)
    {
        Debug.Log("AAA");
        var masterPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab").WaitForCompletion();
        foreach (var aiSkillDriver in masterPrefab.GetComponents<AISkillDriver>())
        {
            GameObject.Destroy(aiSkillDriver);
        }
        
      
        var bodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBody.prefab").WaitForCompletion();
        bodyPrefab = bodyPrefab.ClonePrefab("Test");
        
        var characterBody = bodyPrefab.GetComponent<CharacterBody>();
        characterBody.baseNameToken = $"{Identifier.ToUpperInvariant()}_BODY_NAME";
        characterBody.subtitleNameToken = IsChampion ? $"{Identifier.ToUpperInvariant()}_BODY_SUBTITLE" : "NULL_SUBTITLE";
        
        var deathRewards = bodyPrefab.GetComponent<DeathRewards>();
        deathRewards.logUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
        
        masterPrefab.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;;

        contentPack.bodyPrefabs.Add([bodyPrefab]);
        contentPack.unlockableDefs.Add([deathRewards.logUnlockableDef]);
        contentPack.networkedObjectPrefabs.Add([bodyPrefab, masterPrefab]);
        contentPack.masterPrefabs.Add([masterPrefab]);
        
    }
}