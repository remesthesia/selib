using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EntityStates;
using RoR2.ContentManagement;

namespace SeLib.API.Content;

public abstract class ContentProvider<T> : ContentProvider where T : ContentProvider<T>
{
    [SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
    public override IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
    {
        Pack.identifier = Identifier;

        var loadHelper = AddressablesLoadHelper.CreateUsingDefaultResourceLocator($"ContentPack:{Identifier}");
        loadHelper.AddContentPackLoadOperation(Pack);
        loadHelper.AddGenericOperation(() =>
        {
            foreach (var type in typeof(T).GetNestedTypes())
            {
                switch (type.Name)
                {
                    case "Artifacts": ContentLoadHelper.PopulateTypeFields(type, Pack.artifactDefs); break;
                    case "Buffs": ContentLoadHelper.PopulateTypeFields(type, Pack.buffDefs, x => $"bd{x}"); break;
                    case "Equipment": ContentLoadHelper.PopulateTypeFields(type, Pack.equipmentDefs); break;
                    case "Elites": ContentLoadHelper.PopulateTypeFields(type, Pack.eliteDefs, x => $"ed{x}"); break;
                    case "GameEndings": ContentLoadHelper.PopulateTypeFields(type, Pack.gameEndingDefs); break;
                    case "Items": ContentLoadHelper.PopulateTypeFields(type, Pack.itemDefs); break;
                    case "ItemRelationshipTypes": ContentLoadHelper.PopulateTypeFields(type, Pack.itemRelationshipTypes); break;
                    case "MiscPickups": ContentLoadHelper.PopulateTypeFields(type, Pack.miscPickupDefs); break;
                    case "Survivors": ContentLoadHelper.PopulateTypeFields(type, Pack.survivorDefs); break;
                }
            }
        }, 0.05F);

        while (loadHelper.coroutine.MoveNext())
        {
            args.ReportProgress(loadHelper.progress.value);
            yield return loadHelper.coroutine.Current;
        }
    }

    public override void Init()
    {
        var entityStateTypes = new List<Type>();
        foreach (var type in typeof(T).Assembly.GetTypes())
        {
            if (type.Namespace != EntityStateNamespace) continue;
            if (!type.IsSubclassOf(typeof(EntityState))) continue;
            
            entityStateTypes.Add(type);
        }
        
        Pack.entityStateTypes.Add(entityStateTypes.ToArray());
    }
}

public abstract class ContentProvider : BaseProvider<ContentProvider, ContentNode>, IContentPackProvider
{
    public abstract string Identifier { get; }
    public virtual string EntityStateNamespace => string.Empty;
    
    public ContentPack Pack { get; } = new();

    internal ContentProvider()
    {
    }

    public abstract IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args);

    public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
    {
        ContentPack.Copy(Pack, args.output);
        yield break;
    }

    public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
    {
        yield break;
    }

    string IContentPackProvider.identifier => Identifier;
}