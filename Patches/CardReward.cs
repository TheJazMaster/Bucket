using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using TheJazMaster.Bucket.Artifacts;

namespace TheJazMaster.Bucket.Patches;

public class CardRewardPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(CardReward), nameof(CardReward.GetOffering)),
			postfix: new HarmonyMethod(typeof(CardRewardPatches), nameof(CardRewardPatches_GetOffering_Postfix))
		);
    }

    static readonly List<Card> basicCards = [new CannonColorless(), new BasicShieldColorless(), new DodgeColorless()];
    private static void CardRewardPatches_GetOffering_Postfix(List<Card> __result, State s, int count, Deck? limitDeck = null, BattleType battleType = BattleType.Normal, Rarity? rarityOverride = null, bool? overrideUpgradeChances = null, bool makeAllCardsTemporary = false, bool inCombat = false, int discount = 0, bool isEvent = false)
    {
        if (limitDeck != null || inCombat || isEvent) return;
        foreach (Artifact item in s.EnumerateAllArtifacts()) {
            if (item is ScavengerSight) {
                double chance = 1.0 / (s.characters.Count + 1);
                for (int i = 0; i < __result.Count; i++) {
                    if (s.rngCardOfferings.Next() < chance) {
                        List<Card> list = (s.rngCardOfferings.Next() < 0.5) ? DB.releasedCards.Where(delegate(Card c)
                        {
                            CardMeta meta = c.GetMeta();
                            if (meta.deck != Deck.trash)
                            {
                                return false;
                            }
                            if (meta.unreleased)
                            {
                                return false;
                            }
                            return true;
                        }).ToList() : basicCards;
                        Card card = (Card)Activator.CreateInstance(list.Random(s.rngCardOfferings).GetType());
                        if (makeAllCardsTemporary) card.temporaryOverride = true;
                        if (discount > 0) card.discount = discount;
                        card.upgrade = CardReward.GetUpgrade(s.rngCardOfferings, s.map, card, (s.GetDifficulty() >= 1) ? 0.5 : 1.0, overrideUpgradeChances);
                        card.flipAnim = 1.0;
                        card.drawAnim = 1.0;
                        card.temporaryOverride = false;
                        __result[i] = card;
                    }
                }
                break;
            }
        }
    }
}
