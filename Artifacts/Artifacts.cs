using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Nickel;
using TheJazMaster.Bucket.Actions;
using TheJazMaster.Bucket.Cards;
using TheJazMaster.Bucket.Features;

#nullable enable
namespace TheJazMaster.Bucket.Artifacts;

internal sealed class XRayVisionArtifact : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("XRayVision", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/XRayVision.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "XRayVision", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "XRayVision", "description"]).Localize
		});
	}
}


internal sealed class DoohickyArtifact : Artifact, IBucketArtifact
{
	bool active = true;

	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Doohicky", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/Doohicky.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Doohicky", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Doohicky", "description"]).Localize
		});
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (active) {
			combat.QueueImmediate(new ADoohicky {
				artifactPulse = Key()
			});
		}
	}
}


internal sealed class WorkaroundArtifact : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Workaround", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/Workaround.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Workaround", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Workaround", "description"]).Localize
		});
	}

	public static int GetNumber()
	{
		State s = MG.inst.g.state;
		Combat? c = (s.route is Combat combat) ? combat : null;
		if (c == null) return s.deck.Count / 4;
		return s.deck.Concat(c.hand.Concat(c.exhausted.Concat(c.discard))).Count() / 4;
	}

	public override int? GetDisplayNumber(State s)
	{
		if (s.route is Combat) return null;
		return GetNumber();
	}
	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 1)
		{
			combat.QueueImmediate(new AStatus
			{
				status = ModEntry.Instance.RedrawStatus,
				statusAmount = GetNumber(),
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return StatusMeta.GetTooltips(ModEntry.Instance.RedrawStatus, 1);
	}
}


internal sealed class RecyclingBinArtifact : Artifact, IBucketArtifact
{
	bool active = true;
	internal static Spr ActiveSprite;
	internal static Spr InactiveSprite;

	public static void Register(IModHelper helper)
	{
		ActiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/RecyclingBin.png")).Sprite;
		InactiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/RecyclingBinInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("RecyclingBin", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = ActiveSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "RecyclingBin", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "RecyclingBin", "description"]).Localize
		});
	}

	public override Spr GetSprite()
	{
		return active ? ActiveSprite : InactiveSprite;
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (active && card.GetDataWithOverrides(state).recycle) {
			active = false;
			combat.Queue(new AStatus {
				status = Status.drawNextTurn,
				statusAmount = 1,
				targetPlayer = true
			});
			Pulse();
		}
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		active = true;
	}

	public override void OnCombatEnd(State state)
	{
		active = true;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.drawNextTurn, 1),
		new TTGlossary("cardtrait.recycle")
	];
}


internal sealed class ReturnToSenderArtifact : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ReturnToSender", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/ReturnToSender.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ReturnToSender", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ReturnToSender", "description"]).Localize
		});
	}
}


internal sealed class FavoritismArtifact : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Favoritism", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/Favoritism.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Favoritism", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Favoritism", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(
			new ACardSelect
			{
				browseAction = new AMakeFavorite(),
				browseSource = CardBrowse.Source.Deck
			}
		);
	}

	public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
	{
		if (card == null) return 0;

		List<Card> empty = [];
		var favorites = state.deck.Concat(combat?.hand ?? empty).Concat(combat?.exhausted ?? empty).Concat(combat?.discard ?? empty).Where(FavoriteManager.IsFavorite).ToList();
		if (favorites.Count > 0) {
			if (favorites.Contains(card)) {
				return 2;
			} else {
				return -1;
			}
		}
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return [
            // new CustomTTGlossary(
            //     CustomTTGlossary.GlossaryType.action,
            //     () => ModEntry.Instance.MyFavoriteIcon.Sprite,
            //     () => ModEntry.Instance.Localizations.Localize(["action", "makeFavorite", "name"]),
            //     () => ModEntry.Instance.Localizations.Localize(["action", "makeFavorite", "description"]),
			// 	key: "action.makeFavorite"
            // ),
            new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.cardtrait,
                () => ModEntry.Instance.MyFavoriteIcon.Sprite,
                () => ModEntry.Instance.Localizations.Localize(["trait", "favorite", "name"]),
                () => ModEntry.Instance.Localizations.Localize(["trait", "favorite", "description"]),
				key: "trait.myfavorite"
            )
		];
	}
}


internal sealed class ShredderArtifact : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Shredder", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.BucketDeck.Deck,
				pools = [ArtifactPool.Boss],
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/Shredder.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Shredder", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Shredder", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.ship.baseDraw -= 1;
	}

	public override void OnRemoveArtifact(State state)
	{
		state.ship.baseDraw += 1;
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return StatusMeta.GetTooltips(ModEntry.Instance.RedrawStatus, 1);
	}
}