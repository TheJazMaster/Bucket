using System.Collections.Generic;
using TheJazMaster.Bucket.Features;

#nullable enable
namespace TheJazMaster.Bucket.Actions;

public class AMakeFavorite : CardAction
{
	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
        if (selectedCard != null) {
            FavoriteManager.SetFavorite(selectedCard, true);
            
            return new CustomShowCards
			{
				message = ModEntry.Instance.Localizations.Localize(["action", "makeFavorite", "showCardText"]),
				cardIds = [selectedCard.uuid]
			};
        }
        return null;
	}	

    public override Icon? GetIcon(State s) {
        return new Icon(ModEntry.Instance.MyFavoriteIcon.Sprite, null, Colors.textMain);
    }

	public override List<Tooltip> GetTooltips(State s)
	{
        return [
            new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.cardtrait,
                () => ModEntry.Instance.MyFavoriteIcon.Sprite,
                () => ModEntry.Instance.Localizations.Localize(["trait", "favorite", "name"]),
                () => ModEntry.Instance.Localizations.Localize(["trait", "favorite", "description"]),
				key: "trait.favorite"
            )
        ];
	}

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "makeFavorite", "cardSelectText"]);
	}
}