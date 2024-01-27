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

    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.green;
        // Draw a wire sphere at the position of the explosion with the specified radius
        Gizmos.DrawWireSphere(transform.position, LevelRadius);
    }
}
