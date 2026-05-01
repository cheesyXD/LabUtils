using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow.Interaction;
using LabFusion.Marrow.Integration;
using LabFusion.Network;
using LabFusion.Utilities;
using LabUtils.Developer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LabUtils.Utils.All.Developer
{
    public class SceneUtility : Utility
    {
        public static Page Page;
        public static Page MarrowEntities;
        public static Page SelectedEntity;
        protected override void OnLoad()
        {
            Hooking.OnGripAttached += Hooking_OnGripAttached;
            Page = UICore.UtilitiesPage.CreatePage("Scene Utility", OverrideColor.green);
            Page.CreateFunction("Refresh", Color.white, Refresh);
            Page.CreateFunction("Entity Grip Listen", Color.white, EntityGripListen);
            MarrowEntities = Page.CreatePage("Marrow Entities", Color.white, maxElements: 6);
            SelectedEntity = Page.CreatePage("Selected Entity", Color.white);
        }
        private static bool isListening;
        private static void EntityGripListen()
        {
            if (NetworkInfo.HasServer)
            {
                isListening = false;
                DevUtils.Notify("Cannot listen while in a network session.");
                return;
            }
            isListening = !isListening;
             DevUtils.Notify($"Entity Grip Listen {(isListening ? "Started" : "Stopped")}");
        }

        private void Hooking_OnGripAttached(Il2CppSLZ.Marrow.Grip grip, Il2CppSLZ.Marrow.Hand hand)
        {
            if (!isListening) return;
            var entity = grip._marrowEntity;
            SelectEntity(entity);
            DevUtils.Notify($"Selected Entity {entity.name} via Grip");
        }

        private void Refresh()
        {
            SelectedEntity.RemoveAll();
            MarrowEntities.RemoveAll();
            if(NetworkInfo.HasServer)
            {
                DevUtils.Notify("Cannot refresh while in a network session.");
                return;
            }
            var entities = UnityEngine.Object.FindObjectsOfType<MarrowEntity>();
            foreach (var entity in entities)
            {
                MarrowEntities.CreateFunction(entity.name, Color.white, () => SelectEntity(entity));
            }
        }

        private void SelectEntity(MarrowEntity entity)
        {
            if (NetworkInfo.HasServer)
            {
                DevUtils.Notify("Cannot select while in server.");
                return;
            }
            SelectedEntity.RemoveAll();
            var bodiesPage = SelectedEntity.CreatePage("Bodies", Color.white);
            var pooleePage = SelectedEntity.CreatePage("Poolee", Color.white);
            entity.Bodies.ToList().ForEach(body =>
            {
                var bodyPage = bodiesPage.CreatePage(body.name, Color.white);
                bodyPage.CreateFloat("Mass", Color.white, body._rigidbody.mass, 0.1f, 0.1f, ushort.MaxValue, (a) => body._rigidbody.mass = a);
                bodyPage.CreateFloat("Drag", Color.white, body._rigidbody.drag, 0.1f, 0f, ushort.MaxValue, (a) => body._rigidbody.drag = a);
                bodyPage.CreateFloat("Angular Drag", Color.white, body._rigidbody.angularDrag, 0.1f, 0f, ushort.MaxValue, (a) => body._rigidbody.angularDrag = a);
                bodyPage.CreateFloat("Max Depenetration Velocity", Color.white, body._rigidbody.maxDepenetrationVelocity, 0.1f, 0f, ushort.MaxValue, (a) => body._rigidbody.maxDepenetrationVelocity = a);
                bodyPage.CreateFloat("Max Angular Velocity", Color.white, body._rigidbody.maxAngularVelocity, 0.1f, 0f, ushort.MaxValue, (a) => body._rigidbody.maxAngularVelocity = a);
                bodyPage.CreateBool("Is Kineamtic", Color.white, body._rigidbody.isKinematic, (a) => body._rigidbody.isKinematic = a);
                bodyPage.CreateBool("Use Gravity", Color.white, body._rigidbody.useGravity, (a) => body._rigidbody.useGravity = a);
            });
            var poolee = entity._poolee;
            if (poolee) 
            {
                pooleePage.CreateFunction("Despawn", Color.white, () =>
                {
                    if (NetworkInfo.HasServer)
                        return;
                    poolee.Despawn();
                    SelectedEntity.RemoveAll();
                });
            }
            DevUtils.Notify($"Selected Entity {entity.name}");
        }
    }
}
