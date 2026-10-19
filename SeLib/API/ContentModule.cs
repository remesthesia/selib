using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using RoR2.ContentManagement;

namespace SeLib.API;

public abstract class ContentModule<T> : ContentModule where T : ContentModule<T>
{
    public static T Instance { get; private set; }
    
    [SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
    public override IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
    {
        ContentPack.identifier = Identifier;

        var loadHelper = AddressablesLoadHelper.CreateUsingDefaultResourceLocator("Test");
        loadHelper.AddContentPackLoadOperation(ContentPack);
        loadHelper.AddGenericOperation(() =>
        {
            foreach (var type in typeof(T).GetNestedTypes())
            {
                switch (type.Name)
                {
                    case "Artifacts":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.artifactDefs);
                        break;
                    case "Buffs":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.buffDefs, 
                            fieldName => $"bd{fieldName}");
                        break;
                    case "Elites":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.eliteDefs, 
                            fieldName => $"ed{fieldName}");
                        break;
                    case "Equipment":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.equipmentDefs);
                        break;
                    case "GameEndings":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.gameEndingDefs);
                        break;
                    case "Items":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.itemDefs);
                        break;
                    case "ItemRelationshipTypes":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.itemRelationshipTypes);
                        break;
                    case "MiscPickups":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.miscPickupDefs);
                        break;
                    case "Survivors":
                        ContentLoadHelper.PopulateTypeFields(type, ContentPack.survivorDefs);
                        break;
                }
            }
        }, 0.05F);

        while (loadHelper.coroutine.MoveNext())
        {
            args.ReportProgress(loadHelper.progress.value);
            yield return loadHelper.coroutine.Current;
        }   
    }

    public override void Test()
    {
        base.Test();
        Instance = this as T;
        
        foreach (var type in typeof(T).Assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(MonsterNode)))
                (Activator.CreateInstance(type) as MonsterNode).Test(ContentPack);
        }
    }
}

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
public abstract class ContentModule : IContentPackProvider
{
    public abstract string Identifier { get; }
    public virtual string EntityStateNamespace { get; }
    
    string IContentPackProvider.identifier => Identifier;
    
    internal readonly ContentPack ContentPack = new();

    public abstract IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args);

    public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
    {
        ContentPack.Copy(ContentPack, args.output);
        yield break;
    }

    public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
    {
        yield break;
    }
    
    

    public virtual void Test()
    {
        ContentManager.collectContentPackProviders += provider => provider(this);

       
    }
}