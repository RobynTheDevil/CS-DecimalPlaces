using System;
using System.Collections.Generic;
using UnityEngine;
using SecretHistories;
using HarmonyLib;

public class OneMoreDecimalPlaceForTimers : MonoBehaviour
{
    public static PatchTracker showDecimal {get; private set;}
    public static PatchTracker hidePoint {get; private set;}
    public static PatchTracker hideUnit {get; private set;}

    public void Start() {
        try
        {
            OneMoreDecimalPlaceForTimers.showDecimal = new PatchTracker("ShowTimerDecimal", new MainPatch());
            OneMoreDecimalPlaceForTimers.hidePoint = new PatchTracker("HideTimerPoint", new PointPatch());
            OneMoreDecimalPlaceForTimers.hideUnit = new PatchTracker("HideTimerUnit", new UnitPatch());
        }
        catch (Exception ex)
        {
          NoonUtility.LogException(ex);
        }
        NoonUtility.Log("OneMoreDecimalPlaceForTimers: Trackers Started");
    }

    public static void Initialise() {
        //Harmony.DEBUG = true;
        Patch.harmony = new Harmony("robynthedevil.onemoredecimalplacefortimers");
		new GameObject().AddComponent<OneMoreDecimalPlaceForTimers>();
        NoonUtility.Log("OneMoreDecimalPlaceForTimers: Initialised");
	}

}

