using System.Reflection;
using Harmony;

static class Patcher {
    public static void doPatch() {
        var harmony = HarmonyInstance.Create("com.github.markusmo3.nobrain");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}

[HarmonyPatch(typeof(StringTableManager), "GetLongString")]
class patch_StringTableManager {
    
    static void Postfix(ref string __result, string key) {
        if (!NoBrain.SHOW_SHRINES) {
            return;
        }
        var success = NoBrainDB.SHRINES.TryGetValue(key, out var newDescription);
        if (success) {
            __result = newDescription;
        }
    }
}
