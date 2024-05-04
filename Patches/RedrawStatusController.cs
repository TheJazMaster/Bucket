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

	internal class ShredderHook : IRedrawStatusHook
	{
		public bool DoRedraw(State state, Combat combat, Card card, IRedrawStatusHook possibilityHook, IRedrawStatusHook paymentHook)
		{
            foreach (Artifact item in state.EnumerateAllArtifacts())
            {
                if (item is ShredderArtifact artifact) {
                    card.ExhaustFX();
                    combat.hand.Remove(card);
                    combat.SendCardToExhaust(state, card);
                    combat.QueueImmediate(new ADelay());

                    Audio.Play(Event.CardHandling);
                    return true;
                }
            }
            return false;
		}
	}

	public static void Apply()
    {
        Instance.KokoroApi.RegisterRedrawStatusHook(new ShredderHook(), 3);
    }   
}