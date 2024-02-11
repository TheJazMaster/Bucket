using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FSPRO;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using TheJazMaster.Bucket.Artifacts;

namespace TheJazMaster.Bucket.Patches;

public class RedrawStatusControllerPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        try {
            var originalMethod = AccessTools.DeclaredMethod(AccessTools.TypeByName("PhilipTheMechanic.RedrawStatusController"), "DiscardFromHand");
            if (originalMethod is null)
            {
                throw new Exception("Could not patch RedrawStatusController.DiscardFromHand");
            }

            Harmony.Patch(
                original: originalMethod,
                prefix: new HarmonyMethod(typeof(RedrawStatusControllerPatches), nameof(RedrawStatusController_DiscardFromHand_Prefix))
            );
        } catch (Exception e) {
            Instance.Logger.LogError(e.StackTrace);
        }
    }   

    private static bool RedrawStatusController_DiscardFromHand_Prefix(State s, Card card, bool silent = false)
    {
        if (s.route is Combat c) {
            foreach (Artifact item in s.EnumerateAllArtifacts())
            {
                if (item is ShredderArtifact artifact) {
                    card.ExhaustFX();
                    c.hand.Remove(card);
                    c.SendCardToExhaust(s, card);
                    c.QueueImmediate(new ADelay());

                    if (!silent)
                    {
                        Audio.Play(Event.CardHandling);
                    }
                    return false;
                }
            }
        }
        return true;
    }
}