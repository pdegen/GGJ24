using GGJ24;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static Action<string> AbilityUnlocked;

    public static bool CanDodge = false;
    public static bool CanDoubleJump = false;

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
    }
}
