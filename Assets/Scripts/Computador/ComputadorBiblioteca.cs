using UnityEngine;
using TMPro;

public class ComputadorBiblioteca : MonoBehaviour
{
    [Header("Painéis UI")]
    public GameObject loginPanel;           // Tela de login
    public GameObject conteudoPanel;        // Tela com o relatório

    [Header("Campos de Login")]
    public TMP_InputField loginInput;
    public TMP_InputField senhaInput;
    public TextMeshProUGUI erroText;        // Texto de erro (opcional)

    [Header("Conteúdo do Computador")]
    public TextMeshProUGUI conteudoText;

    [Header("Credenciais Corretas")]
    public string loginCorreto = "HORIZONTE";
    public string senhaCorreta = "1620";

    [Header("Mensagens")]
    [TextArea(5, 15)]
    public string relatorioTexto = "=== RELATÓRIO CIENTÍFICO ===\n\nComposto 16-20 induz esquecimento de 24h.\n\n=== NOTA TÉCNICA ===\n\nAntídoto A-7 guardado no armário de suprimentos da Farmácia.\n\nSenha: 0416";

    [TextArea(2, 4)]
    public string mensagemErro = "Login ou senha incorretos.\nTente novamente.";

    [Header("Tecla de interação")]
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode closeKey = KeyCode.Escape;

    [Header("Dica de Interação (opcional)")]
    public GameObject dicaInteracao;

    private bool playerInside = false;
    private bool computerOpen = false;
    private bool loggedIn = false;

    void Start()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (conteudoPanel != null)
            conteudoPanel.SetActive(false);

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);

        if (erroText != null)
            erroText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        // Abrir computador
        if (Input.GetKeyDown(interactionKey) && !computerOpen)
        {
            AbrirComputador();
        }

        // Fechar com ESC
        if (Input.GetKeyDown(closeKey) && computerOpen)
        {
            FecharComputador();
        }
    }

    void AbrirComputador()
    {
        computerOpen = true;

        if (loginPanel != null)
            loginPanel.SetActive(true);

        // Limpa os campos
        if (loginInput != null)
            loginInput.text = "";

        if (senhaInput != null)
            senhaInput.text = "";

        if (erroText != null)
            erroText.gameObject.SetActive(false);

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);

        Debug.Log("[Computador] Tela de login aberta");
    }

    void FecharComputador()
    {
        computerOpen = false;
        loggedIn = false;

        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (conteudoPanel != null)
            conteudoPanel.SetActive(false);

        if (dicaInteracao != null && playerInside)
            dicaInteracao.SetActive(true);

        Debug.Log("[Computador] Computador fechado");
    }

    // Chamar este método pelo botão de Login na UI
    public void TentarLogin()
    {
        string loginDigitado = loginInput != null ? loginInput.text.Trim().ToUpper() : "";
        string senhaDigitada = senhaInput != null ? senhaInput.text.Trim() : "";

        if (loginDigitado == loginCorreto.ToUpper() && senhaDigitada == senhaCorreta)
        {
            // Login correto
            LoginSucesso();
        }
        else
        {
            // Login incorreto
            LoginFalhou();
        }
    }

    void LoginSucesso()
    {
        loggedIn = true;

        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (conteudoPanel != null)
            conteudoPanel.SetActive(true);

        if (conteudoText != null)
            conteudoText.text = relatorioTexto;

        Debug.Log("[Computador] Login bem-sucedido!");
    }

    void LoginFalhou()
    {
        if (erroText != null)
        {
            erroText.text = mensagemErro;
            erroText.gameObject.SetActive(true);
        }

        // Limpa a senha
        if (senhaInput != null)
            senhaInput.text = "";

        Debug.Log("[Computador] Login falhou!");
    }

    // Chamar este método pelo botão de Fechar no conteúdo
    public void BotaoFechar()
    {
        FecharComputador();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (dicaInteracao != null)
                dicaInteracao.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (computerOpen)
                FecharComputador();

            if (dicaInteracao != null)
                dicaInteracao.SetActive(false);
        }
    }
}
