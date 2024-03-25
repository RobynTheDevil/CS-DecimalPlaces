using SecretHistories.UI;
using SecretHistories.Entities;
using SecretHistories.Fucine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using HarmonyLib;

public abstract class SettingTracker: ISettingSubscriber
{
    public string settingId;

    public SettingTracker(string settingId)
    {
        this.settingId = settingId;
    }

    public void Start()
    {
        Setting setting = Watchman.Get<Compendium>().GetEntityById<Setting>(this.settingId);
        if (setting == null)
        {
            NoonUtility.LogWarning(string.Format("Setting Missing: {0}", this.settingId));
        }
        setting.AddSubscriber((ISettingSubscriber) this);
        this.WhenSettingUpdated(setting.CurrentValue);
    }

    public abstract void WhenSettingUpdated(object newValue);
    public abstract void BeforeSettingUpdated(object newValue);
}

public class ValueTracker<T> : SettingTracker
{
    public T[] values {get; set;}

    public T current {get; protected set;}

    public ValueTracker(string settingId, T[] values)
        : base(settingId)
    {
        this.values = values;
    }

    public void SetCurrent(object newValue)
    {
        if (!(newValue is int num))
            num = 1;
        int index = Mathf.Min(this.values.Length - 1, Mathf.Max(num, 0));
        this.current = this.values[index];
    }

    public override void BeforeSettingUpdated(object newValue) {}

    public override void WhenSettingUpdated(object newValue)
    {
        this.SetCurrent(newValue);
    }
}

public class KeybindTracker : SettingTracker
{
    public Key key {get; private set;}

    public KeybindTracker(string settingId) : base(settingId) {}

	public static Key ToKey(string id)
	{
		int num = id.LastIndexOf('/');
		string s = num < 0 ? id : id.Substring(num + 1);
		s = int.TryParse(s, out _) ? "Digit" + s : s;
		return (Key) Enum.Parse(typeof (Key), s);
	}

    public override void BeforeSettingUpdated(object newValue) {}

    public override void WhenSettingUpdated(object newValue)
    {
        this.key = KeybindTracker.ToKey((string) newValue);
    }

    public bool wasPressedThisFrame()
    {
        return Keyboard.current[this.key].wasPressedThisFrame;
    }

}

public class PatchTracker : ValueTracker<bool>
{
    public Patch patch {get; set;}

    public PatchTracker(string settingId, Patch patch)
        : base(settingId, new bool[2] {false, true})
    {
        this.patch = patch;
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

