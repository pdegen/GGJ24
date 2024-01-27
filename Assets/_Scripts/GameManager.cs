using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static int RageLevel = 0;
    public static int EggsCollected = 0;
    public static float LevelRadius = 60f;

    public static void Victory()
    {
        Debug.Log("Victory!");
        //Time.timeScale = 0;
    }
}
