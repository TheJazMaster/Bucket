using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TheJazMaster.Bucket.Artifacts;

namespace TheJazMaster.Bucket.Patches;
#nullable enable

public class CardPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActionsOverridden)),
			postfix: new HarmonyMethod(typeof(CardPatches), nameof(Card_GetActionsOverridden_Postfix))
		);
    }

	private static void Card_GetActionsOverridden_Postfix(Card __instance, List<CardAction> __result, State s, Combat c)
	{
		foreach (Artifact item in s.EnumerateAllArtifacts()) {
			if (item is ICardActionAffectorArtifact artifact) {
				artifact.AffectCardActions(s, c, __instance, __result);
			}
		}
	}
}