using Nickel;

namespace TheJazMaster.Bucket;

public interface IJohnsonApi
{
	IDeckEntry JohnsonDeck { get; }
	IStatusEntry CrunchTimeStatus { get; }
}