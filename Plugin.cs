using BepInEx;
using BepInEx.Logging;
using BepInEx.IL2CPP;
using HarmonyLib;

namespace FasterCrafting;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class Plugin : BasePlugin
{
	internal static new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("FasterCrafting");
	public override void Load()
	{
		// Plugin startup logic
		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		Harmony.CreateAndPatchAll(typeof(FasterCraft));
		Harmony.CreateAndPatchAll(typeof(Warper));
	}
}
