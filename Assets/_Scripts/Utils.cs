using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.UI;

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
                int k = UnityEngine.Random.Range(0, n + 1);
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
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

            return navHit.position;
        }
    }

    public static class ObjectExtensions
    {
        public static List<T> GetNestedFieldValuesOfType<T>(this object obj)
        {
            List<T> values = new List<T>();
            Type type = typeof(T);

            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == type)
                {
                    T fieldValue = (T)field.GetValue(obj);
                    values.Add(fieldValue);
                }
            }

            return values;
        }

        public static List<T> GetStaticNestedFieldsOfType<T>(this Type type)
        {
            List<T> nestedFieldsOfType = new List<T>();

            // Get all fields of the declaring type
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                // Check if the field is of type T
                if (field.FieldType == typeof(T))
                {
                    // Get the value of the static field
                    T value = (T)field.GetValue(null);
                    nestedFieldsOfType.Add(value);
                }
            }

            return nestedFieldsOfType;
        }
    }
}