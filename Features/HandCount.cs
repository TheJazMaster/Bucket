using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Threading.Tasks;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Input.Touch;
using System.Globalization;
using TheJazMaster.Bucket.Cards;
using System.Net.Security;

namespace TheJazMaster.Bucket.Features;
#nullable enable

public class HandCountManager
{
    private static ModEntry Instance => ModEntry.Instance;
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    public HandCountManager()
    {
    }

    public int CountTrashInHand(State s, Combat c)
    {
        return c.hand.Where(card => card.GetMeta().deck == Deck.trash).ToList().Count;
    }

    public int CountRecycleInHand(State s, Combat c)
    {
        return c.hand.Where(card => card.GetDataWithOverrides(s).recycle).ToList().Count;
    }
}