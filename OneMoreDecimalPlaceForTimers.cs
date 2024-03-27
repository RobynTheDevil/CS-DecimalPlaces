using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SecretHistories;
using SecretHistories.UI;
using SecretHistories.Entities;
using SecretHistories.Spheres;
using SecretHistories.Abstract;
using HarmonyLib;

public class OneMoreDecimalPlaceForTimers : MonoBehaviour
{
    public static PatchTracker showDecimal {get; private set;}
    public static PatchTracker hidePoint {get; private set;}
    public static PatchTracker hideUnit {get; private set;}

    public void Start() {
        try
        {
            showDecimal = new PatchTracker("ShowTimerDecimal", new MainPatch(), WhenSettingUpdated);
            hidePoint = new PatchTracker("HideTimerPoint", new PointPatch(), WhenSettingUpdated);
            hideUnit = new PatchTracker("HideTimerUnit", new UnitPatch(), WhenSettingUpdated);
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

    public static IEnumerable<Token> GetTokens() {
        return Watchman.Get<HornedAxe>().GetExteriorSpheres()
            .Where<Sphere>((Func<Sphere, bool>) (x => (double) x.TokenHeartbeatIntervalMultiplier > 0.0))
            .SelectMany<Sphere, Token>((Func<Sphere, IEnumerable<Token>>) (x => x.GetTokens()))
            .Where<Token>((Func<Token, bool>) (x => x.Payload is ElementStack || x.Payload is Situation));
    }

    public static void WhenSettingUpdated(SettingTracker<bool> tracker) {
        IEnumerable<Token> tokens = GetTokens();
        foreach (Token token in tokens) {
            token.UpdateVisuals();
        }
    }

}

