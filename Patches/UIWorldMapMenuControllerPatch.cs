using HarmonyLib;
using System.IO;
using System.Linq;
using UnityEngine.UI;

namespace CotLTemplateMod.Patches;

[HarmonyPatch]
public static partial class UIWorldMapMenuControllerPatch
{

    [HarmonyPatch(typeof(UIWorldMapMenuController), nameof(UIWorldMapMenuController.OnShowStarted))]
    [HarmonyPrefix]
    private static void UIWorldMapMenuController_Start(UIWorldMapMenuController __instance)
    {
        Log(LogLevel.Info, "Injected");
        Sprite testSprite = TextureHelper.CreateSpriteFromPath(Path.Combine(Plugin.PluginPath, "Assets", "COTL_SF_OVERWORLD_MAP_test.png"));

        var mapContainerTransform = __instance._mapContainer.transform;
        var locationsTransfrom = mapContainerTransform.Find("Locations");
        var layersTransform = mapContainerTransform.Find("Layers");

        if (locationsTransfrom is null)
        {
            Log(LogLevel.Error, "Brother this guy STINKS");
            return;
        }
        if (layersTransform is null)
        {
            Log(LogLevel.Error, "Brother this guy STINKS");
            return;
        }

        GameObject topLayer = new("Top");
        GameObject testClouds = new("Test Clouds");

        testClouds.transform.SetParent(topLayer.transform);
        topLayer.transform.SetParent(layersTransform);
        topLayer.transform.SetAsLastSibling();

        topLayer.transform.localPosition = new Vector3(0f, 0f, 0f);
        topLayer.transform.localScale = new Vector3(1f, 1f, 1f);

        Image image = testClouds.AddComponent<Image>();
        image.sprite = testSprite;

        var cloudRectTransform = testClouds.GetComponent<RectTransform>();
        cloudRectTransform.anchoredPosition = new Vector3(-198.991f, -129.909f, 0f);
        cloudRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 4111.253f);
        cloudRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2411.361f);

        ParallaxLayer parallaxLayer = topLayer.AddComponent<ParallaxLayer>();
        parallaxLayer._distance = 0f;

        topLayer.SetActive(true);
        testClouds.SetActive(true);

        /* ---------------------------------------------------------------------- */
        CanvasParallax canvasParallax = mapContainerTransform.gameObject.AddComponent<CanvasParallax>();
        WorldMapParallax worldMapParallax = mapContainerTransform.parent.Find("Parallax Controller").GetComponent<WorldMapParallax>();

        worldMapParallax._layers.AddItem(parallaxLayer);

        if (worldMapParallax is null)
        {
            Log(LogLevel.Error, "Brother this guy STINKS");
            return;
        }

        canvasParallax.layers = [.. worldMapParallax._layers];

        canvasParallax.parallaxToRelativeMousePosition = true;

        for (int i = 0; i < canvasParallax.layers.Count; i++)
        {
            Log(LogLevel.Debug, canvasParallax.layers[i].gameObject.name);
            canvasParallax.layers[i]._distance = i * 5f;
        }
    }
}