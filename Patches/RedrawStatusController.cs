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

	internal class ShredderHook : IOnRedrawHook
	{
		public void OnRedraw(Card card, State state, Combat combat)
		{
            foreach (Artifact item in state.EnumerateAllArtifacts())
            {
                if (item is ShredderArtifact artifact) {
                    card.ExhaustFX();
                    combat.hand.Remove(card);
                    combat.SendCardToExhaust(state, card);
                    combat.QueueImmediate(new ADelay());

                    Audio.Play(Event.CardHandling);
                    break;
                }
            }
		}
	}

	public static void Apply()
    {
        Instance.PhilipApi.RegisterOnRedrawHook(new ShredderHook(), 3);
    }   
}