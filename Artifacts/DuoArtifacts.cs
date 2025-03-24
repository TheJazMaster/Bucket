using System;
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

internal sealed class JuryRigging : Artifact, IBucketArtifact, IOnExhaustArtifact
{
	public int counter = 0;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("JuryRigging", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/JuryRigging.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "JuryRigging", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "JuryRigging", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<JuryRigging>([Deck.peri, ModEntry.Instance.BucketDeck.Deck]);
	}

	public void OnExhaustCard(State s, Combat c, Card card) {
		if (card.GetMeta().deck == Deck.trash) {
			counter += 1;
			if (counter >= 2) {
				c.Queue(new AAddCard {
					card = new ChipShot(),
					amount = 1,
					destination = CardDestination.Hand,
				});
				counter -= 2;
			Pulse();
			}
		}
	}

	public override int? GetDisplayNumber(State s)
	{
		return counter;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTCard {
			card = new ChipShot()
		}
	];
}


internal sealed class CerebralShield : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("CerebralShield", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/CerebralShield.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "CerebralShield", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "CerebralShield", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<CerebralShield>([Deck.dizzy, ModEntry.Instance.BucketDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		int redrawAmount = state.ship.Get(ModEntry.Instance.RedrawStatus);
		if (redrawAmount / 3 > 0) {
			combat.Queue(new AStatus {
				status = Status.shield,
				statusAmount = redrawAmount / 3,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return [.. StatusMeta.GetTooltips(Status.shield, 1), .. StatusMeta.GetTooltips(ModEntry.Instance.RedrawStatus, 3)];
	}
}


internal sealed class Ruminating : Artifact, IBucketArtifact, ICardActionAffectorArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("Ruminating", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/Ruminating.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "Ruminating", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "Ruminating", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<Ruminating>([Deck.riggs, ModEntry.Instance.BucketDeck.Deck]);
	}

	public void AffectCardActions(State s, Combat c, Card card, List<CardAction> actions) {
		for (int i = 0; i < actions.Count; i++) {
			CardAction action = actions[i];
			if (action is ADrawCard drawAction) {
				actions[i] = new AStatus {
					status = ModEntry.Instance.RedrawStatus,
					statusAmount = drawAction.count,
					targetPlayer = true
				};
			}
		}
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		StatusMeta.GetTooltips(ModEntry.Instance.RedrawStatus, 1);
}


internal sealed class AirlockEjector : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("AirlockEjector", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/AirlockEjector.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "AirlockEjector", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "AirlockEjector", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<AirlockEjector>([Deck.goat, ModEntry.Instance.BucketDeck.Deck]);
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (deck == Deck.trash) {
			combat.QueueImmediate(new ASpawn {
				thing = new Asteroid {
					yAnimation = 0,
				},
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("midrow.asteroid")
	];
}


internal sealed class WizbosCurse : Artifact, IBucketArtifact, IAfterStatusActionArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("WizbosCurse", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/WizbosCurse.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "WizbosCurse", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "WizbosCurse", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<WizbosCurse>([Deck.eunice, ModEntry.Instance.BucketDeck.Deck]);
	}

	public void OnAfterStatusAction(State s, Combat c, AStatus action, int difference)
	{
		if (action.status == Status.heat && action.whoDidThis.HasValue && action.whoDidThis != Deck.trash && difference > 0) {
			c.QueueImmediate(new AAddCard {
				card = new TrashMiasma(),
				amount = difference,
				destination = CardDestination.Hand,
				artifactPulse = Key()
			});
			s.ship.Add(Status.heat, -difference);
		}
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return [
			.. StatusMeta.GetTooltips(Status.heat, 1),
			new TTCard {
				card = new TrashMiasma()
			},
		];
	}
}


internal sealed class ProcessorChip : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("ProcessorChip", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/ProcessorChip.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "ProcessorChip", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "ProcessorChip", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<ProcessorChip>([Deck.hacker, ModEntry.Instance.BucketDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 1) {
			combat.QueueImmediate(new AAddCard {
				card = new SmartSteeringCard {
					exhaustOverride = true,
					exhaustOverrideIsPermanent = true,
					upgrade = Upgrade.A
				},
				amount = 1,
				destination = CardDestination.Hand
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return [
			new TTCard {
				card = new SmartSteeringCard {
					exhaustOverride = true,
					exhaustOverrideIsPermanent = true,
					upgrade = Upgrade.A
				}
			},
		];
	}
}


internal sealed class DiamondInTheRough : Artifact, IBucketArtifact, IBeforeTurnEndArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("DiamondInTheRough", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/DiamondInTheRough.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "DiamondInTheRough", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "DiamondInTheRough", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<DiamondInTheRough>([Deck.shard, ModEntry.Instance.BucketDeck.Deck]);
	}

	public void BeforeTurnEnd(G g, State state, Combat combat)
	{
		int trashInHand = ModEntry.Instance.HandCountManager.CountTrashInHand(state, combat);
		if (trashInHand > 0) {
			combat.QueueImmediate(new AStatus {
				status = Status.shard,
				statusAmount = trashInHand,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		StatusMeta.GetTooltips(Status.shard, 1);
}


internal sealed class ScavengerSight : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("ScavengerSight", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/ScavengerSight.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "ScavengerSight", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "ScavengerSight", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<ScavengerSight>([Deck.catartifact, ModEntry.Instance.BucketDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		int amount = state.deck.Where(card => card.GetMeta().deck == Deck.trash).Count();
		combat.Queue(new AStatus {
			status = Status.evade,
			statusAmount = amount,
			targetPlayer = true
		});
		combat.Queue(new AStatus {
			status = Status.shield,
			statusAmount = amount,
			targetPlayer = true
		});
	}
}


internal sealed class ModusOperandi : Artifact, IBucketArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.TyApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("ModusOperandi", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/ModusOperandi.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "ModusOperandi", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "ModusOperandi", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<ModusOperandi>([ModEntry.Instance.TyApi.TyDeck, ModEntry.Instance.BucketDeck.Deck]);
	}

	public override void OnReceiveArtifact(State state)
	{
		for (int i = 0; i < 2; i++) {
			state.GetCurrentQueue().QueueImmediate(
				new ACardSelect
				{
					browseAction = new CardSelectAddBuoyantForever(),
					browseSource = CardBrowse.Source.Deck,
					filterBuoyant = false
				}
			);
		}
	}


	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("cardtrait.buoyant")
	];
}


internal sealed class LittleHelpers : Artifact, IBucketArtifact
{
	static Type? nanobotsCardType = null;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.PhilipApi == null) return;
		
		nanobotsCardType = helper.Content.Cards.LookupByUniqueName("clay.PhilipTheMechanic::Nanobots")?.Configuration.CardType;
		helper.Content.Artifacts.RegisterArtifact("LittleHelpers", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/LittleHelpers.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "LittleHelpers", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "LittleHelpers", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<LittleHelpers>([ModEntry.Instance.PhilipApi.PhilipDeck.Deck, ModEntry.Instance.BucketDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (nanobotsCardType != null) {
			if (combat.turn == 1) {
				combat.QueueImmediate(new AAddCard {
					card = (Card)Activator.CreateInstance(nanobotsCardType)!,
					amount = 1,
					destination = CardDestination.Deck
				});
			}
		}
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		if (nanobotsCardType != null) {
			Card card = (Card)Activator.CreateInstance(nanobotsCardType)!;
			return [
				new TTCard {
					card = card
				},
			];
		}
		return [];
	}
}