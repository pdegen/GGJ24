using GGJ24;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static Action<string> AbilityUnlocked;

    public static bool CanDance;
    public static bool CanDash;
    public static bool CanDoubleJump;
    public static bool CanReflectMissiles;


    private void Awake()
    {
        CanDance = false;
        CanDash = false;
        CanDoubleJump = false;
        CanReflectMissiles = false;
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
        CanDance = true;
        CanDash = true;
        CanDoubleJump = true;
        CanReflectMissiles = true;
    }

    private void UnlockAbilities()
    {
        if (EggManager.CollectedEggs == GameParamsLoader.EggsCollectedToUnlockDodge)
        {
            AbilityUnlocked?.Invoke("DODGE UNLOCKED!");
            CanDance = true;
        }

        if (EggManager.CollectedEggs == GameParamsLoader.EggsCollectedToUnlockDoubleJump)
        {
            AbilityUnlocked?.Invoke("DOUBLE JUMP UNLOCKED!");
            CanDoubleJump = true;
        }

        if (EggManager.CollectedEggs == GameParamsLoader.EggsCollectedToUnlockDash)
        {
            AbilityUnlocked?.Invoke("DASH UNLOCKED!");
            CanDash = true;
        }

        if (EggManager.CollectedEggs == GameParamsLoader.EggsCollectedToUnlockReflectMissiles)
        {
            AbilityUnlocked?.Invoke("REFLECT UNLOCKED!");
            CanReflectMissiles = true;
        }
    }
}
