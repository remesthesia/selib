using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RoR2;
using SeLib.API;
using SeLib.API.Meta;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SeLib.Core.Bootstrap;

internal static class BootstrapPatcher
{
    public static void Apply()
    {
        On.RoR2.RoR2Application.InitializeGameRoutine += RoR2Application_InitializeGameRoutine;
        On.RoR2.RoR2Application.LoadGameContent += RoR2Application_LoadGameContent;
        On.RoR2.MusicController.Init += MusicController_Init;
        On.RoR2.SplashScreenController.Finish += SplashScreenController_Finish;
    }

    private static IEnumerator RoR2Application_LoadGameContent(On.RoR2.RoR2Application.orig_LoadGameContent orig, RoR2Application self)
    {
        yield return SeLibBootstrap.LoadGameContent();
    }

    private static void SplashScreenController_Finish(On.RoR2.SplashScreenController.orig_Finish orig, SplashScreenController self)
    {
        PlatformSystems.networkManager.ServerChangeScene("intro");	
    }

    private static void MusicController_Init(On.RoR2.MusicController.orig_Init orig)
    {
        Debug.Log("Deferred MusicController.Init");
    }
    
    private static IEnumerator RoR2Application_InitializeGameRoutine(On.RoR2.RoR2Application.orig_InitializeGameRoutine orig, RoR2Application self)
    {
        var splashSceneDependencyHandle = Addressables.DownloadDependenciesAsync("fe13e00695117284fb0d1b25422a0529");
        var introSceneDependencyHandle = Addressables.DownloadDependenciesAsync("330d792ae1727574e969e68ce8e966d2");
        
        UnitySystemConsoleRedirector.Redirect();
        RewiredIntegrationManager.Init();

        foreach (var assetProvider in RoR2PluginManager.GetAssetProviders()) yield return assetProvider.LoadAssetBundles();
        
        var systemsCoroutine = self.StartCoroutine(SetupSystems());
        var canvasCoroutine = self.StartCoroutine(SetupCanvas());
        
        yield return systemsCoroutine;
        yield return canvasCoroutine;
        
        while (!splashSceneDependencyHandle.IsDone) yield return null;
        while (!introSceneDependencyHandle.IsDone) yield return null;

        yield return SeLibBootstrap.FinishGameRoutine();
    }

    private static IEnumerator SetupSystems()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("efbf53f311fee654c8f6b269d80e813b");
        while (!handle.IsDone) yield return null;   
        
        Object.Instantiate(handle.Result);
        PlatformSystems.InitNetworkManagerSystem(Object.Instantiate(RoR2Application.instance.networkManagerPrefab));
        PlatformSystems.InitPlatformManagerObject(RoR2Application.instance.gameObject);
        yield return null;
    }

    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
    private static IEnumerator SetupCanvas()
    {
        while (!SceneManager.GetSceneByName("loadingbasic").isLoaded) yield return null;

        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (rootGameObject.name != "Canvas") continue;
            
            var canvas = rootGameObject.GetComponent<Canvas>();
            rootGameObject.name = "Loading Canvas";
            canvas.sortingOrder = 9999;

            foreach (var image in canvas.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Image>(true))
            {
                if (image.gameObject.name.ToLower().Contains("commando") || image.transform.parent.gameObject.name.ToLower().Contains("commando"))
                    image.sprite = BrandingHelper.TryPickMiniSceneSpriteReplacement(MiniSceneSpriteType.Commando) ?? image.sprite;
                if (image.gameObject.name.ToLower().Contains("lemurian") ||
                    image.transform.parent.gameObject.name.ToLower().Contains("lemurian"))
                    image.sprite = BrandingHelper.TryPickMiniSceneSpriteReplacement(MiniSceneSpriteType.Lemurian) ?? image.sprite;

                if (image.gameObject.name.ToLower().Contains("golem") ||
                    image.transform.parent.gameObject.name.ToLower().Contains("golem"))
                    image.sprite = BrandingHelper.TryPickMiniSceneSpriteReplacement(MiniSceneSpriteType.Golem) ?? image.sprite;

            }
            
            Object.DontDestroyOnLoad(canvas);
            break;
        }
    }
}