using HarmonyLib;
using TheJazMaster.Bucket.Artifacts;

namespace TheJazMaster.Bucket.Patches;

public class AEndTurnPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AEndTurn), nameof(AEndTurn.Begin)),
			prefix: new HarmonyMethod(typeof(AEndTurnPatches), nameof(AEndTurn_Begin_Prefix))
		);
    }

    private static void AEndTurn_Begin_Prefix(AStatus __instance, G g, State s, Combat c)
    {
        foreach (Artifact item in s.EnumerateAllArtifacts()) {
            if (item is IBeforeTurnEndArtifact artifact) {
                artifact.BeforeTurnEnd(g, s, c);
            }
        }
    }
}
