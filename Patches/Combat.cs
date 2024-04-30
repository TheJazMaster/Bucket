using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using TheJazMaster.Bucket.Artifacts;

#nullable enable
namespace TheJazMaster.Bucket.Patches;

public class CombatPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
    static string? nanobotsCard = null;

    public static void Apply()
    {
        nanobotsCard = Instance.Helper.Content.Cards.LookupByUniqueName("clay.PhilipTheMechanic::Nanobots")?.UniqueName;
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
			transpiler: new HarmonyMethod(typeof(CombatPatches), nameof(Combat_TryPlayCard_Transpiler))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.SendCardToExhaust)),
			postfix: new HarmonyMethod(typeof(CombatPatches), nameof(Combat_SendCardToExhaust_Postfix))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.SendCardToHand)),
			postfix: new HarmonyMethod(typeof(CombatPatches), nameof(Combat_SendCardToHand_Postfix))
		);
    }

    private static IEnumerable<CodeInstruction> Combat_TryPlayCard_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        var stuff = new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<bool>(originalMethod),
                ILMatches.Brtrue,
                ILMatches.Ldarg(0),
                ILMatches.Ldarg(1),
                ILMatches.Ldloc(0),
                ILMatches.Ldfld("card").Anchor(out var anchor),
                ILMatches.Call("SendCardToDiscard")
            )
            .Anchors()
            .PointerMatcher(anchor)
            .Element();

        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldloc<bool>(originalMethod),
                ILMatches.Brtrue.GetBranchTarget(out var branchTarget).Anchor(out anchor),
                ILMatches.Ldarg(0),
                ILMatches.Ldarg(1),
                ILMatches.Ldloc(0),
                ILMatches.Ldfld("card"),
                ILMatches.Call("SendCardToDiscard")
            )
            .Anchors()
            .PointerMatcher(anchor)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldloc_0),
                stuff,
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatches), nameof(SendToDeckInstead))),
                new(OpCodes.Brtrue, branchTarget.Value),
            })
            .AllElements();
    }

    private static bool SendToDeckInstead(Combat c, State s, Card card)
    {
        foreach (Artifact item in s.EnumerateAllArtifacts())
        {
            if (item is ReturnToSenderArtifact artifact) {
                s.SendCardToDeck(card, true, true);
                return true;
            }
        }
        return false;
    }

    private static void Combat_SendCardToExhaust_Postfix(Combat __instance, State s, Card card) {
        foreach (Artifact item in s.EnumerateAllArtifacts()) {
            if (item is IOnExhaustArtifact artifact)  {
                artifact.OnExhaustCard(s, __instance, card);
            }     
        }
    }

    private static void Combat_SendCardToHand_Postfix(Combat __instance, State s, Card card) {
        if(card.Key() == nanobotsCard) {
            foreach (Artifact item in s.EnumerateAllArtifacts()) {
                if (item is LittleHelpers artifact) {
                    __instance.QueueImmediate([
                        new AStatus {
                            status = Status.shield,
                            statusAmount = 1,
                            targetPlayer = true
                        },
                        new AStatus {
                            status = Status.tempShield,
                            statusAmount = 1,
                            targetPlayer = true
                        }
                    ]);
                    artifact.Pulse();
                }
            }
        }
    }
}
