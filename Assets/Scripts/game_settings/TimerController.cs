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
            Destroy(gameObject);   // já existe um Timer global -> destrói o novo
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // esse Timer sobrevive às trocas de cena
    }
    void Start()
    {
        duracaoEmSegundos = duracaoEmMinutos * 60f;
        horasTotais = horaFinal - horaInicial;
        horaAtual = horaInicial;

        AtualizarUI();
    }

    void Update()
    {
        if (tempoAcabou) return;

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
        Debug.Log("O tempo do jogo acabou! São 22:00h");
    }

    public float GetHoraAtual() => horaAtual;
    public int GetHoraInteira() => Mathf.FloorToInt(horaAtual);
    public bool IsNoite() => horaAtual >= 18f || horaAtual < 6f;
    public bool IsDia() => !IsNoite();
    public float GetPorcentagemDia() => tempoDecorrido / duracaoEmSegundos;
}