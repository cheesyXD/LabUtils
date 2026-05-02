using BoneLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using LabUtils.Developer;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LabUtils.Utils.All
{
    public class FunUtility : Utility
    {
        public static MelonPreferences_Entry<bool> MidasTouch { get; set; } = Core.Preferences.CreateEntry<bool>("MidasTouch", false);

        protected override void OnLoad()
        {
            Hooking.OnGripAttached += OnGripAttached;
            var page = UICore.UtilitiesPage.CreatePage("Fun Utlity", ColorPlus.GetNextColor(), maxElements: 10);
            page.CreateBool("Midas Touch", ColorPlus.GetNextColor(), MidasTouch.Value, (a) => MidasTouch.Value= a);
        }
        private static void Hooking_OnLevelLoaded(LevelInfo obj)
        {
            Renderers.Clear();
        }
        private static void OnGripAttached(Grip grip, Hand hand)
        {
            bool flag = MidasTouch.Value;
            if (flag)
            {
                List<Renderer> list = grip.transform.root.GetComponentsInChildren<Renderer>().ToArray<Renderer>().ToList<Renderer>();
                list.AddRange(grip.transform.root.GetComponentsInParent<Renderer>().ToArray<Renderer>());
                foreach (Renderer renderer in list)
                {
                    foreach(var material in renderer.materials)
                    {
                        Color yellow = Color.yellow;
                        material.color = yellow;
                        for (int i = 0; i < floats.Length; i++)
                        {
                            material.SetFloat(floats[i], 1f);
                        }
                        bool flag2 = renderer.GetComponent<Graphic>() == null;
                        if (flag2)
                        {
                            material.mainTexture = null;
                        }
                    }                    
                }
            }
        }
        private static string[] floats = new string[]
        {
            "_Metallic",
            "_Smoothness"
        };
        private const float speed = 3f;
        public static List<Renderer> Renderers = new List<Renderer>();
    }
}
