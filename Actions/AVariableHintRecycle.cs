using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TheJazMaster.Bucket.Actions;

public class AVariableHintRecycle : AVariableHint
{   
    public AVariableHintRecycle() : base() {
        hand = true;
    }

    public override Icon? GetIcon(State s) {
        return new Icon(ModEntry.Instance.RecycleHandIcon.Sprite, null, Colors.textMain);
    }

	public override List<Tooltip> GetTooltips(State s)
	{
		List<Tooltip> list = [];
        string parentheses = "";
        if (s.route is Combat c)
        {
            var amt = ModEntry.Instance.HandCountManager.CountTrashInHand(s, c);
            DefaultInterpolatedStringHandler stringHandler = new(22, 1);
            stringHandler.AppendLiteral(" </c>(<c=keyword>");
            stringHandler.AppendFormatted(amt);
            stringHandler.AppendLiteral("</c>)");
            
            parentheses = stringHandler.ToStringAndClear();
        }
        list.Add(new TTText(ModEntry.Instance.Localizations.Localize(["action", "variableHintRecycle", "description"], new { Amount = parentheses })));
        return list;
	}
}