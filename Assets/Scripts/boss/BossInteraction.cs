using UnityEngine;
using TMPro;

public class BossInteraction : MonoBehaviour
{
    [Header("UI de Diálogo")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    [TextArea(2, 5)]
    public string mensagem = "A comida estava péssima, joguei tudo fora.";

    [Header("Tecla de interação")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Dica de Interação (opcional)")]
    public GameObject dicaInteracao;

    private bool playerInside = false;
    private bool dialogOpen = false;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactionKey))
        {
            if (!dialogOpen)
                AbrirDialogo();
            else
                FecharDialogo();
        }
    }

    void AbrirDialogo()
    {
        dialogOpen = true;

        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        if (dialogText != null)
            dialogText.text = mensagem;

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);

        Debug.Log("[Diretor] Diálogo aberto");
    }

    void FecharDialogo()
    {
        dialogOpen = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (dicaInteracao != null && playerInside)
            dicaInteracao.SetActive(true);

        Debug.Log("[Diretor] Diálogo fechado");
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

            if (dialogOpen)
                FecharDialogo();

            if (dicaInteracao != null)
                dicaInteracao.SetActive(false);
        }
    }
}