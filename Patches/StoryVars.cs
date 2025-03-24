using HarmonyLib;

namespace TheJazMaster.Bucket.Patches;

internal class StoryVarsPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
	
	internal static readonly string CreatedTrashKey = "CreatedTrash";

	public static void Apply() {
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterCombatLine)),
			postfix: new HarmonyMethod(typeof(StoryVarsPatches), nameof(StoryVars_ResetAfterCombatLine_Postfix))
		);
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterCombat)),
			postfix: new HarmonyMethod(typeof(StoryVarsPatches), nameof(StoryVars_ResetAfterCombat_Postfix))
		);
	}

	private static void StoryVars_ResetAfterCombatLine_Postfix(StoryVars __instance) {
		__instance.RemoveModData(CreatedTrashKey);
	}

	private static void StoryVars_ResetAfterCombat_Postfix(StoryVars __instance) {
		StoryVars_ResetAfterCombatLine_Postfix(__instance);
	}
}