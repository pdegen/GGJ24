using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParamsLoader : MonoBehaviour
{

    [SerializeField] private GameParams _params;

    public static float DodgeChance { get; private set; }

    void Start()
    {
        DodgeChance = _params._dodgeChance;
    }

}
