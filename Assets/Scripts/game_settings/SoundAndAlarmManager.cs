using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SoundAndAlarmManager : MonoBehaviour
{
    public static SoundAndAlarmManager Instance { get; private set; }

    [Header("Time mapping (game)")]
    [Tooltip("Hora inicial (ex: 6 = 06:00)")]
    public int startHour = 6;
    [Tooltip("Hora final (ex: 22 = 22:00)")]
    public int endHour = 22;
    [Tooltip("Quantos segundos reais duram startHour->endHour (ex: 300 = 5 minutos)")]
    public float dayLengthSeconds = 300f;

    [Header("Audio sources")]
    public AudioSource musicSource;   // looping background music (kept playing)
    public AudioSource sfxSource;     // used to PlayOneShot alarms

    [Header("Clips")]
    public AudioClip alarmClip;       // tocar às alarmHour
    public AudioClip curfewClip;      // tocar às curfewHour

    [Header("Hours (24h)")]
    public float alarmHour = 16f;     // 16:00
    public float curfewHour = 20f;    // 20:00

    [Header("Events")]
    public UnityEvent OnAlarmTriggered;
    public UnityEvent OnCurfewTriggered;

    [Header("Debug")]
    public bool debugLogs = false;

    // runtime
    [HideInInspector] public float normalizedTime = 0f; // 0..1 across startHour->endHour
    float prevNormalized = -1f;
    bool alarmPlayed = false;
    bool curfewPlayed = false;

    // precalc
    float totalHours = 0f;
    float alarmNormalized = -1f;
    float curfewNormalized = -1f;

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (transform.parent != null) transform.SetParent(null, true);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ensure audio sources exist
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            if (debugLogs) Debug.Log("[SoundManager_v2] Auto-created musicSource");
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            if (debugLogs) Debug.Log("[SoundManager_v2] Auto-created sfxSource");
        }

        // start music if clip present
        if (musicSource.clip != null && !musicSource.isPlaying) musicSource.Play();

        // precalc normalized thresholds (safely)
        RecalculateThresholds();
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // nothing mandatory, object persists
    }

    void Update()
    {
        // advance normalized time
        if (dayLengthSeconds <= 0.001f) dayLengthSeconds = 1f;
        normalizedTime += Time.deltaTime / dayLengthSeconds;

        // detect wrap and reset flags when a new cycle starts
        if (normalizedTime >= 1f)
        {
            normalizedTime %= 1f;
            alarmPlayed = false;
            curfewPlayed = false;
            if (debugLogs) Debug.Log("[SoundManager_v2] New cycle started - flags reset");
        }

        // check crossing of normalized thresholds (robust)
        // we use prevNormalized to detect forward crossing, handling wrap naturally
        if (prevNormalized < 0f)
        {
            // first frame: just set prev
            prevNormalized = normalizedTime;
            return;
        }

        // handle normal case or wrap case
        // crossing condition: prev < threshold <= current   (if no wrap)
        // if wrap (current < prev) then we check two segments: prev->1 and 0->current
        bool wrapped = normalizedTime < prevNormalized;

        // check alarm
        if (!alarmPlayed)
        {
            if (CrossedNormalized(prevNormalized, normalizedTime, alarmNormalized))
            {
                TriggerAlarm();
                alarmPlayed = true;
            }
        }

        // check curfew
        if (!curfewPlayed)
        {
            if (CrossedNormalized(prevNormalized, normalizedTime, curfewNormalized))
            {
                TriggerCurfew();
                curfewPlayed = true;
            }
        }

        prevNormalized = normalizedTime;
    }

    bool CrossedNormalized(float prev, float current, float threshold)
    {
        // threshold must be in 0..1
        if (threshold < 0f || threshold > 1f) return false;

        if (prev <= current)
        {
            // no wrap
            return (prev < threshold && threshold <= current);
        }
        else
        {
            // wrapped: prev..1 and 0..current
            return (prev < threshold && threshold <= 1f) || (0f <= threshold && threshold <= current);
        }
    }

    void RecalculateThresholds()
    {
        // compute totalHours in the mapped interval
        totalHours = (endHour >= startHour) ? (endHour - startHour) : (24 - startHour + endHour);
        if (totalHours <= 0f) totalHours = 24f; // fallback

        // normalize alarm/curfew into the mapped interval:
        // We compute offset hours from startHour to the event hour, handling wrap.
        alarmNormalized = HourToNormalized(alarmHour);
        curfewNormalized = HourToNormalized(curfewHour);

        if (debugLogs)
        {
            Debug.LogFormat("[SoundManager_v2] totalHours={0} alarmHour={1} -> alarmNormalized={2:F4} curfewHour={3} -> curfewNormalized={4:F4}",
                totalHours, alarmHour, alarmNormalized, curfewHour, curfewNormalized);
        }
    }

    float HourToNormalized(float hour24)
    {
        // map hour24 to normalized [0..1] within the startHour->endHour range.
        // compute relative offset (0..totalHours)
        // handle wrap-around of hours
        float offset;
        if (endHour >= startHour)
        {
            // simple segment
            if (hour24 < startHour) hour24 += 24f; // bring into range
            offset = hour24 - startHour;
        }
        else
        {
            // segment crosses midnight
            // normalize both to same range
            if (hour24 < startHour) hour24 += 24f;
            offset = hour24 - startHour;
        }

        // clamp offset into [0..totalHours]
        offset = Mathf.Repeat(offset, totalHours);
        return Mathf.Clamp01(offset / totalHours);
    }

    void TriggerAlarm()
    {
        if (alarmClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(alarmClip);
            if (debugLogs) Debug.Log("[SoundManager_v2] Alarm played (remédio).");
        }
        OnAlarmTriggered?.Invoke();
    }

    void TriggerCurfew()
    {
        if (curfewClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(curfewClip);
            if (debugLogs) Debug.Log("[SoundManager_v2] Curfew played (recolher).");
        }
        OnCurfewTriggered?.Invoke();
    }

    // Public helpers
    public void ForcePlayAlarm() => TriggerAlarm();
    public void ForcePlayCurfew() => TriggerCurfew();

    public string GetCurrentTimeString()
    {
        float hourFloat = startHour + normalizedTime * totalHours;
        if (hourFloat >= 24f) hourFloat -= 24f;
        int h = Mathf.FloorToInt(hourFloat);
        int m = Mathf.FloorToInt((hourFloat - h) * 60f + 0.5f);
        if (m >= 60) { m = 0; h = (h + 1) % 24; }
        return string.Format("{0:00}:{1:00}", h, m);
    }

    // in case you change start/end/hour at runtime, call this to recalc thresholds
    public void RecomputeBoundaries() => RecalculateThresholds();
}
