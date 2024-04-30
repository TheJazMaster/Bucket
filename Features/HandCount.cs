using System.Linq;

namespace TheJazMaster.Bucket.Features;

public class HandCountManager
{
    public HandCountManager() {}

    bool busy = false;
    int last = 0;
    public int CountTrashInHand(State s, Combat c)
    {
        return c.hand.Where(card => card.GetMeta().deck == Deck.trash).ToList().Count;
    }

    public int CountRecycleInHand(State s, Combat c)
    {
        if (busy) return last;
        busy = true;
        int ret = c.hand.Where(card => card.GetDataWithOverrides(s).recycle).ToList().Count;
        last = ret;
        busy = false;
        return ret;
    }
}