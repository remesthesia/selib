using System.Collections;
using RoR2.ContentManagement;
using SeLib.Core;

namespace SeLib.API.Content;

public abstract class BaseContentProvider<T> : BaseContentProvider where T : BaseContentProvider<T>
{
    public static T Instance { get; private set; }
    private readonly ContentPack _contentPack = new ContentPack();

    public override void Init() => Instance = this as T;

    public override IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
    {
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