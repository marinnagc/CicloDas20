using UnityEngine;

public class MedicineWarningFromTimer : MonoBehaviour
{
    [Header("Referências Gerais")]
    public PlayerController playerController;
    public TimerController timer;

    [Header("Alerta do Remédio (16h)")]
    public GameObject painelRemedio;
    public int horarioRemedio = 16;
    public float duracaoRemedio = 5f;
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.4f;
    public int fimEfeitoLentidao = 18;

    [Header("Alerta do Toque de Recolher (20h)")]
    public GameObject painelRecolher;
    public int horarioRecolher = 20;
    public float duracaoRecolher = 5f;

    // Variáveis internas - Remédio
    private float originalSpeed;
    private bool effectActive = false;
    private bool remedioShown = false;
    private float remedioTimer = 0f;

    // Variáveis internas - Recolher
    private bool recolherShown = false;
    private float recolherTimer = 0f;

    void Start()
    {
        if (playerController != null)
            originalSpeed = playerController.moveSpeed;

        if (painelRemedio != null)
            painelRemedio.SetActive(false);

        if (painelRecolher != null)
            painelRecolher.SetActive(false);
    }

    void Update()
    {
        if (timer == null) return;

        int horaAtual = timer.GetHoraInteira();

        // ========== REMÉDIO (16h) ==========

        // Aplicar lentidão das 16h até antes das 18h
        if (horaAtual >= horarioRemedio && horaAtual < fimEfeitoLentidao)
        {
            if (!effectActive)
                AtivarLentidao();
        }
        else
        {
            if (effectActive)
                DesativarLentidao();
        }

        // Mostrar aviso do remédio
        if (!remedioShown && horaAtual == horarioRemedio)
        {
            MostrarAviso(painelRemedio, ref remedioTimer, duracaoRemedio, "Remédio");
            remedioShown = true;
        }

        // Timer do painel remédio
        if (painelRemedio != null && painelRemedio.activeSelf)
        {
            remedioTimer -= Time.deltaTime;
            if (remedioTimer <= 0f)
            {
                painelRemedio.SetActive(false);
                Debug.Log("[Alertas] Painel Remédio escondido!");
            }
        }

        // ========== TOQUE DE RECOLHER (20h) ==========

        // Mostrar aviso do recolher
        if (!recolherShown && horaAtual == horarioRecolher)
        {
            MostrarAviso(painelRecolher, ref recolherTimer, duracaoRecolher, "Recolher");
            recolherShown = true;
        }

        // Timer do painel recolher
        if (painelRecolher != null && painelRecolher.activeSelf)
        {
            recolherTimer -= Time.deltaTime;
            if (recolherTimer <= 0f)
            {
                painelRecolher.SetActive(false);
                Debug.Log("[Alertas] Painel Recolher escondido!");
            }
        }

        // ========== RESET PARA PRÓXIMO DIA ==========
        if (horaAtual < horarioRemedio)
            remedioShown = false;

        if (horaAtual < horarioRecolher)
            recolherShown = false;
    }

    private void AtivarLentidao()
    {
        effectActive = true;
        if (playerController != null)
            playerController.moveSpeed = originalSpeed * slowMultiplier;
        Debug.Log("[Alertas] Lentidão ativada!");
    }

    private void DesativarLentidao()
    {
        effectActive = false;
        if (playerController != null)
            playerController.moveSpeed = originalSpeed;
        Debug.Log("[Alertas] Lentidão desativada!");
    }

    private void MostrarAviso(GameObject painel, ref float timerRef, float duracao, string nome)
    {
        if (painel != null)
        {
            painel.SetActive(true);
            timerRef = duracao;
            Debug.Log("[Alertas] Painel " + nome + " mostrado! Timer: " + duracao);
        }
        else
        {
            Debug.LogError("[Alertas] Painel " + nome + " é NULL!");
        }
    }
}