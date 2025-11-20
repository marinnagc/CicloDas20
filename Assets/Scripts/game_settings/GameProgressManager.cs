using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    // Enum com todas as etapas na ordem correta
    public enum ProgressStage
    {
        Inicio = 0,
        PegouBilheteVaso = 1,           // Quarto - vaso
        FalouComJardineiro = 2,         // Pátio - jardineiro
        FalouComDrMauricio = 3,         // Farmácia - Dr. Maurício
        LeuQuadroAvisos = 4,            // Biblioteca - quadro
        UsouComputador = 5,             // Biblioteca - computador
        AbriuArmario = 6,               // Farmácia - armário
        FalouComBeatriz = 7,            // Entrada - Beatriz
        LeuLivroLiberdade = 8,          // Biblioteca - livro
        FalouComDiretor = 9,            // Refeitório - diretor
        VasculhouLixeira = 10,          // Refeitório - lixeira
        FalouComEnfermeira = 11,        // Enfermaria - enfermeira
        PodeUsarPortaFinal = 12         // Entrada - porta final
    }

    [Header("Debug")]
    [SerializeField] private ProgressStage stageAtual = ProgressStage.Inicio;

    void Awake()
    {
        // Singleton persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Carrega o progresso salvo
        int savedStage = PlayerPrefs.GetInt("GameProgress", 0);
        stageAtual = (ProgressStage)savedStage;

        Debug.Log("Progresso carregado: " + stageAtual);
    }

    /// <summary>
    /// Verifica se o jogador pode interagir com determinada etapa
    /// </summary>
    public bool PodeInteragir(ProgressStage stageNecessario)
    {
        return stageAtual == stageNecessario;
    }

    /// <summary>
    /// Avança para a próxima etapa
    /// </summary>
    public void AvancarEtapa(ProgressStage proximaEtapa)
    {
        // Só avança se for a próxima etapa esperada
        if ((int)proximaEtapa == (int)stageAtual + 1)
        {
            stageAtual = proximaEtapa;

            // Salva o progresso
            PlayerPrefs.SetInt("GameProgress", (int)stageAtual);
            PlayerPrefs.Save();

            Debug.Log("Progresso avançado para: " + stageAtual);
        }
        else
        {
            Debug.LogWarning("Tentando pular etapas! Etapa atual: " + stageAtual + ", tentou avançar para: " + proximaEtapa);
        }
    }

    /// <summary>
    /// Retorna a etapa atual
    /// </summary>
    public ProgressStage GetStageAtual()
    {
        return stageAtual;
    }

    /// <summary>
    /// Reseta o progresso (chamado quando inicia novo jogo)
    /// </summary>
    public void ResetarProgresso()
    {
        stageAtual = ProgressStage.Inicio;
        PlayerPrefs.SetInt("GameProgress", 0);
        PlayerPrefs.Save();

        Debug.Log("Progresso resetado");
    }

    /// <summary>
    /// Retorna uma mensagem de bloqueio baseada na etapa atual
    /// </summary>
    public string GetMensagemBloqueio(ProgressStage stageNecessario)
    {
        // Mensagens customizadas para cada situação
        switch (stageNecessario)
        {
            case ProgressStage.PegouBilheteVaso:
                return "Explore o quarto primeiro.";

            case ProgressStage.FalouComJardineiro:
                return "O bilhete mencionou o jardineiro...";

            case ProgressStage.FalouComDrMauricio:
                return "O jardineiro mencionou o médico da farmácia.";

            case ProgressStage.LeuQuadroAvisos:
                return "Dr. Maurício falou sobre a biblioteca.";

            case ProgressStage.UsouComputador:
                return "Preciso encontrar informações no computador.";

            case ProgressStage.AbriuArmario:
                return "O computador mencionou o armário.";

            case ProgressStage.FalouComBeatriz:
                return "O bilhete mencionou a secretária Beatriz.";

            case ProgressStage.LeuLivroLiberdade:
                return "Beatriz disse para ler os livros...";

            case ProgressStage.FalouComDiretor:
                return "Ainda não terminei de explorar a biblioteca.";

            case ProgressStage.VasculhouLixeira:
                return "O diretor mencionou algo sobre o lixo.";

            case ProgressStage.FalouComEnfermeira:
                return "Preciso seguir a pista da lixeira.";

            case ProgressStage.PodeUsarPortaFinal:
                return "A enfermeira mencionou o X da questão.";

            default:
                return "Não posso fazer isso agora.";
        }
    }
}