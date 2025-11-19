using UnityEngine;

public class MedicineWarningFromTimer : MonoBehaviour
{
    [Header("Referências")]
    public PlayerController playerController;   // Player
    public GameObject warningPanel;            // painel "remedio"
    public TimerController timer;              // seu relógio REAL da cena

    [Header("Efeito do Remédio")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.4f;        // velocidade reduzida
    public float warningDurationSeconds = 5f;  // tempo que o painel fica visível

    private float originalSpeed;
    private bool effectActive = false;
    private bool warningShown = false;
    private float warningTimer = 0f;

    void Start()
    {
        if (playerController != null)
            originalSpeed = playerController.moveSpeed;

        if (warningPanel != null)
            warningPanel.SetActive(false);
    }

    void Update()
    {
        if (timer == null) return;

        float horaAtual = timer.GetHoraAtual(); // ex.: 16.25 = 16:15

        // 1) Aplicar lentidão das 16h até antes das 18h
        if (horaAtual >= 16f && horaAtual < 18f)
        {
            if (!effectActive)
                AtivarLentidao();
        }
        else
        {
            if (effectActive)
                DesativarLentidao();
        }

        // 2) Mostrar aviso exatamente às 16h (uma única vez)
        if (!warningShown && horaAtual >= 16f && horaAtual < 16.05f) // janela pequena
        {
            MostrarAviso();
        }

        // 3) Contador para esconder o painel após X segundos
        if (warningPanel.activeSelf)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
                warningPanel.SetActive(false);
        }

        // 4) Se o dia reinicia, permitir mostrar novamente no próximo ciclo
        if (horaAtual < 6f) // se estiver antes do horário inicial
            warningShown = false;
    }

    private void AtivarLentidao()
    {
        effectActive = true;
        playerController.moveSpeed = originalSpeed * slowMultiplier;
    }

    private void DesativarLentidao()
    {
        effectActive = false;
        playerController.moveSpeed = originalSpeed;
    }

    private void MostrarAviso()
    {
        warningShown = true;
        warningPanel.SetActive(true);
        warningTimer = warningDurationSeconds;
    }
}
