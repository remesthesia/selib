
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HG;
using RoR2;
using RoR2.ContentManagement;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zio.FileSystems;

namespace SeLib.Core.Bootstrap;

internal static class SeLibBootstrap
{
	private static readonly ReadableProgress<float> genericProgressReceiver = new ReadableProgress<float>();
	private static readonly ReadableProgress<float> contentLoadProgressReceiver = new ReadableProgress<float>();

	public static void Init()
	{
		BootstrapPatcher.Apply();
	}
	
	public static IEnumerator FinishGameRoutine()
	{
		var manageScenesCoroutine = RoR2Application.instance.StartCoroutine(ManageScenes());
		var updateProgressBarCoroutine = RoR2Application.instance.StartCoroutine(UpdateProgressBar());
		var lodCoroutine = RoR2Application.instance.StartCoroutine(Load());

		yield return manageScenesCoroutine;
		yield return updateProgressBarCoroutine;
		yield return lodCoroutine;
	}

	public static IEnumerator LoadGameContent()
	{
		ContentManager.collectContentPackProviders += addContentPackProvider =>
		{
			addContentPackProvider(new RoR2Content());
			addContentPackProvider(new DLC1Content());
			addContentPackProvider(new JunkContent());
			foreach (var contentProvider in RoR2PluginManager.GetContentProviders()) addContentPackProvider(contentProvider);
		};

		var timeSpan = TimeSpan.FromSeconds(1.0 / 60.0);
		var stopwatch = new Stopwatch();
		var coroutine = ContentManager.LoadContentPacks(contentLoadProgressReceiver);

		var coroutines = new Stack<IEnumerator>([coroutine]);
		while (coroutines.Count > 0)
		{
			stopwatch.Restart();
			do
			{
				var enumerator = coroutines.Peek();
				if (enumerator.Current is IEnumerator next)
				{
					coroutines.Push(next);
					continue;
				}

				while (!enumerator.MoveNext())
				{
					coroutines.Pop();
					if (coroutines.Count <= 0) break;

					enumerator = coroutines.Peek();
				}
			} while (coroutines.Count > 0 && stopwatch.Elapsed < timeSpan);

			stopwatch.Stop();
			yield return null;
		}
	}

	[SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
	[SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
	private static IEnumerator ManageScenes()
	{
		PlatformSystems.networkManager.ServerChangeScene("splash");
		while (!SceneManager.GetSceneByName("splash").isLoaded)
			yield return null;

		var loadingCanvas = GameObject.Find("Loading Canvas");
		if (loadingCanvas)
		{
			var background = loadingCanvas.transform.Find("Background");
			if (background) background.gameObject.SetActive(false);
		}

		var introSceneDefHandle = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/intro/intro.asset");
		while (!introSceneDefHandle.IsDone) yield return null;

		SceneCatalog.mostRecentSceneDef = introSceneDefHandle.Result;

		var handle = Addressables.LoadAssetAsync<GameObject>("f5a901d12172cea45ae283b8c359bff4");
		while (!handle.IsDone) yield return null;

		while (!SceneManager.GetSceneByName("intro").isLoaded) yield return null;
		UnityEngine.Object.Instantiate(handle.Result, RoR2Application.instance.transform);

		var canvas = GameObject.Find("Canvas");
		var skipVoteOverlay = canvas.transform.Find("SkipVoteOverlay").gameObject;

		while (!RoR2Application.loadFinished)
		{
			skipVoteOverlay.SetActive(false);
			yield return null;
		}

		skipVoteOverlay.SetActive(true);
		loadingCanvas.SetActive(false);
	}

	[SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
	private static IEnumerator UpdateProgressBar()
	{
		var loadingPercentIndicatorLabel = (from label in UnityEngine.Object.FindObjectsOfType<TMP_Text>()
			where label.name.Equals("LoadingPercentIndicator", StringComparison.Ordinal)
			select label).First();

		var loadingPercentAnimation = loadingPercentIndicatorLabel.GetComponentInChildren<Animation>();

		const float GenericProgressWeight = 0.2F;
		const float ContentLoadProgressWeight = 0.8F;

		while (!RoR2Application.loadFinished)
		{
			var num = (genericProgressReceiver.value * GenericProgressWeight +
			           contentLoadProgressReceiver.value * ContentLoadProgressWeight) * 100f;
			var percent = Mathf.Clamp(Mathf.RoundToInt(num), 0, 100);

			loadingPercentIndicatorLabel.SetText($"{percent}%");
			loadingPercentAnimation.clip.SampleAnimation(loadingPercentAnimation.gameObject,
				(num / 100f) * 0.99f * loadingPercentAnimation.clip.length);

			yield return null;
		}
	}

	[SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
	private static IEnumerator Load()
	{
		Language.collectLanguageRootFolders += list => list.Add(System.IO.Path.Combine(Application.streamingAssetsPath, "Language"));
		Language.Init();
		
		genericProgressReceiver.Report(0.15F);
		yield return null;

		WwiseIntegrationManager.Init();
		
		genericProgressReceiver.Report(0.2F);
		yield return null;

		var fileSystem = new PhysicalFileSystem();
		var subFileSystem = new SubFileSystem(fileSystem, fileSystem.ConvertPathFromInternal(Application.dataPath));

		RoR2Application.fileSystem = subFileSystem;
		RoR2Application.cloudStorage = subFileSystem;
		
		genericProgressReceiver.Report(0.3F);
		yield return null;

		if (!(RoR2Application.loadSteamworksClient?.Invoke() ?? false))
		{
			Application.Quit();
			yield break;
		}
		
		genericProgressReceiver.Report(0.36F);
		yield return null;

		while (!PlatformSystems.textDataManager.InitializedConfigFiles)
		{
			UnityEngine.Debug.Log("Text config stuff still happening...");
			yield return null;
		}
		
		genericProgressReceiver.Report(0.38F);
		yield return null;

		UnityEngine.Object.DontDestroyOnLoad(new GameObject("Console", typeof(RoR2.Console)));
		
		genericProgressReceiver.Report(0.4F);
		yield return null;

		yield return RoR2Application.instance.LoadGameContent();
		yield return null;

		SystemInitializerAttribute.Execute();
		
		genericProgressReceiver.Report(0.9F);
		yield return null;

		LocalUserManager.Init();
		
		genericProgressReceiver.Report(0.92F);
		yield return null;

		PlatformSystems.saveSystem.LoadUserProfiles();
		
		genericProgressReceiver.Report(0.95F);
		yield return null;

		if (RoR2Application.onLoad != null)
		{
			RoR2Application.onLoad();
			RoR2Application.onLoad = null;
		}
		genericProgressReceiver.Report(1F);
		yield return null;
		
		RoR2Application.loadFinished = true;
	}
}