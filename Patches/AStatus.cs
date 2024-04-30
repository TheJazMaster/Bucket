using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using TheJazMaster.Bucket.Artifacts;

namespace TheJazMaster.Bucket.Patches;

public class AStatusPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
			prefix: new HarmonyMethod(typeof(AStatusPatches), nameof(AStatus_Begin_Prefix)),
			postfix: new HarmonyMethod(typeof(AStatusPatches), nameof(AStatus_Begin_Postfix))
		);
    }

    private static void AStatus_Begin_Prefix(AStatus __instance, G g, State s, Combat c, ref int __state)
    {
        __state = s.ship.Get(__instance.status);
    }

    private static void AStatus_Begin_Postfix(AStatus __instance, G g, State s, Combat c, int __state)
    {
        if (__instance.targetPlayer && __instance.status == Status.heat) {
            __state = s.ship.Get(__instance.status) - __state;
            foreach (Artifact item in s.EnumerateAllArtifacts()) {
                if (item is IAfterStatusActionArtifact artifact) {
                    artifact.OnAfterStatusAction(s, c, __instance, __state);
                }
            }
        }
    }
}
