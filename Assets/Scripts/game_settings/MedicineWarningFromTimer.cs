using UnityEngine;

public class MedicineWarningFromTimer : MonoBehaviour
{
    [Header("Referências")]
    public PlayerController playerController;
    public GameObject warningPanel;
    public TimerController timer;

    [Header("Efeito do Remédio")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.4f;
    public float warningDurationSeconds = 5f;

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

        int horaAtual = timer.GetHoraInteira(); // Use GetHoraInteira() ao invés de GetHoraAtual()

        // 1) Aplicar lentidão das 16h até antes das 18h
        if (horaAtual >= 16 && horaAtual < 18)
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
        if (!warningShown && horaAtual == 16)
        {
            MostrarAviso();
        }

        // 3) Contador para esconder o painel após X segundos
        if (warningPanel != null && warningPanel.activeSelf)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
            {
                warningPanel.SetActive(false);
                Debug.Log("[MedicineWarning] Painel escondido!");
            }
        }

        // 4) Reset para o próximo dia
        if (horaAtual < 16)
            warningShown = false;
    }

    private void AtivarLentidao()
    {
        effectActive = true;
        if (playerController != null)
            playerController.moveSpeed = originalSpeed * slowMultiplier;
        Debug.Log("[MedicineWarning] Lentidão ativada!");
    }

    private void DesativarLentidao()
    {
        effectActive = false;
        if (playerController != null)
            playerController.moveSpeed = originalSpeed;
        Debug.Log("[MedicineWarning] Lentidão desativada!");
    }

    private void MostrarAviso()
    {
        warningShown = true;
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            warningTimer = warningDurationSeconds;
            Debug.Log("[MedicineWarning] Painel mostrado! Timer: " + warningDurationSeconds);
        }
        else
        {
            Debug.LogError("[MedicineWarning] warningPanel é NULL!");
        }
    }
}