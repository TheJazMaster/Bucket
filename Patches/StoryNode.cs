using System.Reflection;
using HarmonyLib;
using Nickel;
using TheJazMaster.Bucket.Patches;

namespace TheJazMaster.Bucket.Patches;

internal class StoryNodePatches
{
	static ModEntry Instance => ModEntry.Instance;
	static IModData ModData => Instance.Helper.ModData;
	static Harmony Harmony => Instance.Harmony;


	public static void Apply()
	{
		Harmony.TryPatch(
			logger: Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(StoryNode), nameof(StoryNode.Filter)),
			postfix: new HarmonyMethod(typeof(StoryNodePatches), nameof(StoryNode_Filter_Postfix))
		);
	}

	private static void StoryNode_Filter_Postfix(ref bool __result, string key, StoryNode n, State s, StorySearch ctx)
	{
		if (!__result) return;

		{
			if (ModData.TryGetModData<bool>(n, StoryVarsPatches.CreatedTrashKey, out var required) &&
				(!ModData.TryGetModData(s.storyVars, StoryVarsPatches.CreatedTrashKey, out bool present) || required != present))
			{
				__result = false;
				return;
			}
		}
	}


}