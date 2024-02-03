using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParamsLoader : MonoBehaviour
{

    [SerializeField] private GameParams _params;

    public static float DodgeChance { get; private set; }
    public static float WaterLevel { get; private set; }
    [SerializeField] private Transform _waterObject;

    void Start()
    {
        DodgeChance = _params._dodgeChance;
        WaterLevel = _waterObject.transform.position.y;
    }

}
