using UnityEngine;
using TMPro;

public class LivroSegredoNoite : MonoBehaviour
{
    [Header("UI do Livro")]
    public GameObject livroPanel;
    public TextMeshProUGUI livroText;

    [Header("Referências")]
    public TimerController timer;

    [Header("Configuração de Horário")]
    public int horarioMinimo = 18;

    [Header("Mensagens")]
    [TextArea(3, 10)]
    public string conteudoLivro = "SEGREDO DA NOITE\n\n\"Para alcançar a liberdade é necessário procurar.\"\n\n\"Recomendo procurar de barriga cheia.\"";

    [TextArea(2, 4)]
    public string mensagemBloqueado = "Este livro só pode ser lido após as 18:00.\n\nVolte mais tarde...";

    [Header("Tecla de interação")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Dica de Interação (opcional)")]
    public GameObject dicaInteracao;

    private bool playerInside = false;
    private bool panelOpen = false;

    void Start()
    {
        if (livroPanel != null)
            livroPanel.SetActive(false);

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactionKey))
        {
            if (!panelOpen)
                AbrirLivro();
            else
                FecharLivro();
        }
    }

    void AbrirLivro()
    {
        panelOpen = true;

        if (livroPanel != null)
            livroPanel.SetActive(true);

        if (livroText != null)
        {
            if (timer != null)
            {
                int horaAtual = timer.GetHoraInteira();

                if (horaAtual >= horarioMinimo)
                {
                    livroText.text = conteudoLivro;
                    Debug.Log("[LivroSegredo] Livro aberto - conteúdo revelado!");
                }
                else
                {
                    livroText.text = mensagemBloqueado;
                    Debug.Log("[LivroSegredo] Livro bloqueado - hora atual: " + horaAtual);
                }
            }
            else
            {
                livroText.text = conteudoLivro;
            }
        }

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);
    }

    void FecharLivro()
    {
        panelOpen = false;

        if (livroPanel != null)
            livroPanel.SetActive(false);

        if (dicaInteracao != null && playerInside)
            dicaInteracao.SetActive(true);

        Debug.Log("[LivroSegredo] Livro fechado");
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

            if (panelOpen)
                FecharLivro();

            if (dicaInteracao != null)
                dicaInteracao.SetActive(false);
        }
    }
}