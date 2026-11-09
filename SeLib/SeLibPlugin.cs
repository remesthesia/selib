using System;
using BepInEx;
using SeLib.Core.Bootstrap;

namespace SeLib;

[BepInPlugin("com.remesthesia.selib","selib", "1.0.0")]
public class SeLibPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        BootstrapPatcher.Init();
    }
}