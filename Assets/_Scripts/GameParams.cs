using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    [CreateAssetMenu(fileName = "GameParams", menuName = "ScriptableObject/GameParams")]
    public class GameParams : ScriptableObject
    {
        [Range(0f, 1f)] public float _dodgeChance;
    }
}