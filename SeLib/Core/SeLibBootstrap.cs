using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HG;
using RoR2;
using RoR2.ContentManagement;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zio.FileSystems;
using Console = RoR2.Console;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Path = System.IO.Path;
using SceneCatalog = On.RoR2.SceneCatalog;

namespace SeLib.Core;

internal static class SeLibBootstrap
{
	private static readonly ReadableProgress<float> genericProgress = new ReadableProgress<float>();
	private static readonly ReadableProgress<float> contentPackProgress = new ReadableProgress<float>();
	private static bool isIntroReady;
	
	public static void Init()
	{
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Disabled;
		
		On.RoR2.RoR2Application.InitializeGameRoutine += InitializeGameRoutine;
		On.RoR2.RoR2Application.LoadGameContent += LoadGameContent;
		On.RoR2.SceneCatalog.Init += SceneCatalogOnInit;
		
		SceneManager.sceneLoaded += (scene, loadSceneMode) =>
		{
			Debug.LogFormat("Loaded scene {0} loadSceneMode={1}", scene.name, loadSceneMode);
		};
        
		SceneManager.sceneUnloaded += scene =>
		{
			Debug.LogFormat("Unloaded scene {0}", scene.name);
		};
        
		SceneManager.activeSceneChanged += (oldScene, newScene) =>
		{
			Debug.LogFormat("Active scene changed from {0} to {1}", oldScene.name, newScene.name);
		};
	}

	private static void SceneCatalogOnInit(SceneCatalog.orig_Init orig)
	{
		orig();

		SceneDef[] sceneDefs = new SceneDef[RoR2.SceneCatalog.allSceneDefs.Length + 2];
		sceneDefs[0] = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/intro/intro.asset").WaitForCompletion();
		sceneDefs[1] = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/splash/splash.asset").WaitForCompletion();
		for (int i = 0; i < RoR2.SceneCatalog.allSceneDefs.Length; i++)
		{
			sceneDefs[i + 2] = RoR2.SceneCatalog.allSceneDefs[i];
		}
	}

	private static IEnumerator LoadGameContent(On.RoR2.RoR2Application.orig_LoadGameContent orig, RoR2Application self)
	{
		ContentManager.collectContentPackProviders += (ContentManager.AddContentPackProviderDelegate addContentPackProvider) =>
			{
				addContentPackProvider(new RoR2Content());
				addContentPackProvider(new JunkContent());
				addContentPackProvider(new DLC1Content());
			
			};
		
		var maxLoadTimePerFrame = TimeSpan.FromSeconds(1.0 / 240.0);
		IEnumerator item = ContentManager.LoadContentPacks(contentPackProgress);
		Stopwatch totalLoadStopwatch = new Stopwatch();
		totalLoadStopwatch.Start();
		Stopwatch thisFrameLoadStopwatch = new Stopwatch();
		int previousProgressPercent = 0;
		Stack<IEnumerator> coroutineStack = new Stack<IEnumerator>();
		coroutineStack.Push(item);
		while (coroutineStack.Count > 0)
		{
			thisFrameLoadStopwatch.Restart();
			int num = 0;
			do
			{
				num++;
				IEnumerator enumerator = coroutineStack.Peek();
				if (enumerator.Current is IEnumerator item2)
				{
					coroutineStack.Push(item2);
					continue;
				}

				while (!enumerator.MoveNext())
				{
					coroutineStack.Pop();
					if (coroutineStack.Count == 0)
					{
						break;
					}

					enumerator = coroutineStack.Peek();
				}
			} while (coroutineStack.Count > 0 && thisFrameLoadStopwatch.Elapsed < maxLoadTimePerFrame);

			thisFrameLoadStopwatch.Stop();
			int num2 = Mathf.FloorToInt(contentPackProgress.value * 100f);
		

			yield return new WaitForEndOfFrame();
		}

		yield return null;
		UnityEngine.Debug.LogFormat("Game content load completed in {0}ms.", totalLoadStopwatch.ElapsedMilliseconds);
	}

	private static IEnumerator InitializeGameRoutine(On.RoR2.RoR2Application.orig_InitializeGameRoutine orig, RoR2.RoR2Application self)
	{
		var handle = Addressables.LoadAssetAsync<GameObject>("efbf53f311fee654c8f6b269d80e813b");

		var standaloneWindows64 = Path.Combine(Application.streamingAssetsPath, "aa", "StandaloneWindows64");
		var splashSceneRequest = AssetBundle.LoadFromFileAsync(Path.Combine(standaloneWindows64, "ror2-base-splash_scenes_all.bundle"));
		var introSceneRequest = AssetBundle.LoadFromFileAsync(Path.Combine(standaloneWindows64, "ror2-base-intro_scenes_all.bundle"));
		
		UnitySystemConsoleRedirector.Redirect();
		RewiredIntegrationManager.Init();
		
		while (!handle.IsDone) yield return null;
		
		Object.Instantiate(handle.Result);

		while (!SceneManager.GetSceneByName("loadingbasic").isLoaded) yield return null;
		
		var canvas = GameObject.Find("Canvas");
		canvas.AddComponent<SetDontDestroyOnLoad>();
		canvas.GetComponent<Canvas>().sortingOrder = 9999;
		canvas.name = "Loading Canvas";
		yield return null;
		
		if (self.platformManagerPrefab) Object.Instantiate(self.platformManagerPrefab);
		PlatformSystems.InitNetworkManagerSystem(Object.Instantiate(self.networkManagerPrefab));
		PlatformSystems.InitPlatformManagerObject(self.gameObject);
		yield return null;
		
		Object.DontDestroyOnLoad(new GameObject("Console", typeof(Console)));
		
		while (!splashSceneRequest.isDone) yield return null;

		PlatformSystems.networkManager.ServerChangeScene("splash");	
		while (!SceneManager.GetSceneByName("splash").isLoaded) yield return null;
		
		canvas.transform.Find("Background").gameObject.SetActive(false);

		self.StartCoroutine(HandleScenes(introSceneRequest));
		self.StartCoroutine(HandleProgress());
		self.StartCoroutine(HandleLoading(self));
	}

	private static IEnumerator HandleScenes(AssetBundleCreateRequest introRequest)
	{
		while (!introRequest.isDone) yield return null;
		while (!isIntroReady) yield return null;
		
		PlatformSystems.networkManager.ServerChangeScene("intro");
		
		yield return null;
	}
	
	private static IEnumerator HandleProgress()
	{
		var loadingPercentIndicatorLabel = (from label in Object.FindObjectsOfType<TMP_Text>()
			where label.name.Equals("LoadingPercentIndicator", StringComparison.Ordinal)
			select label).FirstOrDefault();
		
		var loadingPercentAnimation = (loadingPercentIndicatorLabel ? loadingPercentIndicatorLabel.GetComponentInChildren<Animation>() : null);
		var loadingTextStringBuilder = new StringBuilder();

		
		while (!RoR2Application.loadFinished)
		{
			float num = (genericProgress.value * 0.4f + contentPackProgress.value * 0.6f) * 100f;
			int percent = Mathf.Clamp(Mathf.FloorToInt(num), 0, 100);

			if (loadingPercentIndicatorLabel)
				loadingPercentIndicatorLabel.SetText($"{percent}%");

			if (loadingPercentAnimation)
			{
				AnimationClip clip = loadingPercentAnimation.clip;
				clip.SampleAnimation(loadingPercentAnimation.gameObject,
					(num / 100f) * 0.99f * clip.length);
			}

			yield return null;
		}
	}
	
	private static IEnumerator HandleLoading(RoR2Application self)
	{
		Language.Init();
		genericProgress.Report(0.09F);
		yield return null;

		WwiseIntegrationManager.Init();
		isIntroReady = true;
		genericProgress.Report(0.16F);
		yield return null;
		
		// Say Intro Ready
		var fileSystem = new PhysicalFileSystem();
		var subFileSystem = new SubFileSystem(fileSystem, fileSystem.ConvertPathFromInternal(Application.dataPath));
		
		RoR2Application.fileSystem = subFileSystem;
		RoR2Application.cloudStorage = fileSystem;
		
		if (!(RoR2Application.loadSteamworksClient?.Invoke() ?? false))
		{
			Application.Quit();
			yield break;
		}
		
		while (!PlatformSystems.textDataManager.InitializedConfigFiles)
		{
			UnityEngine.Debug.Log("Text config stuff still happening...");
			yield return null;
		}
		
		genericProgress.Report(0.20F);
		yield return null;

		yield return self.LoadGameContent();
		yield return null;

		SystemInitializerAttribute.Execute();
		yield return null;
		genericProgress.Report(0.90F);

		LocalUserManager.Init();
		yield return null;
		genericProgress.Report(0.93F);

		PlatformSystems.saveSystem.LoadUserProfiles();
		yield return null;
		genericProgress.Report(1.0F);
	}
	

	/*
    public static void Init()
    {
        On.RoR2.RoR2Application.InitializeGameRoutine += InitializeGameRoutine;
        On.RoR2.RoR2Application.LoadGameContent += LoadGameContent;
        On.RoR2.SplashScreenController.Finish += OnFinish;
        
        SceneManager.sceneLoaded += (scene, loadSceneMode) =>
        {
            Debug.LogFormat("Loaded scene {0} loadSceneMode={1}", scene.name, loadSceneMode);
        };
        
        SceneManager.sceneUnloaded += scene =>
        {
            Debug.LogFormat("Unloaded scene {0}", scene.name);
        };
        
        SceneManager.activeSceneChanged += (oldScene, newScene) =>
        {
            Debug.LogFormat("Active scene changed from {0} to {1}", oldScene.name, newScene.name);
        };
    }

    private static void OnFinish(SplashScreenController.orig_Finish orig, RoR2.SplashScreenController self)
    {
	    PlatformSystems.networkManager.ServerChangeScene("intro");
    }

    private static IEnumerator LoadGameContent(On.RoR2.RoR2Application.orig_LoadGameContent orig, RoR2Application self)
    {
		TMP_Text loadingPercentIndicatorLabel = (from label in UnityEngine.Object.FindObjectsOfType<TMP_Text>()
			where label.name.Equals("LoadingPercentIndicator", StringComparison.Ordinal)
			select label).FirstOrDefault();
		Animation loadingPercentAnimation = (loadingPercentIndicatorLabel ? loadingPercentIndicatorLabel.GetComponentInChildren<Animation>() : null);
		StringBuilder loadingTextStringBuilder = new StringBuilder();
		TimeSpan maxLoadTimePerFrame = TimeSpan.FromSeconds(1.0 / 120.0);
		ContentManager.collectContentPackProviders += (ContentManager.AddContentPackProviderDelegate addContentPackProvider) =>
		{
			addContentPackProvider(new RoR2Content());
			addContentPackProvider(new JunkContent());
			addContentPackProvider(new DLC1Content());
		};
		ReadableProgress<float> contentLoadProgressReceiver = new ReadableProgress<float>();
		IEnumerator item = ContentManager.LoadContentPacks(contentLoadProgressReceiver);
		Stopwatch totalLoadStopwatch = new Stopwatch();
		totalLoadStopwatch.Start();
		Stopwatch thisFrameLoadStopwatch = new Stopwatch();
		int previousProgressPercent = 0;
		Stack<IEnumerator> coroutineStack = new Stack<IEnumerator>();
		coroutineStack.Push(item);
		while (coroutineStack.Count > 0)
		{
			thisFrameLoadStopwatch.Restart();
			int num = 0;
			do
			{
				num++;
				IEnumerator enumerator = coroutineStack.Peek();
				if (enumerator.Current is IEnumerator item2)
				{
					coroutineStack.Push(item2);
					continue;
				}
				while (!enumerator.MoveNext())
				{
					coroutineStack.Pop();
					if (coroutineStack.Count == 0)
					{
						break;
					}
					enumerator = coroutineStack.Peek();
				}
			}
			while (coroutineStack.Count > 0 && thisFrameLoadStopwatch.Elapsed < maxLoadTimePerFrame);
			thisFrameLoadStopwatch.Stop();
			int num2 = (int)Util.Remap(Mathf.FloorToInt(contentLoadProgressReceiver.value * 100f), 0, 100, 5, 100);
			if (previousProgressPercent != num2)
			{
				previousProgressPercent = num2;
				loadingTextStringBuilder.Clear();
				loadingTextStringBuilder.AppendInt(num2);
				loadingTextStringBuilder.Append("%");
				if ((bool)loadingPercentIndicatorLabel)
				{
					loadingPercentIndicatorLabel.SetText(loadingTextStringBuilder);
				}
			}
			if ((bool)loadingPercentAnimation)
			{
				AnimationClip clip = loadingPercentAnimation.clip;
				clip.SampleAnimation(loadingPercentAnimation.gameObject, contentLoadProgressReceiver.value * 0.99f * clip.length);
			}
			yield return new WaitForEndOfFrame();
		}
		RoR2.Console.instance.SubmitCmd(null, IntroCutsceneController.shouldSkip ? "set_scene title" : "set_scene intro");
		yield return null;
		UnityEngine.Debug.LogFormat("Game content load completed in {0}ms.", totalLoadStopwatch.ElapsedMilliseconds);
    }

    private static IEnumerator InitializeGameRoutine(On.RoR2.RoR2Application.orig_InitializeGameRoutine orig, RoR2.RoR2Application self)
    {
	    var handle = Addressables.LoadAssetAsync<GameObject>("efbf53f311fee654c8f6b269d80e813b");
	    var splashSceneRequest = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(Application.streamingAssetsPath, "aa", "StandaloneWindows64", "ror2-base-splash_scenes_all.bundle"));
	    var introSceneRequest = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(Application.streamingAssetsPath, "aa", "StandaloneWindows64", "ror2-base-intro_scenes_all.bundle"));

	    UnitySystemConsoleRedirector.Redirect();
	    RewiredIntegrationManager.Init();

		while (!handle.IsDone) yield return null;
	    Object.Instantiate(handle.Result);
	    
	    
	    while (!SceneManager.GetSceneByName("loadingbasic").isLoaded) yield return null;
	    yield return null;
	    yield return null;
	    
	    while (!splashSceneRequest.isDone) yield return null;

	    yield return null;
	    yield return null;
	    
	    PlatformSystems.InitNetworkManagerSystem(UnityEngine.Object.Instantiate(self.networkManagerPrefab));
	    if (self.platformManagerPrefab != null)
	    {
		    UnityEngine.Object.Instantiate(self.platformManagerPrefab);
	    }
	    PlatformSystems.InitPlatformManagerObject(self.gameObject);
	    
	    Application.backgroundLoadingPriority = ThreadPriority.Low;
	    
	    yield return new WaitForSeconds(1);
	    
	    PlatformSystems.networkManager.ServerChangeScene("splash");
	    while (!SceneManager.GetSceneByName("splash").isLoaded) yield return null;
	    
	    yield return null; 
	    yield return null; 
	    

	    yield return new WaitForSeconds(2.5F); 
	    GameObject.Find("SplashScreenController").transform.GetChild(0).gameObject.SetActive(true);
	    /// SceneManager.UnloadSceneAsync("loadingbasic");
    }
    */
}