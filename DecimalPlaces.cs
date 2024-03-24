using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using SecretHistories;
using HarmonyLib;

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

public class OneMoreDecimalPlaceForTimers : MonoBehaviour
{
    private static Harmony harmony;

    public void Start() {}
    public void Update() {}
    public void OnDestroy() {}

    public static void Initialise() {
        DecimalPlaces.harmony = new Harmony("robynthedevil.decimalplaces");
		//new GameObject().AddComponent<DecimalPlaces>();
		try
		{
            DecimalPlaces.harmony.PatchAll();
		}
		catch (Exception ex)
		{
			NoonUtility.LogWarning(ex.ToString());
			NoonUtility.LogException(ex);
		}
        NoonUtility.LogWarning("DecimalPlaces: Initialised");
	}
}

