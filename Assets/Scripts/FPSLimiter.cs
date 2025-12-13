using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    void Awake()
    {
        // FPS'i 30'a sınırlar. Ses anında kesilir.
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }
}