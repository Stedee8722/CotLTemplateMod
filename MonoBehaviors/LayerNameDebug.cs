using CustomSpineLoader.MapEditor;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CotLTemplateMod.MonoBehaviors
{
    public class LayerNameDebug : MonoBehaviour
    {
        private void Awake()
        {
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (!string.IsNullOrEmpty(layerName))
                {
                    bool included =
                        (gameObject.GetComponent<Camera>().cullingMask & (1 << i)) != 0;

                    Debug.Log(
                        $"Layer {i}: {layerName} | Included: {included}"
                    );
                }
            }
        }
    }
}
