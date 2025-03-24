using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace TheJazMaster.Bucket.Patches;

public class DrawThreePatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(DrawThree), nameof(DrawThree.GetActions)),
			postfix: new HarmonyMethod(typeof(DrawThreePatches), nameof(DrawThree_GetActions_Postfix))
		);
    }

    private static void DrawThree_GetActions_Postfix(DrawThree __instance, List<CardAction> __result, State s, Combat c)
    {
        if (__result.Last() is { } last && last.dialogueSelector == null)
            last.dialogueSelector = ".DrawThree";
    }
}
