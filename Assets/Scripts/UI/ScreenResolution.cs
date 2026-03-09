using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : MonoBehaviour
{
    private const int targetWidth = 1920;
    private const int targetHeight = 1080;

    private void Start()
    {
        SetResolution();
    }

    private void SetResolution()
    {
        // Définir la résolution de l'écran à 1920x1080, en mode fenêtré sans bordures
        Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
    }
}
