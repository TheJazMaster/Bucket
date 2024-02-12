// Total time spent: 400 min

using FMOD;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.Bucket.Artifacts;
using TheJazMaster.Bucket.Cards;
using TheJazMaster.Bucket.Features;
using TheJazMaster.Bucket.Patches;

namespace TheJazMaster.Bucket;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null;

    internal Harmony Harmony { get; }
	internal IKokoroApi KokoroApi { get; }
	internal IMoreDifficultiesApi MoreDifficultiesApi { get; }

	internal HandCountManager HandCountManager { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal ICharacterEntry BucketCharacter { get; }

    internal IDeckEntry BucketDeck { get; }

	internal IStatusEntry SalvageStatus { get; }
	internal IStatusEntry IngenuityStatus { get; }
    internal IStatusEntry SteamCoverStatus { get; }
	internal IStatusEntry RedrawStatus { get; }

    internal ISpriteEntry BucketPortrait { get; }
    internal ISpriteEntry BucketPortraitMini { get; }
    internal ISpriteEntry BucketFrame { get; }
    internal ISpriteEntry BucketCardBorder { get; }

    internal ISpriteEntry ExhaustTrashFromHandIcon { get; }
	internal ISpriteEntry TrashHandIcon { get; }
	internal ISpriteEntry RecycleHandIcon { get; }
	internal ISpriteEntry MyFavoriteIcon { get; }
	internal ISpriteEntry AddMyFavoriteIcon { get; }

    internal static IReadOnlyList<Type> StarterCardTypes { get; } = [
		typeof(TrickShotCard),
		typeof(OverhaulCard),
	];

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(ReboundShotCard),
		typeof(LearningAlgorithmCard),
		typeof(CrossWiresCard),
		typeof(TinkerCard),
		typeof(GarbageChuteCard),
		typeof(TriggerHappyCard),
        typeof(EurekaCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(OxiFuelCellCard),
		typeof(GreenEnergyCard),
		typeof(FeedbackLoopCard),
		typeof(FineTuneCard),
		typeof(ControlledChaosCard),
		typeof(ContingencyPlanCard),
		typeof(IllegalModsCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(AdvancedAICard),
		typeof(TrashCannonCard),
		typeof(SalvageCard),
		typeof(IngenuityCard),
		typeof(VaporizorCard),
	];


	internal static IReadOnlyList<Type> SecretCardTypes { get; } = [
		typeof(PriceOfProgressCard),
		typeof(SmartSteeringCard),
	];

    internal static IEnumerable<Type> AllCardTypes
		=> StarterCardTypes
			.Concat(CommonCardTypes)
			.Concat(UncommonCardTypes)
			.Concat(RareCardTypes)
			.Concat(SecretCardTypes);

    internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(XRayVisionArtifact),
		typeof(DoohickyArtifact),
		typeof(WorkaroundArtifact),
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(FavoritismArtifact),
		typeof(ReturnToSenderArtifact),
		typeof(ShredderArtifact),
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties")!;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;

		RedrawStatus = helper.Content.Statuses.LookupByUniqueName("clay.PhilipTheEngineer::clay.PhilipTheMechanic.Statuses.Redraw") ?? throw new Exception("Failed to get redraw status");

		HandCountManager = new HandCountManager();
		_ = new StatusManager();
		_ = new FavoriteManager();
		CardBrowsePatches.Apply();
		CombatPatches.Apply();
		RedrawStatusControllerPatches.Apply();
		CustomTTGlossary.ApplyPatches(Harmony);
		DynamicWidthCardAction.ApplyPatches(Harmony);

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);


        BucketPortrait = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Bucket_neutral_0.png"));
        BucketPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Bucket_mini.png"));
		BucketFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Panel.png"));
        BucketCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/CardBorder.png"));

        IngenuityStatus = helper.Content.Statuses.RegisterStatus("Ingenuity", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Ingenuity.png")).Sprite,
				color = new("FFED82")
			},
			Name = AnyLocalizations.Bind(["status", "Ingenuity", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Ingenuity", "description"]).Localize
		});

        SalvageStatus = helper.Content.Statuses.RegisterStatus("Salvage", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Salvage.png")).Sprite,
				color = new("3D3D3D")
			},
			Name = AnyLocalizations.Bind(["status", "Salvage", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Salvage", "description"]).Localize
		});

        SteamCoverStatus = helper.Content.Statuses.RegisterStatus("SteamCover", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/SteamCover.png")).Sprite,
				color = new("CACACA")
			},
			Name = AnyLocalizations.Bind(["status", "SteamCover", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "SteamCover", "description"]).Localize
		});


		ExhaustTrashFromHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/ExhaustTrashFromHand.png"));
		TrashHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/TrashHand.png"));
		RecycleHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/RecycleHand.png"));
		MyFavoriteIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/MyFavorite.png"));
		AddMyFavoriteIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/AddMyFavorite.png"));


		BucketDeck = helper.Content.Decks.RegisterDeck("Bucket", new()
		{
			Definition = new() { color = new Color("7D86C2"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = BucketCardBorder.Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

        foreach (var cardType in AllCardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(IBucketCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in AllArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(IBucketCard.Register))?.Invoke(null, [helper]);

		// MoreDifficultiesApi?.RegisterAltStarters(BucketDeck.Deck, new StarterDeck {
        //     cards = {
        //     }
        // });

        BucketCharacter = helper.Content.Characters.RegisterCharacter("Bucket", new()
		{
			Deck = BucketDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = BucketFrame.Sprite,
			Starters = new StarterDeck {
				cards = [ new TrickShotCard(), new OverhaulCard() ]
			},
			NeutralAnimation = new()
			{
				Deck = BucketDeck.Deck,
				LoopTag = "neutral",
				Frames = [
					BucketPortrait.Sprite
				]
			},
			MiniAnimation = new()
			{
				Deck = BucketDeck.Deck,
				LoopTag = "mini",
				Frames = [
					BucketPortraitMini.Sprite
				]
			}
		});

		helper.Content.Characters.RegisterCharacterAnimation("GameOver", new()
		{
			Deck = BucketDeck.Deck,
			LoopTag = "gameover",
			Frames = [BucketPortrait.Sprite]
		});
		helper.Content.Characters.RegisterCharacterAnimation("Squint", new()
		{
			Deck = BucketDeck.Deck,
			LoopTag = "squint",
			Frames = [BucketPortrait.Sprite]
		});
    }
}