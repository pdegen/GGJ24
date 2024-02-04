using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{

    public static class ListExtensions
    {
        // Extension method to shuffle a generic list using Fisher-Yates algorithm
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 RandomNavSphere(this Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

            return navHit.position;
        }
    }
}