using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using SecretHistories;
using HarmonyLib;

public class PatchHelper
{
    public static int FindLdstrOperand(List<CodeInstruction> codes, string operand, int skip=0)
    {
        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldstr)
            {
                string op = codes[i].operand as string;
                if (op == operand)
                {
                    if (skip > 0)
                    {
                        skip--;
                    } else {
                        return i;
                    }
                }
            }
        }
        return -1;
    }
}

public class Patch
{
    public MethodInfo original;
    public MethodInfo patch;

    public void DoPatch()
    {
        OneMoreDecimalPlaceForTimers.harmony.Patch(this.original, transpiler: new HarmonyMethod(this.patch));
    }

    public void UnPatch()
    {
        OneMoreDecimalPlaceForTimers.harmony.Unpatch(this.original, this.patch);
    }
}

public class PatchTracker : ValueTracker<bool>
{
    public Patch patch {get; set;}

    public PatchTracker(string settingId, Patch patch, bool start=true)
        : base(settingId, new bool[2] {false, true}, false)
    {
        this.patch = patch;
        if (start)
            this.Start();
    }

    public override void BeforeSettingUpdated(object newValue) {}

    public override void WhenSettingUpdated(object newValue)
    {
        bool prev = this.current;
        this.SetCurrent(newValue);
        if (prev != this.current) {
            if (this.current) {
                NoonUtility.Log(string.Format("OneMoreDecimalPlaceForTimers: Patching {0}, {1}", patch.original, patch.patch));
                try {
                    this.patch.DoPatch();
                } catch (Exception ex) {
                    NoonUtility.LogWarning(ex.ToString());
                    NoonUtility.LogException(ex);
                }
            } else {
                NoonUtility.Log(string.Format("OneMoreDecimalPlaceForTimers: Unpatching {0}, {1}", patch.original, patch.patch));
                try {
                    this.patch.UnPatch();
                } catch (Exception ex) {
                    NoonUtility.LogWarning(ex.ToString());
                    NoonUtility.LogException(ex);
                }
            }
        }
    }
}

[HarmonyPatch(typeof(LanguageManager), nameof(LanguageManager.GetTimeStringForCurrentLanguage))]
public class MainPatch : Patch
{
    public MainPatch() {
        this.original = AccessTools.Method(typeof(LanguageManager), "GetTimeStringForCurrentLanguage");
        this.patch = AccessTools.Method(typeof(MainPatch), "Transpiler");
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        int index = PatchHelper.FindLdstrOperand(codes, "0.0");
        codes[index].operand = "0.00";
        return codes.AsEnumerable();
    }

    public static void Prefix()
    {
        
    }
}

[HarmonyPatch(typeof(LanguageManager), nameof(LanguageManager.GetTimeStringForCurrentLanguage))]
public class PointPatch : Patch
{
    public PointPatch() {
        this.original = AccessTools.Method(typeof(LanguageManager), "GetTimeStringForCurrentLanguage");
        this.patch = AccessTools.Method(typeof(PointPatch), "Transpiler");
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        int index = PatchHelper.FindLdstrOperand(codes, ".");
        codes[index + 1].opcode = OpCodes.Ldsfld;
        codes[index + 1].operand = AccessTools.Field(typeof(String), "Empty");
        return codes.AsEnumerable();
    }
}

[HarmonyPatch(typeof(LanguageManager), nameof(LanguageManager.GetTimeStringForCurrentLanguage))]
public class UnitPatch : Patch
{
    public UnitPatch() {
        this.original = AccessTools.Method(typeof(LanguageManager), "GetTimeStringForCurrentLanguage");
        this.patch = AccessTools.Method(typeof(UnitPatch), "Transpiler");
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        int index = PatchHelper.FindLdstrOperand(codes, "UI_SECONDS_POSTFIX_SHORT");
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Pop, null));
        codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(String), "Empty")));
        return codes.AsEnumerable();
    }
}

