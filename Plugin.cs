using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace FasterCrafting;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class FasterCraftingPlugin : BasePlugin
{
	internal static new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("FasterCrafting");
	public override void Load()
	{
		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loading!");
		
		Harmony.CreateAndPatchAll(typeof(FasterCraft));
		Harmony.CreateAndPatchAll(typeof(Warper));

		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loaded!");
	}
}
