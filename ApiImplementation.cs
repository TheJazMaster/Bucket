using Nickel;

#nullable enable
namespace TheJazMaster.Bucket;

public sealed class ApiImplementation : IBucketApi
{
	readonly ModEntry Instance = ModEntry.Instance;
	public Deck BucketDeck => Instance.BucketDeck.Deck;
	public Status IngenuityStatus => Instance.IngenuityStatus.Status;
	public Status SalvageStatus => Instance.SalvageStatus.Status;
	public Status SteamCoverStatus => Instance.SteamCoverStatus.Status;
}
