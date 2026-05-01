using BoneLib;
using BoneLib.BoneMenu;
using LabUtils.Developer;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LabUtils.Utils.All
{
    public class RagdollUtility : Utility
    {
        public static Page page;
        public static MelonPreferences_Entry<bool> RagdollOnDeath { get; set; } = Core.Preferences.CreateEntry<bool>("RagdollOnDeath", false);

        protected override void OnLoad()
        {
            page = UICore.UtilitiesPage.CreatePage("Ragdoll Utility", OverrideColor.green, maxElements: 10);
            page.CreateBool("Ragdoll On Death", Color.white, RagdollOnDeath.Value, (a) => RagdollOnDeath.Value = a);
            page.CreateFunction("Ragdoll", Color.white, Ragdoll);
            page.CreateFunction("Unragdoll", Color.white, UnRagdoll);
        }

        public static void Ragdoll()
        {
            Player.PhysicsRig.ShutdownRig();
            Player.PhysicsRig.RagdollRig();
        }

        public static void UnRagdoll()
        {
            Player.PhysicsRig.TurnOnRig();
            Player.PhysicsRig.UnRagdollRig();
        }
    }
}
