
using CustomSpineLoader.MapEditor.Tools;
using CustomSpineLoader.MapEditor;
using System.IO;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;

namespace CotLTemplateMod.Patches;

[HarmonyPatch]
public static partial class ShapeToolPatch
{
    private static bool IsReflective(SpriteShape profile) =>
        profile != null && profile.name.IndexOf("reflection", System.StringComparison.OrdinalIgnoreCase) >= 0;

    public static RenderTexture ReflectionTexture
    {
        get
        {
            Camera cam = SceneRefs.Cam.transform.parent.Find("ReflectionCamera").GetComponent<Camera>();
            if (cam != null)
            {
                return cam.targetTexture;
            }
            Log(LogLevel.Error, "Reflection camera not found for scene ." + SceneManager.GetActiveScene().name);
            return null;
        }
    }

    [HarmonyPatch(typeof(ShapeTool), "CommitShape")]
    [HarmonyPrefix]
    private static void ShapeTool_CommitShape(ShapeTool __instance, SpriteShapeController ctrl)
    {
        if (ctrl == null)
            return;
        var renderer = ctrl.GetComponent<SpriteShapeRenderer>();

        if (IsReflective(ctrl.spriteShape))
        {
            Log(LogLevel.Debug, "Setting reflection texture for shape " + ctrl.spriteShape.name + ".");
            renderer.materials[0].SetTexture("_MainTex", ReflectionTexture, UnityEngine.Rendering.RenderTextureSubElement.Depth);
            renderer.materials[0].SetTexture("_BaseMap", ReflectionTexture, UnityEngine.Rendering.RenderTextureSubElement.Depth);
        } else
        {
            Log(LogLevel.Debug, "Setting normal texture for shape " + ctrl.spriteShape.name + ".");
            renderer.materials[0] = SceneRefs.ShapeMaterial;
        }
    }
}
