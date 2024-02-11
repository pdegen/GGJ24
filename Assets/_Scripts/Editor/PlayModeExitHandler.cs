using UnityEditor;
using UnityEngine;

namespace GGJ24
{
    [InitializeOnLoad]
    public static class PlayModeExitHandler
    {
        static PlayModeExitHandler()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                //ResetMaterials();
                Debug.Log("Exiting Play Mode");
            }
        }

        //// Reset materials to have cleaner git commits
        //private static void ResetMaterials()
        //{
        //    Resources.Load<Material>("Materials/DanceRadialMaterial").SetFloat("_FillPercent", 1); 
        //    Resources.Load<Material>("Materials/DashRadialMaterial").SetFloat("_FillPercent", 1);
        //    Resources.Load<Material>("Materials/GoldenEggRadialMaterial").SetFloat("_FillPercent", 1);
        //}
    }
}