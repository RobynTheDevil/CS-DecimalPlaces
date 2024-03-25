using System;
using System.Collections.Generic;
using UnityEngine;
using SecretHistories;
using HarmonyLib;

public class OneMoreDecimalPlaceForTimers : MonoBehaviour
{
    public static Harmony harmony;
    public static PatchTracker showDecimal {get; private set;}
    public static PatchTracker hidePoint {get; private set;}
    public static PatchTracker hideUnit {get; private set;}

    public void Start() {
        try
        {
            OneMoreDecimalPlaceForTimers.showDecimal = new PatchTracker("ShowTimerDecimal", new MainPatch());
            OneMoreDecimalPlaceForTimers.showDecimal.Start();
            OneMoreDecimalPlaceForTimers.hidePoint = new PatchTracker("HideTimerPoint", new PointPatch());
            OneMoreDecimalPlaceForTimers.hidePoint.Start();
            OneMoreDecimalPlaceForTimers.hideUnit = new PatchTracker("HideTimerUnit", new UnitPatch());
            OneMoreDecimalPlaceForTimers.hideUnit.Start();
        }
        catch (Exception ex)
        {
          NoonUtility.LogException(ex);
        }
        NoonUtility.LogWarning("OneMoreDecimalPlaceForTimers: Trackers Started");
    }

    public static void Initialise() {
        //Harmony.DEBUG = true;
        OneMoreDecimalPlaceForTimers.harmony = new Harmony("robynthedevil.onemoredecimalplacefortimers");
		new GameObject().AddComponent<OneMoreDecimalPlaceForTimers>();
        NoonUtility.Log("OneMoreDecimalPlaceForTimers: Initialised");
	}

}

