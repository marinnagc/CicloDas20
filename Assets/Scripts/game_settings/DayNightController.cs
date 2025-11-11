using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Day/night controller com mapeamento de horas reais (ex: 06:00 -> 22:00)
/// Configure "dayLengthSeconds" para ajustar quão rápido um dia passa no jogo.
/// </summary>
public class DayNightController : MonoBehaviour
{
    [Header("Day Length (game hours mapping)")]
    [Tooltip("Hora inicial do ciclo (ex: 6 = 06:00).")]
    [Range(0, 23)] public int startHour = 6;
    [Tooltip("Hora final do ciclo (ex: 22 = 22:00).")]
    [Range(0, 23)] public int endHour = 22;

    [Tooltip("Quantos segundos reais duram esse período (startHour -> endHour). Ex: 300 = 5 minutos.")]
    public float dayLengthSeconds = 300f; // configure para "velocidade do dia"

    [Header("References")]
    public Light directionalLight;
    public Light2D globalLight;
    public Camera mainCamera;

    [Header("Visuals")]
    public Gradient skyGradient;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public AnimationCurve overlayAlphaCurve;
    [Tooltip("Aumenta saturação/presença das cores")]
    public float colorPresence = 1.2f;

    [HideInInspector]
    public float normalizedTime = 0f; // 0..1 across the game-day (startHour -> endHour)

    void Reset()
    {
        // sensible defaults if you add the component in editor
        dayLengthSeconds = 300f;
        // define simple gradients/curves if needed (or set in Inspector)
        if (lightIntensityCurve == null || lightIntensityCurve.length == 0)
        {
            lightIntensityCurve = new AnimationCurve(
                new Keyframe(0f, 0.2f),
                new Keyframe(0.25f, 0.9f),
                new Keyframe(0.5f, 1.2f),
                new Keyframe(0.75f, 0.9f),
                new Keyframe(1f, 0.2f)
            );
        }
    }

    void Start()
    {
        // begin at start of day by default
        normalizedTime = 0f;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (dayLengthSeconds <= 0f) dayLengthSeconds = 1f;

        // advance normalizedTime so that a full range 0..1 takes dayLengthSeconds seconds
        normalizedTime += Time.deltaTime / dayLengthSeconds;
        normalizedTime %= 1f;

        // visuals: evaluate gradient & alpha
        if (skyGradient != null)
        {
            Color baseSky = skyGradient.Evaluate(normalizedTime) * colorPresence;
            float alpha = (overlayAlphaCurve != null && overlayAlphaCurve.length > 0) ? overlayAlphaCurve.Evaluate(normalizedTime) : 0.25f;
            baseSky.a = alpha;

            if (globalLight != null)
            {
                globalLight.color = baseSky;
            }

            if (mainCamera != null)
            {
                Color camBg = Color.Lerp(Color.black, skyGradient.Evaluate(normalizedTime), 0.45f);
                mainCamera.backgroundColor = camBg * (0.7f + 0.3f * alpha);
            }

            // ambient
            RenderSettings.ambientLight = skyGradient.Evaluate(normalizedTime) * 0.6f;
        }

        if (directionalLight != null)
        {
            if (lightColorGradient != null)
                directionalLight.color = lightColorGradient.Evaluate(normalizedTime);
            if (lightIntensityCurve != null)
                directionalLight.intensity = lightIntensityCurve.Evaluate(normalizedTime);

            // rotate sun through the sky
            float angle = Mathf.Lerp(-90f, 270f, normalizedTime);
            directionalLight.transform.rotation = Quaternion.Euler(angle, 170f, 0f);
        }
    }

    /// <summary>
    /// Retorna a hora atual do jogo como float (ex: 6.5 = 06:30)
    /// </summary>
    public float GetCurrentHourFloat()
    {
        // map normalizedTime (0..1) to hours from startHour to endHour (wrap handled)
        float totalHours;
        if (endHour >= startHour)
            totalHours = endHour - startHour; // ex: 22 - 6 = 16
        else
            totalHours = (24 - startHour) + endHour; // caso passe meia-noite (não no seu caso)

        // hour offset from start
        float hourOffset = normalizedTime * totalHours;
        return startHour + hourOffset;
    }

    /// <summary>
    /// Retorna string formatada HH:MM (padrão 24h) do horário do jogo.
    /// </summary>
    public string GetCurrentTimeString()
    {
        float hourFloat = GetCurrentHourFloat();
        // wrap around 24h
        if (hourFloat >= 24f) hourFloat -= 24f;
        int h = Mathf.FloorToInt(hourFloat);
        float frac = hourFloat - h;
        int m = Mathf.FloorToInt(frac * 60f + 0.5f); // arredonda minutos
        if (m >= 60) { m = 0; h = (h + 1) % 24; }
        return string.Format("{0:00}:{1:00}", h, m);
    }

    /// <summary>
    /// Utility: define dayLengthSeconds a partir de um "speed" relativo.
    /// Exemplo: se quiser que 16 horas de mundo (6->22) durem X segundos, defina dayLengthSeconds = X.
    /// </summary>
    public void SetDayLengthSeconds(float seconds)
    {
        dayLengthSeconds = Mathf.Max(0.1f, seconds);
    }
}
