using UnityEngine;
using UnityEngine.SceneManagement;

public class MedicineWarningManager : MonoBehaviour
{
    [Header("Configuração do efeito")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.4f;
    public float warningDurationSeconds = 5f;

    // Referências auto-detectadas por cena
    private PlayerController player;
    private TimerController timer;
    private GameObject remedioPanel;

    private float originalSpeed;
    private float warningTimer = 0f;
    private bool effectActive = false;
    private bool warningShown = false;

    void Awake()
    {
        // Garante que só exista 1 Manager
        var managers = FindObjectsOfType<MedicineWarningManager>();
        if (managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Sempre que trocar de cena → reconectar tudo
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        DetectarReferencias();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DetectarReferencias();
    }

    private void DetectarReferencias()
    {
        // Player da cena atual
        player = FindObjectOfType<PlayerController>();

        // Timer da cena atual
        timer = TimerController.Instance;

        // Painel chamado "remedio" na cena atual
        remedioPanel = GameObject.Find("remedio");

        // Se existir o painel, ele começa desligado
        if (remedioPanel != null)
            remedioPanel.SetActive(false);

        // Guarda velocidade original
        if (player != null)
            originalSpeed = player.moveSpeed;
    }

    void Update()
    {
        if (timer == null)
            return;

        float horaAtual = timer.GetHoraAtual();

        // 1) Lentidão entre 16h e 18h
        if (horaAtual >= 16f && horaAtual < 18f)
        {
            if (!effectActive && player != null)
            {
                effectActive = true;
                originalSpeed = player.moveSpeed;
                player.moveSpeed = originalSpeed * slowMultiplier;
            }
        }
        else
        {
            if (effectActive && player != null)
            {
                effectActive = false;
                player.moveSpeed = originalSpeed;
            }
        }

        // 2) Mostrar painel às 16h
        if (!warningShown && horaAtual >= 16f && horaAtual < 16.05f)
        {
            if (remedioPanel != null)
            {
                remedioPanel.SetActive(true);
                warningTimer = warningDurationSeconds;
            }
            warningShown = true;
        }

        // 3) Esconder painel após X segundos
        if (remedioPanel != null && remedioPanel.activeSelf)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
                remedioPanel.SetActive(false);
        }

        // 4) Reset do efeito por "dia" (antes das 6h)
        if (horaAtual < 6f)
            warningShown = false;
    }
}
