using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheJazMaster.Bucket.Artifacts;
using TheJazMaster.Bucket.Cards;
using TheJazMaster.Bucket.Features;
using TheJazMaster.Bucket.Patches;

#nullable enable
namespace TheJazMaster.Bucket;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	internal IPhilipAPI? PhilipApi { get; }
	internal ITyAndSashaApi? TyApi { get; }
	internal IKokoroApi KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; } = null!;

    internal IDuoArtifactsApi? DuoArtifactsApi { get; private set; } = null!;

	internal HandCountManager HandCountManager { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPlayableCharacterEntryV2 BucketCharacter { get; }

    internal IDeckEntry BucketDeck { get; }

	internal IStatusEntry SalvageStatus { get; }
	internal IStatusEntry IngenuityStatus { get; }
    internal IStatusEntry SteamCoverStatus { get; }
	internal Status RedrawStatus { get; }

    internal ISpriteEntry BucketPortrait { get; }
    internal ISpriteEntry BucketPortraitMini { get; }
    internal ISpriteEntry BucketFrame { get; }
    internal ISpriteEntry BucketCardBorder { get; }

	internal List<ISpriteEntry> NeutralFrames { get; } = [];
	internal List<ISpriteEntry> SquintFrames { get; } = [];
	internal List<ISpriteEntry> GameoverFrames { get; } = [];

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
		typeof(BucketExeCard)
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
		typeof(RecyclingBinArtifact)
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(FavoritismArtifact),
		typeof(ReturnToSenderArtifact),
		typeof(ShredderArtifact),
	];

	internal static IReadOnlyList<Type> DuoArtifacts { get; } = [
		typeof(JuryRigging),
		typeof(CerebralShield),
		typeof(Ruminating),
		typeof(AirlockEjector),
		typeof(WizbosCurse),
		typeof(ProcessorChip),
		typeof(DiamondInTheRough),
		typeof(ScavengerSight),
		typeof(LittleHelpers),
		typeof(ModusOperandi),
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts).Concat(DuoArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");
		DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
		PhilipApi = helper.ModRegistry.GetApi<IPhilipAPI>("clay.PhilipTheMechanic");
		TyApi = helper.ModRegistry.GetApi<ITyAndSashaApi>("TheJazMaster.TyAndSasha");

		RedrawStatus = KokoroApi.RedrawVanillaStatus;

		HandCountManager = new HandCountManager();
		_ = new StatusManager();
		_ = new FavoriteManager();
		CardPatches.Apply();
		CardBrowsePatches.Apply();
		CombatPatches.Apply();
		CardRewardPatches.Apply();
		RedrawStatusControllerPatches.Apply();
		AStatusPatches.Apply();
		AEndTurnPatches.Apply();
		CustomTTGlossary.ApplyPatches(Harmony);
		DynamicWidthCardAction.ApplyPatches(Harmony);

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		NeutralFrames = RegisterTalkSprites("Neutral");
		SquintFrames = RegisterTalkSprites("Squint");
		GameoverFrames = RegisterTalkSprites("Gameover");

        BucketPortrait = NeutralFrames[0];
        BucketPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Bucket_mini.png"));
		BucketFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Panel.png"));
        BucketCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/CardBorder.png"));

        IngenuityStatus = helper.Content.Statuses.RegisterStatus("Ingenuity", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Ingenuity.png")).Sprite,
				color = new("FFED82"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Ingenuity", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Ingenuity", "description"]).Localize
		});

        SalvageStatus = helper.Content.Statuses.RegisterStatus("Salvage", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Salvage.png")).Sprite,
				color = new("3D3D3D"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Salvage", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Salvage", "description"]).Localize
		});

        SteamCoverStatus = helper.Content.Statuses.RegisterStatus("SteamCover", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/SteamCover.png")).Sprite,
				color = new("CACACA"),
				isGood = true
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

        BucketCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Bucket", new()
		{
			Deck = BucketDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = BucketFrame.Sprite,
			Starters = new StarterDeck {
				cards = [ new TrickShotCard(), new OverhaulCard() ]
			},
			ExeCardType = typeof(BucketExeCard),
			NeutralAnimation = new()
			{
				CharacterType = BucketDeck.Deck.Key(),
				LoopTag = "neutral",
				Frames = NeutralFrames.Select(entry => entry.Sprite).ToList()
			},
			MiniAnimation = new()
			{
				CharacterType = BucketDeck.Deck.Key(),
				LoopTag = "mini",
				Frames = [
					BucketPortraitMini.Sprite
				]
			}
		});

		MoreDifficultiesApi?.RegisterAltStarters(BucketDeck.Deck, new StarterDeck {
            cards = {
                new ReboundShotCard(),
                new LearningAlgorithmCard()
            }
        });

		helper.Content.Characters.V2.RegisterCharacterAnimation("GameOver", new()
		{
			CharacterType = BucketDeck.Deck.Key(),
			LoopTag = "gameover",
			Frames = GameoverFrames.Select(entry => entry.Sprite).ToList()
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation("Squint", new()
		{
			CharacterType = BucketDeck.Deck.Key(),
			LoopTag = "squint",
			Frames = SquintFrames.Select(entry => entry.Sprite).ToList()
		});
    }

	public override object? GetApi(IModManifest requestingMod)
		=> new ApiImplementation();

	private static List<ISpriteEntry> RegisterTalkSprites(string fileSuffix)
    {
        var files = Instance.Package.PackageRoot.GetRelative($"Sprites/Character/{fileSuffix}").AsDirectory?.GetFilesRecursively().Where(f => f.Name.EndsWith(".png"));
		List<ISpriteEntry> sprites = [];
		if (files != null) {
			foreach (IFileInfo file in files) {
				sprites.Add(Instance.Helper.Content.Sprites.RegisterSprite(file));
			}
		}
		return sprites;
    }
}