using Nickel;
using TheJazMaster.Bucket.Actions;
using System.Collections.Generic;
using System.Reflection;
using TheJazMaster.Bucket.Features;

namespace TheJazMaster.Bucket.Cards;


internal sealed class TrickShotCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("TrickShot", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/TrickShot.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TrickShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack
		{
			damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1)
		},
		new AStatus
		{
			status = ModEntry.Instance.RedrawStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		}
	];
}


internal sealed class OverhaulCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Overhaul", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Overhaul.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Overhaul", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		description = ModEntry.Instance.Localizations.Localize(["card", "Overhaul", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus
		{
			status = Status.shield,
			statusAmount = upgrade == Upgrade.B ? 3 : 2,
			targetPlayer = true
		},
		new AAddCard
		{
			card = new ColorlessTrash(),
			amount = upgrade == Upgrade.A ? 1 : 2,
			destination = CardDestination.Hand
		}
	];
}

internal sealed class ReboundShotCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("ReboundShot", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/ReboundShot.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ReboundShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		recycle = upgrade != Upgrade.B,
		retain = upgrade == Upgrade.B,
		infinite = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
		},
	];
}

internal sealed class LearningAlgorithmCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("LearningAlgorithm", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/LearningAlgorithm.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "LearningAlgorithm", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.B => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.RedrawStatus,
				statusAmount = upgrade == Upgrade.None ? 1 : 2,
				targetPlayer = true
			},
		]
	};
}


internal sealed class CrossWiresCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("CrossWires", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CrossWires.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CrossWires", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		retain = upgrade == Upgrade.B,
		recycle = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus
		{
			status = Status.autopilot,
			statusAmount = 1,
			targetPlayer = true
		},
		new AStatus
		{
			status = Status.tempShield,
			statusAmount = 2,
			targetPlayer = true
		}
	];
}

internal sealed class TinkerCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Tinker", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Tinker.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Tinker", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = ModEntry.Instance.HandCountManager.CountTrashInHand(s, c);
		return upgrade switch
		{
			Upgrade.A => [
				new AVariableHintTrash(),
				new ADrawCard {
					count = amt,
					xHint = 1
				},
				new ADrawCard {
					count = 2
				},
			],
			Upgrade.B => [
				new AVariableHintTrash(),
				new AStatus {
					status = ModEntry.Instance.RedrawStatus,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				}
			],
			_ => [
				new AVariableHintTrash(),
				new ADrawCard {
					count = amt,
					xHint = 1
				}
			]
		};
	}
}

internal sealed class GarbageChuteCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("GarbageChute", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/GarbageChute.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "GarbageChute", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = upgrade != Upgrade.A,
		retain = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AExhaustTrashFromHand()
	];
}

internal sealed class TriggerHappyCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("TriggerHappy", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/TriggerHappy.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TriggerHappy", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", "TriggerHappy", "description", upgrade.ToString()], upgrade == Upgrade.A ? new { Damage = GetDmg(state, 1) } : null),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AAttack {
					damage = GetDmg(s, 1)
				},
				new AAddCard {
					card = new TrashAutoShoot(),
					amount = 1,
					destination = CardDestination.Deck
				}
			],
			Upgrade.B => [
				new AAddCard {
					card = new TrashAutoShoot(),
					amount = 2,
					destination = CardDestination.Deck
				}
			],
			_ => [
				new AAddCard {
					card = new TrashAutoShoot(),
					amount = 1,
					destination = CardDestination.Deck
				}
			]
		};
	}
}

internal sealed class EurekaCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Eureka", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Eureka.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Eureka", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		description = ModEntry.Instance.Localizations.Localize(["card", "Eureka", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.RedrawStatus,
			statusAmount = upgrade == Upgrade.A ? 4 : 3,
			targetPlayer = true
		},
		new AAddCard {
			card = upgrade == Upgrade.B ? new ColorlessTrash() : new PriceOfProgressCard()
		}
	];
}


internal sealed class OxiFuelCellCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("OxiFuelCell", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/OxiFuelCell.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "OxiFuelCell", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		description = ModEntry.Instance.Localizations.Localize(["card", "OxiFuelCell", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AEnergy {
					changeAmount = 3
				},
				new AAddCard {
					card = new OxygenLeak(),
					amount = 1
				}
			],
			Upgrade.B => [
				new AEnergy {
					changeAmount = 2
				},
				new AAddCard {
					card = new TrashFumes(),
					amount = 2
				}
			],
			_ => [
				new AEnergy {
					changeAmount = 2
				},
				new AAddCard {
					card = new OxygenLeak(),
					amount = 1
				}
			]
		};
	}
}


internal sealed class GreenEnergyCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("GreenEnergy", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/GreenEnergy.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "GreenEnergy", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		retain = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = (c == DB.fakeCombat) ? s.ship.baseEnergy : ModEntry.Instance.HandCountManager.CountRecycleInHand(s, c);
		return upgrade switch {
			Upgrade.B => [
				new AVariableHintRecycle(),
				new AEnergy {
					changeAmount = amt,
					xHint = 1
				},
				new ADrawCard {
					count = amt,
					xHint = 1
				},
				new AStatus {
					status = ModEntry.Instance.RedrawStatus,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				}
			],
			_ => [
				new AVariableHintRecycle(),
				new AEnergy {
					changeAmount = amt,
					xHint = 1
				},
				new ADrawCard {
					count = amt,
					xHint = 1
				},
			],
		};
	}
}


internal sealed class FeedbackLoopCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FeedbackLoop", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FeedbackLoop.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FeedbackLoop", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		recycle = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.B => [
			new AStatus {
				status = Status.shield,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				targetPlayer = true
			},
		],
	};
}


internal sealed class FineTuneCard : Card, IBucketCard
{
	internal static Spr Sprite;
	public static void Register(IModHelper helper) {
		Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FineTune.png")).Sprite;
		helper.Content.Cards.RegisterCard("FineTune", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FineTune", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		description = ModEntry.Instance.Localizations.Localize(["card", "FineTune", "description", upgrade.ToString()], new { Damage = GetDmg(state, upgrade == Upgrade.A ? 2 : 1) }),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new ACardSelectImproved {
				browseAction = new ADiscardAndFeedToAttack {
					shootDamageAmount = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
					shootOnTrash = upgrade == Upgrade.B,
				},
				browseSource = CardBrowse.Source.DrawPile
			},
			new ACardSelectImproved {
				browseAction = new ADiscardAndFeedToAttack {
					shootDamageAmount = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
					shootOnTrash = upgrade == Upgrade.B,
				},
				browseSource = CardBrowse.Source.DrawPile
			},
			new ACardSelectImproved {
				browseAction = new ADiscardAndFeedToAttack {
					shootDamageAmount = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
					shootOnTrash = upgrade == Upgrade.B,
				},
				browseSource = CardBrowse.Source.DrawPile
			}
		];
	}
}


internal sealed class ControlledChaosCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("ControlledChaos", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/ControlledChaos.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ControlledChaos", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.None ? 1 : 0,
		exhaust = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", "ControlledChaos", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ACardSelectImproved
			{
				browseAction = new APlayAnyInDrawPile(),
				browseSource = CardBrowse.Source.DrawPile,
				ignoreCardType = Key()
			}
		],
		_ => [
			new APlayTopOfDrawPile()
		]
	};
}


internal sealed class ContingencyPlanCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("ContingencyPlan", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/ContingencyPlan.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ContingencyPlan", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		retain = upgrade == Upgrade.A,
		// floppable = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", "ContingencyPlan", "description", upgrade.ToString()/* + ((upgrade == Upgrade.B && flipped) ? "alt" : "")*/]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.shield,
				statusAmount = 2,
				targetPlayer = true
			},
			new ACardSelectImproved
			{
				browseAction = new APutCardOnTopOfDrawPile(),
				browseSource = CardBrowse.Source.DrawPile
			}
		],
		_ => [
			new AStatus {
				status = Status.shield,
				statusAmount = 2,
				targetPlayer = true
			},
			new ACardSelectImproved
			{
				browseAction = new APutCardOnTopOfDrawPile(),
				browseSource = CardBrowse.Source.Hand
			}
		],
	};
}


internal sealed class IllegalModsCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("IllegalMods", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/IllegalMods.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "IllegalMods", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		description = ModEntry.Instance.Localizations.Localize(["card", "IllegalMods", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 2,
				targetPlayer = true
			},
			new AAddCard {
				card = new PriceOfProgressCard(),
				amount = 1,
				destination = CardDestination.Discard
			}
		],
		_ => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 2,
				targetPlayer = true
			},
			new AAddCard {
				card = new ColorlessTrash(),
				amount = 2,
				destination = CardDestination.Deck
			}
		],
	};
}


internal sealed class AdvancedAICard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("AdvancedAI", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/AdvancedAI.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "AdvancedAI", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "AdvancedAI", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 3,
				targetPlayer = true
			},
			new AAddCard {
				card = new SmartSteeringCard(),
				amount = 1,
				destination = CardDestination.Hand
			}
		],
		Upgrade.B => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 1,
				targetPlayer = true
			},
			new AAddCard {
				card = new SmartSteeringCard(),
				amount = 2,
				destination = CardDestination.Hand
			}
		],
		_ => [
			new AStatus {
				status = Status.autopilot,
				statusAmount = 1,
				targetPlayer = true
			},
			new AAddCard {
				card = new SmartSteeringCard(),
				amount = 1,
				destination = CardDestination.Hand
			}
		],
	};
}


internal sealed class TrashCannonCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("TrashCannon", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/TrashCannon.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TrashCannon", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		retain = upgrade == Upgrade.A,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = ModEntry.Instance.HandCountManager.CountTrashInHand(s, c);
		return upgrade switch {
			Upgrade.B => [
				new AVariableHintTrash(),
				new AAttack {
					damage = GetDmg(s, 3*amt),
					xHint = 3
				},
				new AExhaustTrashFromHand()
			],
			_ => [
				new AVariableHintTrash(),
				new AAttack {
					damage = GetDmg(s, 3*amt),
					xHint = 3
				},
			]
		};
	}
}


internal sealed class SalvageCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Salvage", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Salvage.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Salvage", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.SalvageStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 4,
				targetPlayer = true
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.SalvageStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class IngenuityCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Ingenuity", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Ingenuity.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Ingenuity", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 1,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.IngenuityStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			},
			new ADrawCard {
				count = 2,
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.IngenuityStatus.Status,
				statusAmount = upgrade == Upgrade.B ? 2 : 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class VaporizorCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Vaporizor", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Vaporizor.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Vaporizor", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		recycle = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 6 : 4),
			piercing = true
		},
		new AStatus {
			status = ModEntry.Instance.SteamCoverStatus.Status,
			statusAmount = 1,
			targetPlayer = true
		},
	];
}


internal sealed class SmartSteeringCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("SmartSteering", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.BucketDeck.Deck,
				rarity = Rarity.rare,
				dontOffer = true,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/SmartSteering.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SmartSteering", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		flippable = true,
		retain = true,
		recycle = true,
		temporary = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "SmartSteering", "description", upgrade.ToString()], new {
			Dir = ModEntry.Instance.Localizations.Localize(["card", "SmartSteering", "description", flipped ? "right" : "left"]),
			InvDir = upgrade == Upgrade.A ? ModEntry.Instance.Localizations.Localize(["card", "SmartSteering", "description", flipped ? "left" : "right"]) : null
		}),
		artTint = "ffffff"
	};

	public override void OnFlip(G g)
	{
		base.OnFlip(g);
		if (g.state.route is not Combat combat)
			return;

		var index = combat.hand.IndexOf(this);
		if (index == -1)
			return;

		combat.hand.Remove(this);
		if (flipped)
			combat.hand.Insert(0, this);
		else
			combat.hand.Insert(combat.hand.Count, this);
	}

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AMove {
				dir = 1,
				targetPlayer = true
			}
		],
		Upgrade.B => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => []
	};
}


internal sealed class PriceOfProgressCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("PriceOfProgress", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = Deck.trash,
				rarity = Rarity.common,
				dontOffer = true
			},
			Art = StableSpr.cards_Trash,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PriceOfProgress", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		temporary = true,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "PriceOfProgress", "description"]),
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAddCard {
			card = new ColorlessTrash(),
			amount = 1,
			destination = CardDestination.Discard
		}
	];
}


internal sealed class BucketExeCard : Card, IBucketCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("BucketExe", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = Deck.colorless,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = FineTuneCard.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BucketExe", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ColorlessLoc.GetDesc(state, upgrade == Upgrade.B ? 3 : 2, ModEntry.Instance.BucketDeck.Deck),
		artTint = "ffffff"
    };

	public override List<CardAction> GetActions(State s, Combat c)
    {
		Deck deck = ModEntry.Instance.BucketDeck.Deck;
		return upgrade switch
		{
			Upgrade.B => [
				new ACardOffering
				{
					amount = 3,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
					dialogueSelector = ".summonBucket"
				}
			],
			_ => [
				new ACardOffering
				{
					amount = 2,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
					dialogueSelector = ".summonBucket"
				}
			],
		};
	}
}

