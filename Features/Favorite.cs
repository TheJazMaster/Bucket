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

public class FavoriteManager
{
    private static ModEntry Instance => ModEntry.Instance;
    private static IModData ModData => ModEntry.Instance.Helper.ModData;
    internal static readonly string FavoriteKey = "Favorite";

    public FavoriteManager()
    {
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.Render)),
			transpiler: new HarmonyMethod(GetType(), nameof(Card_Render_Transpiler))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetAllTooltips)),
			postfix: new HarmonyMethod(GetType(), nameof(Card_GetAllTooltips_Postfix))
		);
    }
    
    private static IEnumerable<CodeInstruction> Card_Render_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        int index = -1;
        foreach (LocalVariableInfo info in originalMethod.GetMethodBody()!.LocalVariables)
        {
            if (info.LocalType == typeof(State))
            {
                index = info.LocalIndex;
            }
        }
        if (index == -1) throw new Exception("Failed!!");

        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldloc<CardData>(originalMethod).ExtractLabels(out var labels).Anchor(out var findAnchor),
                ILMatches.Ldfld("buoyant"),
                ILMatches.Brfalse
            )
            .Find(
                ILMatches.Ldloc<Vec>(originalMethod).CreateLdlocInstruction(out var ldlocVec),
                ILMatches.Ldfld("y"),
                ILMatches.LdcI4(8),
                ILMatches.Ldloc<int>(originalMethod).CreateLdlocaInstruction(out var ldlocaCardTraitIndex),
                ILMatches.Instruction(OpCodes.Dup),
                ILMatches.LdcI4(1),
                ILMatches.Instruction(OpCodes.Add),
                ILMatches.Stloc<int>(originalMethod)
            )
            .Anchors().PointerMatcher(findAnchor)
            .Insert(
                SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                new(OpCodes.Ldloc, index),
                ldlocaCardTraitIndex,
                ldlocVec,
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(RenderFavoriteIcon)))
            )
            .AllElements();
    }

    private static void RenderFavoriteIcon(Card card, State s, ref int cardTraitIndex, Vec vec)
    {
        if (IsFavorite(card))
        {
            Draw.Sprite(Instance.MyFavoriteIcon.Sprite, vec.x, vec.y - (8 * cardTraitIndex++));
        }
    }
    private static void Card_GetAllTooltips_Postfix(Card __instance, State s, bool showCardTraits, ref IEnumerable<Tooltip> __result)
	{
		if (!showCardTraits)
			return;

		if (!IsFavorite(__instance))
			return;

		CustomTTGlossary MakeTooltip()
			=> new(
				CustomTTGlossary.GlossaryType.cardtrait,
				() => ModEntry.Instance.MyFavoriteIcon.Sprite,
				() => ModEntry.Instance.Localizations.Localize(["trait", "favorite", "name"]),
				() => ModEntry.Instance.Localizations.Localize(["trait", "favorite", "description"]),
                key: "trait.favorite"
			);

		IEnumerable<Tooltip> ModifyTooltips(IEnumerable<Tooltip> tooltips)
		{
			bool addTooltip = true;

			foreach (var tooltip in tooltips)
			{
				if (addTooltip && tooltip is CustomTTGlossary glossary && glossary.key == "trait.favorite")
				{
					addTooltip = false;
				}
				yield return tooltip;
			}

			if (addTooltip)
				yield return MakeTooltip();
		}

		__result = ModifyTooltips(__result);
	}

    public static void SetFavorite(Card c, bool to)
    {
        ModData.SetModData(c, FavoriteKey, to);
    }

    public static bool IsFavorite(Card c)
    {
        return ModData.TryGetModData<bool>(c, FavoriteKey, out var value) && value;
    }
}