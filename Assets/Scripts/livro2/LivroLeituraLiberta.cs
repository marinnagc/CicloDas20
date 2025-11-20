using UnityEngine;
using TMPro;

public class LivroLeituraLiberta : MonoBehaviour
{
    [Header("UI do Livro")]
    public GameObject livroPanel;
    public TextMeshProUGUI livroText;

    [Header("Mensagem do Livro")]
    [TextArea(5, 12)]
    public string conteudoLivro = "A LEITURA LIBERTA\n\nReceita para uma boa saúde mental:\n\n• 1 hora de almoço\n• 5 momentos de reflexão\n• 3 capítulos de um bom livro\n• 8 copos de água\n• 0 remédios\n\nSeguir a receita uma vez ao dia.";

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
            livroText.text = conteudoLivro;

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);

        Debug.Log("[LivroLiberta] Livro aberto - Senha: 15380");
    }

    void FecharLivro()
    {
        panelOpen = false;

        if (livroPanel != null)
            livroPanel.SetActive(false);

        if (dicaInteracao != null && playerInside)
            dicaInteracao.SetActive(true);

        Debug.Log("[LivroLiberta] Livro fechado");
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