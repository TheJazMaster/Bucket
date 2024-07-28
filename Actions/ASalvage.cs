using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class ASalvage : CardAction
{
	internal int amount = 0;

	public override void Begin(G g, State s, Combat c)
	{
		int trash = 0;
		foreach (Card card in c.hand) {
            if (card.GetMeta().deck == Deck.trash) {
                trash++;
            }
		}
		c.QueueImmediate(new AStatus {
			status = Status.tempShield,
			statusAmount = amount * trash,
			targetPlayer = true,
			statusPulse = ModEntry.Instance.SalvageStatus.Status
		});
	}
	
    public override List<Tooltip> GetTooltips(State s) => 
        [new TTGlossary("status.tempShield", amount)];
}