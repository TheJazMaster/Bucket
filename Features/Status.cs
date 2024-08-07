using System;
using System.Collections.Generic;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using TheJazMaster.Bucket.Artifacts;
using Microsoft.Extensions.Logging;
using TheJazMaster.Bucket.Actions;

namespace TheJazMaster.Bucket.Features;
#nullable enable

public class StatusManager : IStatusLogicHook, IStatusRenderHook
{
    private static ModEntry Instance => ModEntry.Instance;

    public StatusManager()
    {
        Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
        Instance.KokoroApi.RegisterStatusRenderHook(this, 0);

        // ModEntry.Instance.Harmony.TryPatch(
		//     logger: ModEntry.Instance.Logger,
		//     original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.SendCardToHand)),
		// 	postfix: new HarmonyMethod(GetType(), nameof(Combat_SendCardToHand_Postfix))
		// );
    }

    // private static void Combat_SendCardToHand_Postfix(Combat __instance, State s, Card card, int? position)
    // {
    //     if (__instance.hand.Contains(card)) {
    //         foreach (Artifact item in s.EnumerateAllArtifacts())
	// 		{
    //             if (item is IOnDrawSpecificCardArtifact artifact)
	// 			    artifact.OnDrawSpecificCard(s, __instance, card);
	// 		}

    //         if (card.GetMeta().deck == Deck.trash) {
    //             var amount = s.ship.Get(ModEntry.Instance.SalvageStatus.Status);
    //             if (amount > 0)
    //                 __instance.Queue(new ASalvage {
    //                     amount = amount
    //                 });
    //         }
    //     }
    // }

    private static bool HandleIngenuity(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
    {
        if (status != Instance.IngenuityStatus.Status)
			return false;
		if (timing != StatusTurnTriggerTiming.TurnStart)
			return false;

		if (amount > 0) {
            combat.QueueImmediate(new ADrawCard {
                count = amount,
                statusPulse = status
            });
        }
        return false;
    }

    private static bool HandleSalvage(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
    {
        if (status != Instance.SalvageStatus.Status)
			return false;
		if (timing != StatusTurnTriggerTiming.TurnStart)
			return false;

		if (amount > 0) {
            combat.Queue(new ASalvage {
                amount = amount
            });
        }
        return false;
    }

    private static bool HandleSteamCover(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
    {
        if (status != Instance.SteamCoverStatus.Status)
			return false;
		if (timing != StatusTurnTriggerTiming.TurnEnd)
			return false;

		if (amount > 0) {
            combat.QueueImmediate(new AAddCard {
                card = new TrashFumes(),
                amount = amount,
                destination = CardDestination.Deck,
                statusPulse = status
            });
        }
        return false;
    }

    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
	{
		return HandleSteamCover(state, combat, timing, ship, status, ref amount, ref setStrategy)
            || HandleIngenuity(state, combat, timing, ship, status, ref amount, ref setStrategy)
            || HandleSalvage(state, combat, timing, ship, status, ref amount, ref setStrategy);
	}
    
    public List<Tooltip> OverrideStatusTooltips(Status status, int amount, Ship? ship, List<Tooltip> tooltips) {
		if (status == Instance.SteamCoverStatus.Status) return [
            ..tooltips,
            new TTCard {
                card = new TrashFumes()
            }
        ];
        if (status == Instance.SalvageStatus.Status) return [
            ..tooltips,
            ..StatusMeta.GetTooltips(Status.tempShield, 1)
        ];
        return tooltips;
    }
}