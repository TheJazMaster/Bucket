// AExhaustEntireHand
using System.Collections.Generic;

namespace TheJazMaster.Bucket.Actions;

public class AExhaustTrashFromHand : DynamicWidthCardAction
{
	public override void Begin(G g, State s, Combat c)
	{
		timer = 0.0;
		foreach (Card item in c.hand)
		{
            if (item.GetMeta().deck == Deck.trash) {
                c.Queue(new AExhaustOtherCard
                {
                    uuid = item.uuid
                });
            }
		}
	}

	public override List<Tooltip> GetTooltips(State s)
	{
		return [
            new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.action,
                () => ModEntry.Instance.ExhaustTrashFromHandIcon.Sprite,
                () => ModEntry.Instance.Localizations.Localize(["action", "exhaustTrashFromHand", "name"]),
                () => ModEntry.Instance.Localizations.Localize(["action", "exhaustTrashFromHand", "description"])
            )
        ];
	}

	public override Icon? GetIcon(State s)
	{
		return new Icon(ModEntry.Instance.ExhaustTrashFromHandIcon.Sprite, null, Colors.textMain);
	}
}
