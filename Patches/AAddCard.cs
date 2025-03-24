using HarmonyLib;
using TheJazMaster.Bucket.Artifacts;

namespace TheJazMaster.Bucket.Patches;

public class AAddCardPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAddCard), nameof(AAddCard.Begin)),
			postfix: new HarmonyMethod(typeof(AAddCardPatches), nameof(AAddCard_Begin_Postfix))
		);
    }

    private static void AAddCard_Begin_Postfix(AAddCard __instance, G g, State s, Combat c)
    {
        if (__instance.amount == 1 && __instance.dialogueSelector is null && __instance.card.GetMeta().deck == Deck.trash) {
            s.storyVars.ApplyModData(StoryVarsPatches.CreatedTrashKey, true);
        }
    }
}
