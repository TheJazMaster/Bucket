using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class APlayAnyInDrawPile : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
        if (selectedCard != null)
		    ModEntry.Instance.KokoroApi.Actions.MakePlaySpecificCardFromAnywhere(selectedCard.uuid).Begin(g, s, c);
	}

    public override Icon? GetIcon(State s)
    {
        return new Icon(StableSpr.icons_bypass, null, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s) => 
        [new TTGlossary("action.bypass")];
}