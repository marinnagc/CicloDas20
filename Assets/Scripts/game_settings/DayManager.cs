using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [Header("Configurações")]
    [SerializeField] private string cenaQuarto = "Quarto"; // Nome da cena do quarto
    [SerializeField] private Vector3 posicaoSpawnQuarto = new Vector3(0, 0, 0); // Posição onde o player aparece no quarto
    [SerializeField] private string cenaGameOver = "PerdeuGO"; // Nome da cena de Game Over
    [SerializeField] private int diasParaGameOver = 10; // Número de dias até o Game Over

    [Header("UI - Tela de Transição")]
    [SerializeField] private GameObject painelTransicao; // Panel preto que cobre tudo
    [SerializeField] private TextMeshProUGUI textoDia; // Texto que mostra "Day X"
    [SerializeField] private TextMeshProUGUI textoMensagem; // Texto adicional (ex: "você foi pego fora do quarto")
    [SerializeField] private CanvasGroup canvasGroupTransicao; // Para fade in/out

    [Header("Tempos de Animação")]
    [SerializeField] private float tempoFadeIn = 1f; // Tempo para a tela ficar preta
    [SerializeField] private float tempoMostrarDia = 2f; // Tempo que mostra o dia
    [SerializeField] private float tempoFadeOut = 1f; // Tempo para a tela sumir

    // Variáveis internas
    private int diaAtual = 1;
    private bool emTransicao = false;
    private bool proximoDiaComecaAs8 = false; // Flag para punição

    void Awake()
    {
        // Singleton persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // RESETA OS DIAS TODA VEZ QUE O JOGO INICIA
            diaAtual = 1;
            PlayerPrefs.SetInt("DiaAtual", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // IMPORTANTE: Desativa o painel imediatamente no Awake
        if (painelTransicao != null)
        {
            painelTransicao.SetActive(false);
        }

        if (canvasGroupTransicao != null)
        {
            canvasGroupTransicao.alpha = 0f;
        }
    }

    void Start()
    {
        // Não precisa mais carregar do PlayerPrefs pois sempre começa em 1
        diaAtual = 1;
    }

    /// <summary>
    /// Chamado pelo TimerController quando chega 22h
    /// </summary>
    public void IniciarTransicaoDia()
    {
        if (emTransicao) return;

        StartCoroutine(TransicaoDiaCoroutine(false)); // false = transição normal
    }

    /// <summary>
    /// Chamado quando o segurança pega o player
    /// </summary>
    public void IniciarTransicaoDiaPorCaptura()
    {
        if (emTransicao) return;

        // Define que o próximo dia começa às 8h como punição
        proximoDiaComecaAs8 = true;

        StartCoroutine(TransicaoDiaCoroutine(true)); // true = player foi capturado
    }

    private IEnumerator TransicaoDiaCoroutine(bool foiCapturado)
    {
        emTransicao = true;

        // Incrementa o dia
        diaAtual++;

        // Verifica se chegou no limite de dias (Game Over)
        if (diaAtual > diasParaGameOver)
        {
            Debug.Log("Chegou ao dia " + diaAtual + " - Carregando Game Over");

            // Salva o progresso
            PlayerPrefs.SetInt("DiaAtual", diaAtual);
            PlayerPrefs.Save();

            // Pequeno delay antes de carregar o Game Over
            yield return new WaitForSecondsRealtime(0.5f);

            // Carrega a cena de Game Over
            SceneManager.LoadScene(cenaGameOver);

            emTransicao = false;
            yield break; // Sai da coroutine
        }

        // Salva o progresso
        PlayerPrefs.SetInt("DiaAtual", diaAtual);
        PlayerPrefs.Save();

        // Pausa o jogo (opcional - remove se não quiser)
        // Time.timeScale = 0f;

        // Ativa o painel de transição
        if (painelTransicao != null)
        {
            painelTransicao.SetActive(true);
        }

        // Atualiza o texto do dia
        if (textoDia != null)
        {
            textoDia.text = "Day " + diaAtual;
            textoDia.alpha = 0f; // Começa invisível
        }

        // Atualiza a mensagem adicional (se foi capturado)
        if (textoMensagem != null)
        {
            if (foiCapturado)
            {
                textoMensagem.text = "Você foi pego fora do quarto e perderá 2 horas do seu dia";
                textoMensagem.alpha = 0f; // Começa invisível
            }
            else
            {
                textoMensagem.text = ""; // Sem mensagem na transição normal
                textoMensagem.alpha = 0f;
            }
        }

        // === FADE IN (tela fica preta) ===
        if (canvasGroupTransicao != null)
        {
            float t = 0f;
            while (t < tempoFadeIn)
            {
                t += Time.unscaledDeltaTime; // Usa unscaled para funcionar mesmo com timeScale = 0
                canvasGroupTransicao.alpha = Mathf.Lerp(0f, 1f, t / tempoFadeIn);
                yield return null;
            }
            canvasGroupTransicao.alpha = 1f;
        }

        // === MOSTRA O TEXTO DO DIA ===
        if (textoDia != null)
        {
            // Fade in do texto
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                textoDia.alpha = Mathf.Lerp(0f, 1f, t / 0.5f);
                yield return null;
            }
            textoDia.alpha = 1f;
        }

        // === MOSTRA A MENSAGEM ADICIONAL (se foi capturado) ===
        if (foiCapturado && textoMensagem != null && !string.IsNullOrEmpty(textoMensagem.text))
        {
            // Fade in da mensagem
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                textoMensagem.alpha = Mathf.Lerp(0f, 1f, t / 0.5f);
                yield return null;
            }
            textoMensagem.alpha = 1f;
        }

        // Espera mostrando o dia
        yield return new WaitForSecondsRealtime(tempoMostrarDia);

        // === CARREGA A CENA DO QUARTO ===
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cenaQuarto);

        // Espera a cena carregar
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Pequeno delay para garantir que tudo foi inicializado
        yield return new WaitForSecondsRealtime(0.1f);

        // Reposiciona o player
        ReposicionarPlayer();

        // Reseta o TimerController para 6h ou 8h (se foi pego)
        if (TimerController.Instance != null)
        {
            int horaInicio = proximoDiaComecaAs8 ? 8 : 6;
            TimerController.Instance.ResetarDia(horaInicio);

            // Reseta a flag após usar
            proximoDiaComecaAs8 = false;
        }

        // === FADE OUT DO TEXTO ===
        if (textoDia != null)
        {
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                textoDia.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);

                // Faz fade out da mensagem junto
                if (foiCapturado && textoMensagem != null)
                {
                    textoMensagem.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
                }

                yield return null;
            }
            textoDia.alpha = 0f;

            if (foiCapturado && textoMensagem != null)
            {
                textoMensagem.alpha = 0f;
            }
        }

        // === FADE OUT (tela volta ao normal) ===
        if (canvasGroupTransicao != null)
        {
            float t = 0f;
            while (t < tempoFadeOut)
            {
                t += Time.unscaledDeltaTime;
                canvasGroupTransicao.alpha = Mathf.Lerp(1f, 0f, t / tempoFadeOut);
                yield return null;
            }
            canvasGroupTransicao.alpha = 0f;
        }

        // Desativa o painel
        if (painelTransicao != null)
        {
            painelTransicao.SetActive(false);
        }

        // Volta o timeScale ao normal (se tiver pausado)
        // Time.timeScale = 1f;

        emTransicao = false;

        Debug.Log("Novo dia começou: Day " + diaAtual);
    }

    private void ReposicionarPlayer()
    {
        // Tenta encontrar o player de várias formas
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player != null)
        {
            player.transform.position = posicaoSpawnQuarto;

            // Se tiver Rigidbody2D, zera a velocidade
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Debug.Log("Player reposicionado em: " + posicaoSpawnQuarto);
        }
        else
        {
            Debug.LogWarning("[DayManager] Player não encontrado para reposicionar!");
        }
    }

    // === MÉTODOS PÚBLICOS ===

    public int GetDiaAtual()
    {
        return diaAtual;
    }

    public void ResetarJogo()
    {
        diaAtual = 1;
        PlayerPrefs.SetInt("DiaAtual", 1);
        PlayerPrefs.Save();
        Debug.Log("Dias resetados para 1");
    }

    /// <summary>
    /// Método estático para resetar os dias - pode ser chamado do menu mesmo sem Instance
    /// </summary>
    public static void ResetarDias()
    {
        PlayerPrefs.SetInt("DiaAtual", 1);
        PlayerPrefs.Save();

        // Se a instância existir, atualiza ela também
        if (Instance != null)
        {
            Instance.diaAtual = 1;
        }

        Debug.Log("Dias resetados para 1 (static)");
    }

    public bool EstaEmTransicao()
    {
        return emTransicao;
    }
}