using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class ADiscardAndFeedToAttack : CardAction
{
    public bool? shootOnTrash;
    public int shootDamageAmount = 0;


	public override void Begin(G g, State s, Combat c)
	{
        if (selectedCard != null) {
            s.RemoveCardFromWhereverItIs(selectedCard.uuid);
            c.SendCardToDiscard(s, selectedCard);
            
            if (shootOnTrash.HasValue) {
                if ((selectedCard.GetMeta().deck == Deck.trash) == shootOnTrash)
                    c.QueueImmediate(new AAttack {
                        damage = shootDamageAmount
                    });
            }
        }
	}	

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "discardAndFeedToAttack", "cardSelectText", shootOnTrash.HasValue ? (shootOnTrash.Value ? "trash" : "nontrash") : "default"], new { Damage = shootDamageAmount });
	}
}