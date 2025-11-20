using UnityEngine;
using TMPro; // TextMesh Pro

public class TimerController : MonoBehaviour
{
    [Header("Configurações de Tempo")]
    [SerializeField] private float duracaoEmMinutos = 5f; // Duração real em minutos
    [SerializeField] private int horaInicial = 6; // 6h da manhã
    [SerializeField] private int horaFinal = 22; // 22h (10pm)

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoRelogio;

    // Variáveis internas
    private float tempoDecorrido = 0f;
    private float duracaoEmSegundos;
    private int horasTotais;
    private float horaAtual;
    private bool tempoAcabou = false;

    public static TimerController Instance;

    void Awake()
    {
        // SINGLETON GLOBAL
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InicializarTempo();
    }

    void InicializarTempo()
    {
        duracaoEmSegundos = duracaoEmMinutos * 60f;
        horasTotais = horaFinal - horaInicial;
        horaAtual = horaInicial;
        tempoDecorrido = 0f;
        tempoAcabou = false;
        AtualizarUI();
    }

    void Update()
    {
        if (tempoAcabou) return;

        // Não atualiza se estiver em transição de dia
        if (DayManager.Instance != null && DayManager.Instance.EstaEmTransicao())
        {
            return;
        }

        tempoDecorrido += Time.deltaTime;
        float porcentagem = tempoDecorrido / duracaoEmSegundos;
        horaAtual = horaInicial + (horasTotais * porcentagem);

        AtualizarUI();

        if (tempoDecorrido >= duracaoEmSegundos)
        {
            tempoAcabou = true;
            horaAtual = horaFinal;
            AtualizarUI();
            OnTempoFinalizado();
        }
    }

    void AtualizarUI()
    {
        if (textoRelogio != null)
        {
            int horas = Mathf.FloorToInt(horaAtual);
            int minutos = Mathf.FloorToInt((horaAtual - horas) * 60f);
            textoRelogio.text = string.Format("{0:00}:{1:00}", horas, minutos);
        }
    }

    void OnTempoFinalizado()
    {
        Debug.Log("O tempo do jogo acabou! São 22:00h - Iniciando transição de dia...");

        // Chama o DayManager para fazer a transição
        if (DayManager.Instance != null)
        {
            DayManager.Instance.IniciarTransicaoDia();
        }
        else
        {
            Debug.LogWarning("[TimerController] DayManager não encontrado! A transição de dia não vai acontecer.");
        }
    }

    /// <summary>
    /// Reseta o dia - chamado pelo DayManager após a transição
    /// </summary>
    /// <param name="novaHoraInicial">Hora para começar (padrão: 6h, punição: 8h)</param>
    public void ResetarDia(int novaHoraInicial = -1)
    {
        // Se não foi especificada uma hora, usa a hora inicial padrão
        if (novaHoraInicial == -1)
        {
            novaHoraInicial = horaInicial;
        }

        // Calcula quanto tempo já passou baseado na nova hora inicial
        float horasPassadas = novaHoraInicial - horaInicial; // Ex: 8 - 6 = 2 horas
        float porcentagemPassada = horasPassadas / horasTotais; // Ex: 2 / 16 = 0.125

        tempoDecorrido = porcentagemPassada * duracaoEmSegundos;
        horaAtual = novaHoraInicial;
        tempoAcabou = false;
        AtualizarUI();

        Debug.Log("TimerController resetado para " + novaHoraInicial + ":00");
    }

    // === MÉTODOS PÚBLICOS ===

    public float GetHoraAtual() => horaAtual;
    public int GetHoraInteira() => Mathf.FloorToInt(horaAtual);
    public bool IsNoite() => horaAtual >= 18f || horaAtual < 6f;
    public bool IsDia() => !IsNoite();
    public float GetPorcentagemDia() => tempoDecorrido / duracaoEmSegundos;
    public bool TempoAcabou() => tempoAcabou;
}