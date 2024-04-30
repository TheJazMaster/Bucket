using System.Collections.Generic;

namespace TheJazMaster.Bucket.Artifacts;

public interface IAfterStatusActionArtifact
{
	void OnAfterStatusAction(State s, Combat c, AStatus action, int difference);
}