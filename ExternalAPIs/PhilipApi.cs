using Nickel;
using System.Collections.Generic;

namespace TheJazMaster.Bucket
{
    public interface IPhilipAPI
    {
        IStatusEntry RedrawStatus { get; }

        void RegisterOnRedrawHook(IOnRedrawHook hook, double priority);
    }

    public interface IOnRedrawHook
    {
        void OnRedraw(Card card, State state, Combat combat);
    }

}