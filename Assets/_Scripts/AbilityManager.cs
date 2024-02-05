using GGJ24;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static Action<string> AbilityUnlocked;

    public static bool CanDodge;
    public static bool CanDash;
    public static bool CanDoubleJump;


    private void Awake()
    {
        CanDodge = false;
        CanDash = false;
        CanDoubleJump = false;
    }

    private void OnEnable()
    {
        Egg.CollectedEgg += UnlockAbilities;
    }

    private void OnDisable()
    {
        Egg.CollectedEgg -= UnlockAbilities;
    }

    private void UnlockAbilities()
    {
        if (EggManager.CollectedEggs == GameParamsLoader.EggsCollectedToUnlockDodge)
        {
            AbilityUnlocked?.Invoke("DODGE UNLOCKED!");
            CanDodge = true;
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
    }
}
