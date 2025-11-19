using UnityEngine;

public class MedicineWarningSystem : MonoBehaviour
{
    [Header("Referências")]
    public PlayerController playerController;   // arrasta o Player aqui
    public GameObject warningPanel;            // arrasta o objeto "remedio" aqui

    [Header("Configuração do dia (relógio interno)")]
    public float dayDurationRealSeconds = 300f; // 5 min reais
    public int startHour = 6;                   // 06:00
    public int endHour = 22;                    // 22:00

    [Header("Efeito do remédio")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.5f;         // 50% da velocidade
    public float warningDurationSeconds = 5f;   // tempo que o painel fica visível

    [Header("Debug (somente leitura)")]
    public int debugHour;
    public int debugMinute;

    [Header("Teste rápido")]
    public bool testEffectOnStart = false;

    // ---- internos ----
    private float normalizedTime = 0f;
    private int currentHour;
    private int currentMinute;

    private bool effectActive = false;
    private bool warningShownToday = false;
    private float warningTimer = 0f;
    private int lastHour = -1;

    private float originalMoveSpeed;

    void Start()
    {
        if (playerController != null)
            originalMoveSpeed = playerController.moveSpeed;

        // modo de teste: já começa lento e com aviso
        if (testEffectOnStart)
        {
            AtivarEfeitoLentidao();
            MostrarAviso();
        }
    }

    void Update()
    {
        AtualizarRelogioInterno();

        debugHour = currentHour;
        debugMinute = currentMinute;

        float hourFloat = currentHour + currentMinute / 60f;

        // virada de dia → pode mostrar aviso de novo
        if (lastHour != -1 && currentHour < lastHour)
            warningShownToday = false;

        lastHour = currentHour;

        // 1) Lentidão entre 16h e 18h
        if (hourFloat >= 16f && hourFloat < 18f)
        {
            if (!effectActive)
                AtivarEfeitoLentidao();
        }
        else
        {
            if (effectActive)
                DesativarEfeitoLentidao();
        }

        // 2) Mostrar painel uma vez às 16h
        if (!warningShownToday && hourFloat >= 16f && hourFloat < 16.1f)
        {
            MostrarAviso();
        }

        // 3) Timer pra esconder o painel
        if (warningPanel != null && warningPanel.activeSelf)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
                warningPanel.SetActive(false);
        }
    }

    private void AtualizarRelogioInterno()
    {
        normalizedTime += Time.deltaTime / dayDurationRealSeconds;
        if (normalizedTime > 1f)
            normalizedTime -= 1f;

        float startMinutes = startHour * 60;
        float endMinutes = endHour * 60;
        float currentMinutesTotal = Mathf.Lerp(startMinutes, endMinutes, normalizedTime);

        currentHour = Mathf.FloorToInt(currentMinutesTotal / 60f);
        currentMinute = Mathf.FloorToInt(currentMinutesTotal % 60f);
    }

    private void AtivarEfeitoLentidao()
    {
        effectActive = true;
        if (playerController != null)
            playerController.moveSpeed = originalMoveSpeed * slowMultiplier;
    }

    private void DesativarEfeitoLentidao()
    {
        effectActive = false;
        if (playerController != null)
            playerController.moveSpeed = originalMoveSpeed;
    }

    private void MostrarAviso()
    {
        warningShownToday = true;
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            warningTimer = warningDurationSeconds;
        }
    }
}
