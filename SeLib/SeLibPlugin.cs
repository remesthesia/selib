using System.Linq;
using BepInEx;
using SeLib.API;
using SeLib.Core;

namespace SeLib;

[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public sealed class SeLibPlugin : BaseRoR2Plugin<SeLibPlugin>
{
    public override bool IsDebug => RoR2PluginManager.IsDebug;
}