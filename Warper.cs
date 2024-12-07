using HarmonyLib;

namespace FasterCrafting;

public partial class Plugin
{
    [HarmonyPatch]
	public class Warper
	{
		[HarmonyPatch(typeof(UICraftMenu), nameof(UICraftMenu.CanRequestCraftNum))]
		[HarmonyPostfix]
		public static void CanRequestPatch(UICraftMenu __instance, ref bool __result)
		{
			if (__instance.CraftNum == 1 || __instance.CraftNum == __instance.CraftNumMax)
			{
				__result = true;
			}
		}

		[HarmonyPatch(typeof(UICraftMenu), nameof(UICraftMenu.RequestCraftNum))]
		[HarmonyPrefix]
		public static void RequestPatch(UICraftMenu __instance, ref bool isLeft)
		{
			if (__instance.CraftNum == 1 && isLeft)
			{
				__instance.CraftNum = __instance.CraftNumMax;
				isLeft = false;
			}
			else if (__instance.CraftNum == __instance.CraftNumMax && !isLeft)
			{
				__instance.CraftNum = 1;
				isLeft = true;
			}
		}
	}
}
