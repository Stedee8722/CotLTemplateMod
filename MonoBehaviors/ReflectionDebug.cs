using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CotLTemplateMod.MonoBehaviors
{
    public class ReflectionDebug : MonoBehaviour
    {
        private void OnGUI()
        {
            if (ReflectionCamera.Instance == null)
                return;

            Camera cam =
                ReflectionCamera.Instance.ReflectionCameraInstance;

            if (cam == null || cam.targetTexture == null)
                return;

            GUI.DrawTexture(
                new Rect(10, 10, 960, 540),
                cam.targetTexture,
                ScaleMode.ScaleToFit,
                false
            );
        }
    }
}
