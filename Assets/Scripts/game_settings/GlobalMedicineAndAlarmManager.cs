using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalMedicineAndAlarmManager : MonoBehaviour
{
    [Header("Horários (em horas do jogo)")]
    public float medicineHour = 16f;      // hora do remédio
    public float medicineEndHour = 18f;   // fim do efeito de lentidão
    public float curfewHour = 20f;        // toque de recolher / patrulha

    [Header("Efeito do remédio")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.4f;       // 40% da velocidade
    public float warningDurationSeconds = 5f; // quanto tempo o painel fica visível

    [Header("Áudio")]
    public AudioSource sfxSource;         // AudioSource só para efeitos
    public AudioClip medicineAlarmClip;   // som das 16h
    public AudioClip curfewAlarmClip;     // som das 20h

    [Header("Debug")]
    public bool debugLog = false;

    // refs dinâmicas por cena
    private PlayerController player;
    private GameObject medicinePanel;     // painel "remedio" da cena

    private float originalSpeed;
    private bool slowActive = false;

    private bool medicineTriggered = false;
    private bool curfewTriggered = false;
    private float warningTimer = 0f;

    private static GlobalMedicineAndAlarmManager Instance;

    void Awake()
    {
        // singleton global
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        DetectSceneReferences();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DetectSceneReferences();
    }

    /// <summary>
    /// Procura PlayerController e painel "remedio" na cena atual
    /// </summary>
    private void DetectSceneReferences()
    {
        player = FindObjectOfType<PlayerController>();
        if (player != null)
            originalSpeed = player.moveSpeed;

        // procura um GameObject chamado "remedio" (igual em TODAS as scenes)
        medicinePanel = GameObject.Find("remedio");
        if (medicinePanel != null)
            medicinePanel.SetActive(false);

        if (debugLog)
            Debug.Log($"[GlobalMedicine] Cena carregada: {SceneManager.GetActiveScene().name} | Player={player != null} | Panel={(medicinePanel != null)}");
    }

    void Update()
    {
        // se o relógio global ainda não existe, não faz nada
        if (TimerController.Instance == null)
            return;

        float horaAtual = TimerController.Instance.GetHoraAtual();

        if (debugLog)
            Debug.Log($"[GlobalMedicine] Hora atual: {horaAtual:0.00}");

        // 1) Efeito de lentidão 16h–18h
        HandleSlowEffect(horaAtual);

        // 2) Aviso + som do remédio às 16h
        HandleMedicineWarning(horaAtual);

        // 3) Som do toque de recolher às 20h
        HandleCurfewAlarm(horaAtual);

        // 4) Contador do painel do remédio
        HandlePanelTimer();
    }

    private void HandleSlowEffect(float horaAtual)
    {
        bool inWindow = horaAtual >= medicineHour && horaAtual < medicineEndHour;

        if (inWindow && player != null)
        {
            if (!slowActive)
            {
                slowActive = true;
                originalSpeed = player.moveSpeed;
                player.moveSpeed = originalSpeed * slowMultiplier;

                if (debugLog)
                    Debug.Log("[GlobalMedicine] Lentidão ativada.");
            }
        }
        else
        {
            if (slowActive && player != null)
            {
                slowActive = false;
                player.moveSpeed = originalSpeed;

                if (debugLog)
                    Debug.Log("[GlobalMedicine] Lentidão desativada.");
            }
        }
    }

    private void HandleMedicineWarning(float horaAtual)
    {
        // só dispara UMA vez, em uma janelinha logo após as 16h
        if (!medicineTriggered && horaAtual >= medicineHour && horaAtual < medicineHour + 0.1f)
        {
            medicineTriggered = true;

            if (medicinePanel != null)
            {
                medicinePanel.SetActive(true);
                warningTimer = warningDurationSeconds;
            }

            PlaySfx(medicineAlarmClip);

            if (debugLog)
                Debug.Log("[GlobalMedicine] Aviso de remédio disparado.");
        }
    }

    private void HandleCurfewAlarm(float horaAtual)
    {
        if (!curfewTriggered && horaAtual >= curfewHour && horaAtual < curfewHour + 0.1f)
        {
            curfewTriggered = true;
            PlaySfx(curfewAlarmClip);

            if (debugLog)
                Debug.Log("[GlobalMedicine] Alarme de toque de recolher disparado.");
        }
    }

    private void HandlePanelTimer()
    {
        if (medicinePanel != null && medicinePanel.activeSelf)
        {
            warningTimer -= Time.unscaledDeltaTime;
            if (warningTimer <= 0f)
                medicinePanel.SetActive(false);
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
