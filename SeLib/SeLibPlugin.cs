using BepInEx;
using SeLib.API;
using SeLib.Core.Bootstrap;

namespace SeLib;

[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public sealed class SeLibPlugin : BaseRoR2Plugin<SeLibPlugin>
{
    public override void OnInit()
    {
        SeLibBootstrap.Init();
    }
}