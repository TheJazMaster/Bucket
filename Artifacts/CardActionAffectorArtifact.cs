using System.Collections.Generic;

namespace TheJazMaster.Bucket.Artifacts;

public interface ICardActionAffectorArtifact
{
	void AffectCardActions(State s, Combat c, Card card, List<CardAction> actions);
}