using GGJ24;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AbilityManager : MonoBehaviour
{
    public static Action<Ability> AbilityUnlocked;

    public class Ability
    {
        public string Name { get; }
        public bool IsUnlocked { get; private set; }
        public int UnlockThreshold { get; private set; }

        public Ability(string name, bool isUnlocked, int unlockThreshold)
        {
            Name = name;
            IsUnlocked = isUnlocked;
            UnlockThreshold = unlockThreshold;
        }

        public void CheckAndUnlock(int eggs)
        {
            if (eggs == UnlockThreshold)
            {
                IsUnlocked = true;
                AbilityUnlocked?.Invoke(this);
            }
        }
    }

    public static Ability DoubleJump;
    public static Ability Dash;
    public static Ability Dodge;
    public static Ability Reflect;
    public static Ability XRay;

    private static List<Ability> _alllAbilities;

    private void Start()
    {
        // must do in start to guarantee GameParamsLoader is fully initialized
        DoubleJump = new("DOUBLE JUMP", false, GameParamsLoader.EggsCollectedToUnlockDoubleJump);
        Dash = new("DASH", false, GameParamsLoader.EggsCollectedToUnlockDash);
        Dodge = new("DODGE", false, GameParamsLoader.EggsCollectedToUnlockDodge);
        Reflect = new("REFLECT", false, GameParamsLoader.EggsCollectedToUnlockReflectMissiles);
        XRay = new("XRAY", false, GameParamsLoader.EggsCollectedToUnlockXRay);

        _alllAbilities = new(typeof(AbilityManager).GetStaticNestedFieldsOfType<Ability>());
        Debug.Log($"Collected {_alllAbilities.Count} abilities");
    }

    private void OnEnable()
    {
        Egg.CollectedEgg += UnlockAbilities;
    }

    private void OnDisable()
    {
        Egg.CollectedEgg -= UnlockAbilities;
    }

    public static void UnlockAll()
    {
        foreach (Ability ability in _alllAbilities)
        {
            ability.CheckAndUnlock(ability.UnlockThreshold);
        }
    }

    private void UnlockAbilities()
    {
        foreach (Ability ability in _alllAbilities)
        {
            if (!ability.IsUnlocked)
                ability.CheckAndUnlock(EggManager.CollectedEggs);
        }
    }
}
