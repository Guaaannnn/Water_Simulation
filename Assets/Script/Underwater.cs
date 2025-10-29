using UnityEngine;

[ExecuteAlways]
public class UnderwaterEffect : MonoBehaviour
{
    [Header("Water Reference")]
    public Transform waterSurface;      // Taruh objek plane / air di sini

    [Header("Underwater Settings")]
    public Color underwaterColor = new Color(0.05f, 0.35f, 0.55f, 1f);
    public float underwaterFogDensity = 0.08f;
    public float underwaterLightIntensity = 0.6f;

    [Header("Above Water Settings")]
    public Color aboveWaterFogColor = new Color(0.5f, 0.8f, 1f, 1f);
    public float aboveWaterFogDensity = 0.002f;
    public float aboveWaterLightIntensity = 1f;

    [Header("Debug")]
    public bool showDebug = true;

    private Light mainLight;
    private bool isUnderwater = false;

    void Start()
    {
        mainLight = RenderSettings.sun;
        if (waterSurface == null)
        {
            Debug.LogWarning("🌊 UnderwaterEffect: Water surface belum di-assign!");
        }

        // Pastikan fog aktif
        RenderSettings.fog = true;
    }

    void Update()
    {
        if (waterSurface == null)
            return;

        float waterY = waterSurface.position.y;
        float cameraY = transform.position.y;

        bool currentlyUnderwater = cameraY < waterY;

        // Ganti efek hanya jika status berubah
        if (currentlyUnderwater != isUnderwater)
        {
            isUnderwater = currentlyUnderwater;

            if (isUnderwater)
                EnterUnderwater();
            else
                ExitUnderwater();
        }

        // Debug status real-time
        if (showDebug)
        {
            Debug.Log($"[{name}] {(isUnderwater ? "🌊 Di dalam air" : "☀️ Di atas air")} | CameraY={cameraY:F2}, WaterY={waterY:F2}");
        }
    }

    void EnterUnderwater()
    {
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = underwaterFogDensity;
        if (Camera.main != null)
            Camera.main.backgroundColor = underwaterColor;
        if (mainLight != null)
            mainLight.intensity = underwaterLightIntensity;

        Debug.Log("💧 Masuk ke bawah air");
    }

    void ExitUnderwater()
    {
        RenderSettings.fogColor = aboveWaterFogColor;
        RenderSettings.fogDensity = aboveWaterFogDensity;
        if (Camera.main != null)
            Camera.main.backgroundColor = aboveWaterFogColor;
        if (mainLight != null)
            mainLight.intensity = aboveWaterLightIntensity;

        Debug.Log("☀️ Keluar dari air");
    }
}
