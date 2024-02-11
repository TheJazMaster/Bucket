using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Threading.Tasks;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TheJazMaster.Bucket.Artifacts;
using System.Runtime.CompilerServices;

namespace TheJazMaster.Bucket.Patches;
#nullable enable

public class CardBrowsePatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(CardBrowse), nameof(CardBrowse.GetCardList)),
			postfix: new HarmonyMethod(typeof(CardBrowsePatches), nameof(CardBrowse_GetCardList_Postfix))
		);
    }

	private static void CardBrowse_GetCardList_Postfix(CardBrowse __instance, ref List<Card> __result, G g)
	{
		if (__instance.browseSource == CardBrowse.Source.DrawPile && g.state.route is Combat combat) {
			foreach (Artifact item in g.state.EnumerateAllArtifacts())
			{
				if (item is XRayVisionArtifact artifact) {
					List<Card> result = __result;
					__result = g.state.deck.Where(result.Contains).Reverse().ToList();
					break;
				}
			}
		}
	}
}