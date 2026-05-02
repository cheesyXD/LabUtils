using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using LabFusion.Network;
using LabUtils.Developer;
using UnityEngine;

namespace LabUtils.Utils.All.Developer
{
    public class SceneUtility : Utility
    {
        public static Page Page;
        public static Page SceneSettings;
        public static Page MarrowEntities;
        public static Page SelectedEntity;
        private static MarrowEntity entity;
        protected override void OnLoad()
        {
            Page = UICore.UtilitiesPage.CreatePage("Scene Utility", ColorPlus.GetNextColor(), maxElements: 10);
            Page.CreateFunction("Refresh", ColorPlus.GetNextColor(), Refresh);
            Page.CreateFunction("Entity Grip Listen", ColorPlus.GetNextColor(), EntityGripListen).SetTooltip("Listens for a Grip on any object that has an entity to automatically select them faster!");
            SceneSettings = Page.CreatePage("Scene Settings", ColorPlus.GetNextColor(), maxElements: 10);
            MarrowEntities = Page.CreatePage("Marrow Entities", ColorPlus.GetNextColor(), maxElements: 10);
            SelectedEntity = Page.CreatePage("Selected Entity", ColorPlus.GetNextColor(), maxElements: 10);
            Hooking.OnGripAttached += Hooking_OnGripAttached;
            Hooking.OnLevelLoaded += (_) => CreateSceneSettings();
        }

        public struct SceneSetting
        {
            public bool fog;
            public float sunIntensity;
            public Color sunColor;
            public Vector3 gravity;
            public static SceneSetting Create()
            {
                float sunIntensity = 1f;
                Color sunColor = ColorPlus.GetNextColor();
                if (RenderSettings.sun)
                {
                    sunIntensity = RenderSettings.sun.intensity;
                    sunColor = RenderSettings.sun.color;
                }
                return new()
                {
                    fog = RenderSettings.fog,
                    sunIntensity = sunIntensity,
                    sunColor = sunColor,
                    gravity = Physics.gravity
                };
            }
        }
        public static SceneSetting sceneSettings = SceneSetting.Create();
        private void CreateSceneSettings()
        {
            sceneSettings = SceneSetting.Create();
            SceneSettings.RemoveAll();
            SceneSettings.CreateBool("Fog", ColorPlus.GetNextColor(), sceneSettings.fog, (a)=> sceneSettings.fog = a);
            if (RenderSettings.sun)
            {
                SceneSettings.CreateFloat("Sun Intensity", ColorPlus.GetNextColor(), sceneSettings.sunIntensity, 1f, 0f, ushort.MaxValue, (a) => sceneSettings.sunIntensity = a);
                SceneSettings.CreateString("Sun Color", ColorPlus.GetNextColor(), ColorUtility.ToHtmlStringRGBA(sceneSettings.sunColor), (a) => sceneSettings.sunColor = ColorPlus.GetColor(a));
            }          
            var ogPhysicsGravity = Physics.gravity;
            var gravityPage = SceneSettings.CreatePage("Gravity", ColorPlus.GetNextColor(), maxElements: 10);
            gravityPage.CreateFloat("x", ColorPlus.GetNextColor(), sceneSettings.gravity.x, 1, short.MinValue, short.MaxValue, (a)=>sceneSettings.gravity.x = a);
            gravityPage.CreateFloat("y", ColorPlus.GetNextColor(), sceneSettings.gravity.y, 1, short.MinValue, short.MaxValue, (a) => sceneSettings.gravity.y = a);
            gravityPage.CreateFloat("z", ColorPlus.GetNextColor(), sceneSettings.gravity.z, 1, short.MinValue, short.MaxValue, (a) => sceneSettings.gravity.z = a);
            gravityPage.CreateFunction("Reset Gravity", ColorPlus.GetNextColor(), () => sceneSettings.gravity = ogPhysicsGravity);
            SceneSettings.CreateFunction("Apply Scene Settings", ColorPlus.GetNextColor(), ApplySceneSettings);
        }

        private static void ApplySceneSettings()
        {
            RenderSettings.fog = sceneSettings.fog;
            if (RenderSettings.sun)
            {
                RenderSettings.sun.intensity = sceneSettings.sunIntensity;
                RenderSettings.sun.color = sceneSettings.sunColor;
            }
            DevUtils.Notify("Applied Scene Settings");
            if (NetworkInfo.HasServer) return;
            Physics.gravity = sceneSettings.gravity;
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
            isListening = false;
            var entity = grip._marrowEntity;
            SelectEntity(entity);
            DevUtils.Notify($"via Grip Listen, Listening Stopped");
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
                MarrowEntities.CreateFunction(entity.name, ColorPlus.GetNextColor(), () => SelectEntity(entity));
            }
        }

        private void SelectEntity(MarrowEntity _entity)
        {
            if (NetworkInfo.HasServer)
            {
                DevUtils.Notify("Cannot select while in server.");
                return;
            }
            entity = _entity;
            SelectedEntity.RemoveAll();
            var bodiesPage = SelectedEntity.CreatePage("Bodies", ColorPlus.GetNextColor(), maxElements: 10);
            var pooleePage = SelectedEntity.CreatePage("Poolee", ColorPlus.GetNextColor(), maxElements: 10);
            entity.Bodies.ToList().ForEach(body =>
            {
                var bodyPage = bodiesPage.CreatePage(body.name, ColorPlus.GetNextColor(), maxElements: 10);
                bodyPage.CreateFloat("Mass", ColorPlus.GetNextColor(), body._rigidbody.mass, 1f, 0.01f, ushort.MaxValue, (a) => body._rigidbody.mass = a);
                bodyPage.CreateFloat("Drag", ColorPlus.GetNextColor(), body._rigidbody.drag, 1f, 0f, ushort.MaxValue, (a) => body._rigidbody.drag = a);
                bodyPage.CreateFloat("Angular Drag", ColorPlus.GetNextColor(), body._rigidbody.angularDrag, 1f, 0f, ushort.MaxValue, (a) => body._rigidbody.angularDrag = a);
                bodyPage.CreateFloat("Max Depenetration Velocity", ColorPlus.GetNextColor(), body._rigidbody.maxDepenetrationVelocity, 1f, 0f, ushort.MaxValue, (a) => body._rigidbody.maxDepenetrationVelocity = a);
                bodyPage.CreateFloat("Max Angular Velocity", ColorPlus.GetNextColor(), body._rigidbody.maxAngularVelocity, 1f, 0f, ushort.MaxValue, (a) => body._rigidbody.maxAngularVelocity = a);
                bodyPage.CreateBool("Is Kinematic", ColorPlus.GetNextColor(), body._rigidbody.isKinematic, (a) => body._rigidbody.isKinematic = a);
                bodyPage.CreateBool("Use Gravity", ColorPlus.GetNextColor(), body._rigidbody.useGravity, (a) => body._rigidbody.useGravity = a);
            });
            var poolee = entity._poolee;
            if (poolee) 
            {
                pooleePage.CreateFunction("Despawn", ColorPlus.GetNextColor(), () =>
                {
                    if (NetworkInfo.HasServer)
                        return;
                    if (entity.GetComponentInParent<RigManager>() || entity.GetComponentInChildren<RigManager>() || entity.GetComponentInParent<Rig>() || entity.GetComponentInChildren<Rig>()) return;
                    poolee.Despawn();
                    SelectedEntity.RemoveAll();
                });
            }
            DevUtils.Notify($"Selected Entity {entity.name}");
        }

        public static GameObject Forward;
    }
}
