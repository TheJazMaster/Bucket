using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class ADoohicky : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
		foreach (Card card in c.hand) {
			if (card.GetMeta().deck == Deck.trash) {
				c.QueueImmediate(new ADrawCard {
					count = 1
				});
				return;
			}
		}
	}
	
    public override List<Tooltip> GetTooltips(State s) => 
        [new TTGlossary("action.drawCard", 1)];
}