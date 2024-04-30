namespace TheJazMaster.Bucket.Artifacts;

public interface IOnExhaustArtifact {
    void OnExhaustCard(State s, Combat c, Card card);
}
