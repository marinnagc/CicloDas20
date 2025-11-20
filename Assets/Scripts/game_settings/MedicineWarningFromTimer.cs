using UnityEngine;

public class MedicineWarningFromTimer : MonoBehaviour
{
    [Header("Alerta do Remédio (16h)")]
    public GameObject painelRemedio;
    public int horarioRemedio = 16;
    public float duracaoRemedio = 5f;
    [Range(0.1f, 1f)] public float slowMultiplier = 0.4f;
    public int fimEfeitoLentidao = 18;

    [Header("Alerta do Toque de Recolher (20h)")]
    public GameObject painelRecolher;
    public int horarioRecolher = 20;
    public float duracaoRecolher = 5f;

    // estado interno
    private bool effectActive = false;
    private bool remedioShown = false;
    private float remedioTimer = 0f;
    private bool recolherShown = false;
    private float recolherTimer = 0f;

    private TimerController timer;

    void Start()
    {
        // usa o Timer global
        timer = TimerController.Instance;

        if (painelRemedio != null)
            painelRemedio.SetActive(false);

        if (painelRecolher != null)
            painelRecolher.SetActive(false);
    }

    void Update()
    {
        // se por algum motivo ainda não tem Timer, tenta pegar de novo
        if (timer == null)
        {
            timer = TimerController.Instance;
            if (timer == null) return;
        }

        int horaAtual = timer.GetHoraInteira();

        // ======= REMÉDIO 16h–18h =======
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

        // aviso do remédio
        if (!remedioShown && horaAtual == horarioRemedio)
        {
            MostrarAviso(painelRemedio, ref remedioTimer, duracaoRemedio, "Remédio");
            remedioShown = true;
        }

        if (painelRemedio != null && painelRemedio.activeSelf)
        {
            remedioTimer -= Time.deltaTime;
            if (remedioTimer <= 0f)
            {
                painelRemedio.SetActive(false);
                Debug.Log("[Alertas] Painel Remédio escondido!");
            }
        }

        // ======= RECOLHER 20h =======
        if (!recolherShown && horaAtual == horarioRecolher)
        {
            MostrarAviso(painelRecolher, ref recolherTimer, duracaoRecolher, "Recolher");
            recolherShown = true;
        }

        if (painelRecolher != null && painelRecolher.activeSelf)
        {
            recolherTimer -= Time.deltaTime;
            if (recolherTimer <= 0f)
            {
                painelRecolher.SetActive(false);
                Debug.Log("[Alertas] Painel Recolher escondido!");
            }
        }

        // reset flags pro próximo dia
        if (horaAtual < horarioRemedio)
            remedioShown = false;
        if (horaAtual < horarioRecolher)
            recolherShown = false;
    }

    private void AtivarLentidao()
    {
        effectActive = true;
        PlayerController.globalSpeedMultiplier = slowMultiplier;
        Debug.Log("[Alertas] Lentidão ativada! Multiplier = " + slowMultiplier);
    }

    private void DesativarLentidao()
    {
        effectActive = false;
        PlayerController.globalSpeedMultiplier = 1f;
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
