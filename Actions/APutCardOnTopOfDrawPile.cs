using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class APutCardOnTopOfDrawPile : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
        if (selectedCard != null) {
			s.RemoveCardFromWhereverItIs(selectedCard.uuid);
		    s.SendCardToDeck(selectedCard, true);
		}
	}
}