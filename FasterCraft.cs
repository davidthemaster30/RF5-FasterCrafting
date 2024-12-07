using HarmonyLib;

namespace FasterCrafting;

[HarmonyPatch]
internal static class FasterCraft
{
	internal static UIStrengthening? strengthMenu = null;
	internal static UICraftMenu? craftMenu = null;
	internal static UICraftResult? craftResult = null;
	internal static UICraftSuccess? success = null;
	internal static CursorController? cursor = null;
	internal static CursorLinker? lastSelect = null;
	internal static ButtonWorkBase? okBtn = null;
	internal static bool startSynthAnim = false;
	internal static bool startResultAnim = false;
	internal static bool doneUpgrade = false;
	internal static SynthMode synthMode = SynthMode.NONE;

	private static bool TrytoSearch(SynthMode mode)
	{
		CursorLinkConnector? parent;
		switch (mode)
		{
			case SynthMode.CRAFT:
				{
					parent = craftMenu;
					okBtn = parent?.GetComponentInChildren<UICraftSynthesisOK>(true);
					if (okBtn is null)
					{
						FasterCraftingPlugin.Log.LogError("Failed to find OK Button!");
						return false;
					}
					break;
				}
			case SynthMode.UPGRADE:
				{
					parent = strengthMenu;
					okBtn = parent?.GetComponentInChildren<UIStrengtheningOK>(true);
					if (okBtn is null)
					{
						FasterCraftingPlugin.Log.LogError("Failed to find OK Button!");
						return false;
					}
					break;
				}
			default:
				{
					FasterCraftingPlugin.Log.LogError("No SynthMode defined!");
					return false;
				}
		}

		success = parent?.GetComponentInChildren<UICraftSuccess>(true);
		if (success is null)
		{
			FasterCraftingPlugin.Log.LogError("Failed to find success Component!");
			return false;
		}
		cursor = parent?.GetComponentInChildren<CursorController>(true);
		if (cursor is null)
		{
			FasterCraftingPlugin.Log.LogError("Failed to find cursor Component!");
			return false;
		}

		return true;
	}

	[HarmonyPatch(typeof(UICraftMenu), "Start")]
	[HarmonyPostfix]
	internal static void StartCraft(UICraftMenu __instance)
	{
		synthMode = SynthMode.CRAFT;
		craftMenu = __instance;
		FasterCraftingPlugin.Log.LogInfo("Starting crafting shorcut");
	}

	[HarmonyPatch(typeof(UICraftMenu), "OnDestroy")]
	[HarmonyPostfix]
	internal static void EndCraft()
	{
		synthMode = SynthMode.NONE;
		craftMenu = null;
		craftResult = null;
		success = null;
		cursor = null;
		lastSelect = null;
		okBtn = null;
		startSynthAnim = false;
		doneUpgrade = false;
		FasterCraftingPlugin.Log.LogInfo("Ending crafting shorcut");
	}

	[HarmonyPatch(typeof(UIStrengthening), "Start")]
	[HarmonyPostfix]
	internal static void StartUpgrade(UIStrengthening __instance)
	{
		synthMode = SynthMode.UPGRADE;
		strengthMenu = __instance;
		FasterCraftingPlugin.Log.LogInfo("Starting upgrade shorcut");
	}

	[HarmonyPatch(typeof(UIStrengthening), "OnDestroy")]
	[HarmonyPostfix]
	internal static void EndUpgrade()
	{
		synthMode = SynthMode.NONE;
		strengthMenu = null;
		craftResult = null;
		success = null;
		cursor = null;
		lastSelect = null;
		okBtn = null;
		startSynthAnim = false;
		doneUpgrade = false;
		FasterCraftingPlugin.Log.LogInfo("Ending upgrade shorcut");
	}

	[HarmonyPatch(typeof(UICraftResult), "Start")]
	[HarmonyPostfix]
	internal static void ResultStart(UICraftResult __instance)
	{
		if (synthMode == SynthMode.NONE)
		{
			return;
		}
		craftResult = __instance;
	}

	[HarmonyPatch(typeof(UICraftResult), "Update")]
	[HarmonyPostfix]
	internal static void ResultUpdate()
	{
		if (success is null || cursor is null || okBtn is null)
		{
			FasterCraftingPlugin.Log.LogInfo($"Something is missing");
			if (!TrytoSearch(synthMode))
			{
				return;
			}
		}

		if (craftResult is null || success is null || cursor is null)
		{
			return;
		}

		if (craftResult.isPlaying)
		{
			startSynthAnim = true;
		}

		if (!craftResult.isPlaying && startSynthAnim && success.isActiveAndEnabled)
		{
			startSynthAnim = false;
			startResultAnim = true;
		}

		if (!success.isActiveAndEnabled && startResultAnim)
		{
			startResultAnim = false;
			if (synthMode == SynthMode.UPGRADE)
			{
				doneUpgrade = true;
			}
		}

		if (doneUpgrade && lastSelect != null)
		{
			doneUpgrade = false;
			cursor.NextFocusObject = lastSelect;
			cursor.NowFocusObject = lastSelect;
			FasterCraftingPlugin.Log.LogInfo("Try to change back position");
		}
	}

	[HarmonyPatch(typeof(UIMainController), "Update")]
	[HarmonyPostfix]
	internal static void InputUpdate()
	{
		if (RF5Input.Pad.End(RF5Input.Key.PS) && !startResultAnim && !startSynthAnim)
		{
			if (synthMode == SynthMode.CRAFT && craftMenu != null)
			{
				FasterCraftingPlugin.Log.LogInfo("Crafting...");
				lastSelect = cursor?.NowFocusObject;
				okBtn?.ButtonWork(RF5Input.Key.A);
			}
			else if (synthMode == SynthMode.UPGRADE && strengthMenu != null && !doneUpgrade)
			{
				FasterCraftingPlugin.Log.LogInfo("Upgrading...");
				lastSelect = cursor?.NowFocusObject;
				okBtn?.ButtonWork(RF5Input.Key.A);
			}
		}
	}
}
