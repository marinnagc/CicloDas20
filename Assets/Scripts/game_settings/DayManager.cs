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

    [Header("UI - Tela de Transição")]
    [SerializeField] private GameObject painelTransicao; // Panel preto que cobre tudo
    [SerializeField] private TextMeshProUGUI textoDia; // Texto que mostra "Day X"
    [SerializeField] private CanvasGroup canvasGroupTransicao; // Para fade in/out

    [Header("Tempos de Animação")]
    [SerializeField] private float tempoFadeIn = 1f; // Tempo para a tela ficar preta
    [SerializeField] private float tempoMostrarDia = 2f; // Tempo que mostra o dia
    [SerializeField] private float tempoFadeOut = 1f; // Tempo para a tela sumir

    // Variáveis internas
    private int diaAtual = 1;
    private bool emTransicao = false;

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
    /// Chamado pelo TimeController quando chega 22h
    /// </summary>
    public void IniciarTransicaoDia()
    {
        if (emTransicao) return;

        StartCoroutine(TransicaoDiaCoroutine());
    }

    private IEnumerator TransicaoDiaCoroutine()
    {
        emTransicao = true;

        // Incrementa o dia
        diaAtual++;

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

        // Reseta o TimerController para 6h
        if (TimerController.Instance != null)
        {
            TimerController.Instance.ResetarDia();
        }

        // === FADE OUT DO TEXTO ===
        if (textoDia != null)
        {
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                textoDia.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
                yield return null;
            }
            textoDia.alpha = 0f;
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