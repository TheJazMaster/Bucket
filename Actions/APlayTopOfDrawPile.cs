using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class APlayTopOfDrawPile : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
        Card? card = s.deck.Count > 0 ? s.deck[^1] : null;
        if (card != null)
		    ModEntry.Instance.KokoroApi.Actions.MakePlaySpecificCardFromAnywhere(card.uuid).Begin(g, s, c);
	}

    public override Icon? GetIcon(State s)
    {
        return new Icon(StableSpr.icons_bypass, null, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s) => 
        [new TTGlossary("action.bypass")];
}