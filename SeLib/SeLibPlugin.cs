using System;
using BepInEx;
using SeLib.API;
using SeLib.Core;

namespace SeLib;

[BepInPlugin("com.remesthesia.selib2", "selib", "1.0.0")]
public sealed class SeLibPlugin : BaseRoR2Plugin<SeLibPlugin>
{
    private void Awake()
    {
        SeLibBootstrap.Init();
    }
}