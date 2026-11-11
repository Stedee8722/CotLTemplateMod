using System;
using System.Collections.Generic;
using System.Text;

namespace CotLTemplateMod.Patches;

    [HarmonyPatch]
public static partial class ReflectionCameraPatch
{
    private static readonly AccessTools.FieldRef<ReflectionCamera, Camera> mainCameraRef =
        AccessTools.FieldRefAccess<ReflectionCamera, Camera>("mainCamera");

    private static readonly AccessTools.FieldRef<ReflectionCamera, Camera> reflectionCameraRef =
        AccessTools.FieldRefAccess<ReflectionCamera, Camera>("reflectionCamera");

    private static readonly AccessTools.FieldRef<ReflectionCamera, RenderTexture> renderTextureRef =
        AccessTools.FieldRefAccess<ReflectionCamera, RenderTexture>("renderTexture");

    private static readonly AccessTools.FieldRef<ReflectionCamera, Vector3> planePosRef =
        AccessTools.FieldRefAccess<ReflectionCamera, Vector3>("planePos");

    private static readonly AccessTools.FieldRef<ReflectionCamera, Vector3> planeNormalRef =
        AccessTools.FieldRefAccess<ReflectionCamera, Vector3>("planeNormal");

    private static readonly AccessTools.FieldRef<ReflectionCamera, Vector3> rotationOffsetRef =
        AccessTools.FieldRefAccess<ReflectionCamera, Vector3>("rotationOffset");

    [HarmonyPatch(typeof(ReflectionCamera), nameof(ReflectionCamera.Awake))]
    [HarmonyPostfix]
    private static void ReflectionCamera_Awake(ReflectionCamera __instance)
    {
        __instance.reflectionCamera.farClipPlane = 1000f;
        __instance.reflectionCamera.cullingMask = ((1 << 1) | (1 << 2) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14) | (1 << 15) | (1 << 16) | (1 << 21) | (1 << 23) | (1 << 24) | (1 << 29));
    }

    [HarmonyPatch(typeof(ReflectionCamera), "Update")]
    [HarmonyPrefix]
    public static bool ReflectionCamera_Update(ReflectionCamera __instance)
    {
        // Setups
        Camera mainCamera = mainCameraRef(__instance);
        Camera reflectionCamera = reflectionCameraRef(__instance);
        RenderTexture renderTexture = renderTextureRef(__instance);
        Vector3 planePos = planePosRef(__instance);
        Vector3 planeNormal = planeNormalRef(__instance);
        Vector3 rotationOffset = rotationOffsetRef(__instance);

        // modified Update()
        reflectionCamera.ResetProjectionMatrix();
        reflectionCamera.ResetWorldToCameraMatrix();
        int graphicsPreset = SettingsManager.Settings.Graphics.GraphicsPreset;
        //MonoBehaviour.print(string.Format("Current Quality Level: {0}", graphicsPreset));
        if (graphicsPreset <= 1)
        {
            renderTexture.Release();
            return false;
        }
        Vector3 position = mainCamera.transform.position;
        Vector3 vector = new Vector3(position.x, position.y, -position.z * 0.5f);
        vector.z += planePos.z;

        Vector3 forward = mainCamera.transform.forward;
        Vector3 up = mainCamera.transform.up;

        Vector3 vector2 = new Vector3(forward.x, forward.y, -forward.z * 0.5f);
        Vector3 vector3 = new Vector3(up.x, up.y, -up.z * 0.5f);

        reflectionCamera.transform.position = vector;
        reflectionCamera.transform.rotation = Quaternion.LookRotation(vector2, vector3);
        reflectionCamera.transform.rotation *= Quaternion.Euler(rotationOffset);
        reflectionCamera.orthographic = mainCamera.orthographic;
        reflectionCamera.orthographicSize = mainCamera.orthographicSize;
        reflectionCamera.fieldOfView = mainCamera.fieldOfView;
        Matrix4x4 projectionMatrix = mainCamera.projectionMatrix;
        ref Matrix4x4 ptr = ref projectionMatrix;
        ptr[0, 0] = ptr[0, 0] * -1f;
        reflectionCamera.projectionMatrix = projectionMatrix;
        reflectionCamera.targetTexture = renderTexture;
        Action onPreReflectionRender = ReflectionCamera.OnPreReflectionRender;
        if (onPreReflectionRender != null)
        {
            onPreReflectionRender();
        }
        reflectionCamera.Render();
        Action onPostReflectionRender = ReflectionCamera.OnPostReflectionRender;
        if (onPostReflectionRender == null)
        {
            return false;
        }
        onPostReflectionRender();

        return false;
    }
    
}
