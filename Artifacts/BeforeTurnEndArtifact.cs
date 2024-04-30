using System.Collections.Generic;

namespace TheJazMaster.Bucket.Artifacts;

public interface IBeforeTurnEndArtifact
{
	void BeforeTurnEnd(G g, State s, Combat c);
}