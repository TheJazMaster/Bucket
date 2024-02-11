using Nickel;

namespace TheJazMaster.Bucket;

internal interface IBucketCard
{
	static abstract void Register(IModHelper helper);
}

internal interface IBucketArtifact
{
	static abstract void Register(IModHelper helper);
}