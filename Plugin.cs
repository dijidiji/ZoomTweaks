using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Receiver2;
using UnityEngine;

namespace AimZoom
{
    [BepInProcess("Receiver2.exe")]
    [BepInPlugin("dijidiji.plugins.ZoomTweaks", "ZoomTweaks", "1.0.0")]
    public class ZoomTweaks : BaseUnityPlugin
    {
        private static ConfigEntry<int> zoomFov;
        private static ConfigEntry<float> timeScale;

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(ZoomTweaks));
            zoomFov = Config.Bind("Settings", "FOV", 60, new ConfigDescription("The field of view to use when aiming down the sights", new AcceptableValueRange<int>(1, 180)));
            timeScale = Config.Bind("Settings", "Time scale", 1f, new ConfigDescription("How fast time runs when aiming down the sights", new AcceptableValueRange<float>(0.01f, 1f)));
        }

        public void Update()
        {
            ConfigFiles.global.media_mode_time_scale = timeScale.Value;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalAimHandler), "UpdateAimZoom")]
        static bool Prefix(LocalAimHandler __instance, Spring ___aim_spring, Spring ___slide_pose_spring, Spring ___reload_pose_spring, Spring ___press_check_pose_spring, Spring ___inspect_cylinder_pose_spring, Spring ___add_rounds_pose_spring, Spring ___eject_rounds_pose_spring, Spring ___camera_zoom_spring, ref float ___main_camera_fov, Camera ___main_camera, Camera ___ui_camera)
        {
            if (___aim_spring.target_state > 0f && ___slide_pose_spring.target_state == 0f && ___reload_pose_spring.target_state == 0f && ___press_check_pose_spring.target_state < 0.2f && ___inspect_cylinder_pose_spring.target_state == 0f && ___add_rounds_pose_spring.target_state == 0f && ___eject_rounds_pose_spring.target_state < 0.2f)
            {
                ___camera_zoom_spring.target_state = 1f;
                ConfigFiles.global.media_mode_enabled = true;
            }
            else
            {
                ___camera_zoom_spring.target_state = 0f;
                ConfigFiles.global.media_mode_enabled = false;
            }
            ___camera_zoom_spring.Update(Time.deltaTime);
            float b = zoomFov.Value;
            float num2 = Mathf.Lerp(___main_camera_fov, b, ___camera_zoom_spring.state);

            if (___main_camera.fieldOfView != num2)
            {
                ___main_camera.fieldOfView = num2;
            }

            return false;
        }
    }
}
