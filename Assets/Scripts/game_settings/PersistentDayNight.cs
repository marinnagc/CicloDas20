using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Persistent Day/Night controller:
/// - Singleton + DontDestroyOnLoad (works even if originally childed)
/// - Reconnects scene-specific references on scene load (Overlay Image, Main Camera, optional Timer UI)
/// - Logs helpful info to console
/// Replace your DayNightController component or attach this to the same GameObject (it will try to keep that GameObject persistent).
/// </summary>
[DisallowMultipleComponent]
public class PersistentDayNight : MonoBehaviour
{
    public static PersistentDayNight Instance { get; private set; }

    [Header("Cycle settings (example)")]
    public int startHour = 6;
    public int endHour = 22;
    public float dayLengthSeconds = 300f; // seconds for full 6->22

    [Header("Scene references (auto found if empty)")]
    public Light directionalLight;    // can be assigned in inspector or auto-found
    public Image overlayImage;        // UI image that tints the screen
    public Camera mainCamera;         // main camera

    [Header("Visual presets (keep or edit in inspector)")]
    public Gradient skyGradient;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public AnimationCurve overlayAlphaCurve;
    public float colorPresence = 1.25f;

    // runtime
    [HideInInspector] public float normalizedTime = 0f;

    void Awake()
    {
        // -----------------------
        // SINGLETON + ROOTING
        // -----------------------
        if (Instance != null && Instance != this)
        {
            Debug.Log("[PersistentDayNight] Duplicate instance detected -> destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure this object is at root (DontDestroyOnLoad works only on root objects).
        if (transform.parent != null)
        {
            transform.SetParent(null, true);
            Debug.Log("[PersistentDayNight] Unparented from previous parent to make persistent root.");
        }

        DontDestroyOnLoad(gameObject);

        // Subscribe to scene loaded so we can reconnect references
        SceneManager.sceneLoaded -= OnSceneLoaded; // safe unsubscribe
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Try to fill references now (for first scene)
        TryAutoAssignReferences();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[PersistentDayNight] Scene loaded: " + scene.name + " -> trying to reconnect references.");
        TryAutoAssignReferences();
    }

    void TryAutoAssignReferences()
    {
        // Main Camera
        if (mainCamera == null)
        {
            if (Camera.main != null) mainCamera = Camera.main;
            else
            {
                var cams = FindObjectsOfType<Camera>();
                if (cams.Length > 0) mainCamera = cams[0];
            }

            Debug.Log("[PersistentDayNight] mainCamera assigned: " + (mainCamera ? mainCamera.name : "NULL"));
        }

        // Directional Light: try to find a Light tagged or named 'SunLight' or first directional light
        if (directionalLight == null)
        {
            // try by name
            var sun = GameObject.Find("SunLight");
            if (sun != null) directionalLight = sun.GetComponent<Light>();

            if (directionalLight == null)
            {
                // fallback: first directional light in scene
                var allLights = FindObjectsOfType<Light>();
                foreach (var l in allLights)
                {
                    if (l.type == LightType.Directional) { directionalLight = l; break; }
                }
            }
            Debug.Log("[PersistentDayNight] directionalLight assigned: " + (directionalLight ? directionalLight.name : "NULL"));
        }

        // Overlay Image: try by name 'Overlay', then by tag 'OverlayUI'
        if (overlayImage == null)
        {
            // try by name
            var go = GameObject.Find("Overlay");
            if (go != null) overlayImage = go.GetComponent<Image>();

            // try by tag (if you set a tag 'OverlayUI' on your overlay)
            if (overlayImage == null)
            {
                try
                {
                    var tagged = GameObject.FindWithTag("OverlayUI");
                    if (tagged != null) overlayImage = tagged.GetComponent<Image>();
                }
                catch (UnityException) { /* no tag defined */ }
            }

            // fallback: first Image in scene under a Canvas (weak fallback)
            if (overlayImage == null)
            {
                var images = FindObjectsOfType<Image>();
                foreach (var img in images)
                {
                    if (img.canvas != null && img.canvas.renderMode != RenderMode.WorldSpace)
                    {
                        overlayImage = img;
                        break;
                    }
                }
            }
            Debug.Log("[PersistentDayNight] overlayImage assigned: " + (overlayImage ? overlayImage.name : "NULL"));
        }

        // If you want a TMP clock UI to be auto-linked, add that logic here (example commented).
    }

    void Update()
    {
        if (dayLengthSeconds <= 0f) dayLengthSeconds = 1f;
        normalizedTime += Time.deltaTime / dayLengthSeconds;
        normalizedTime %= 1f;

        // apply visuals if refs exist
        if (overlayImage != null && skyGradient != null && overlayAlphaCurve != null)
        {
            Color baseSky = skyGradient.Evaluate(normalizedTime) * colorPresence;
            float alpha = overlayAlphaCurve.Evaluate(normalizedTime);
            baseSky.a = Mathf.Clamp01(alpha);
            overlayImage.color = baseSky;
        }

        if (directionalLight != null && lightColorGradient != null && lightIntensityCurve != null)
        {
            directionalLight.color = lightColorGradient.Evaluate(normalizedTime);
            directionalLight.intensity = lightIntensityCurve.Evaluate(normalizedTime);
            float angle = Mathf.Lerp(-90f, 270f, normalizedTime);
            directionalLight.transform.rotation = Quaternion.Euler(angle, 170f, 0f);
        }

        if (mainCamera != null && skyGradient != null)
        {
            Color camBg = Color.Lerp(Color.black, skyGradient.Evaluate(normalizedTime), 0.45f);
            mainCamera.backgroundColor = camBg * (0.7f + 0.3f * overlayAlphaCurve.Evaluate(normalizedTime));
        }

        // optional: update RenderSettings ambient
        if (skyGradient != null) RenderSettings.ambientLight = skyGradient.Evaluate(normalizedTime) * 0.6f;
    }

    // Public helpers
    public string GetCurrentTimeString()
    {
        // map normalizedTime to hours between startHour and endHour
        float totalHours = (endHour >= startHour) ? (endHour - startHour) : (24 - startHour + endHour);
        float hourFloat = startHour + normalizedTime * totalHours;
        if (hourFloat >= 24f) hourFloat -= 24f;
        int h = Mathf.FloorToInt(hourFloat);
        int m = Mathf.FloorToInt((hourFloat - h) * 60f + 0.5f);
        if (m >= 60) { m = 0; h = (h + 1) % 24; }
        return string.Format("{0:00}:{1:00}", h, m);
    }
}
