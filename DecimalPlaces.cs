using HarmonyLib;
using SecretHistories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

class DecimalPlaces
{
  public void Start() {
      var harmony = new Harmony("com.cultistsim.robynthedevil.decimalplaces");
      harmony.PatchAll();
  }
}

[HarmonyPatch(typeof(LanguageManager), nameof(LanguageManager.GetTimeStringForCurrentLanguage))]
class Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int index = 0;
        var codes = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldstr)
            {
                var op = codes[i].operand as string;
                if (op == "0.0")
                {
                    index = i;
                    break;
                }
            }
        }
        codes[index].operand = "0.00";
        return codes.AsEnumerable();
    }
}

