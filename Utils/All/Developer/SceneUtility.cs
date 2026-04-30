using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow.Interaction;
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
            Page = UICore.UtilitiesPage.CreatePage("Scene Utility", OverrideColor.green);
            Page.CreateFunction("Refresh", Color.white, Refresh);
            MarrowEntities = Page.CreatePage("Marrow Entities", Color.white);
            SelectedEntity = Page.CreatePage("Selected Entity", Color.white);
        }

        private void Refresh()
        {
            SelectedEntity.RemoveAll();
            MarrowEntities.RemoveAll();
            var entities = UnityEngine.Object.FindObjectsOfType<MarrowEntity>();
            foreach (var entity in entities)
            {
                MarrowEntities.CreateFunction(entity.name, Color.white, () => SelectEntity(entity));
            }
        }

        private void SelectEntity(MarrowEntity entity)
        {
            SelectedEntity.RemoveAll();
            var bodiesPage = SelectedEntity.CreatePage("Bodies", Color.white);
            var pooleePage = SelectedEntity.CreatePage("Poolee", Color.white);
            entity.Bodies.ToList().ForEach(body =>
            {
                var bodyPage = pooleePage.CreatePage(body.name, Color.white);
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
                    poolee.Despawn();
                    SelectedEntity.RemoveAll();
                });
            }
            DevUtils.Notify($"Selected Entity {entity.name}");
        }
    }
}
