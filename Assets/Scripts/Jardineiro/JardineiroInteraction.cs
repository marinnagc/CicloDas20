using UnityEngine;
using TMPro;

public class JardineiroInteraction : MonoBehaviour, IInteractable
{
    [Header("UI de Diálogo")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    [TextArea(2, 5)]
    public string mensagem = "Você não deveria estar aqui tão tarde... O jardim guarda segredos.";

    private bool dialogOpen = false;
    private bool playerInside = false;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void Update()
    {
        // 👉 Se o bilhete estiver aberto e o jogador clicar em QUALQUER lugar, fecha
        if (dialogOpen && Input.GetMouseButtonDown(0))
        {
            FecharDialogo();
        }
    }

    // Chamado pelo PlayerInteraction (botão E ou tecla E)
    public void Interact(GameObject interactor)
    {
        // ABRIR só se player estiver perto
        if (!dialogOpen)
        {
            if (!playerInside) return;
            AbrirDialogo();
        }
        // FECHAR sempre (mesmo se playerInside for false)
        else
        {
            FecharDialogo();
        }
    }

    public string GetPrompt()
    {
        return "Ler bilhete";   // ou "Falar", etc.
    }

    void AbrirDialogo()
    {
        dialogOpen = true;

        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        if (dialogText != null)
            dialogText.text = mensagem;
    }

    void FecharDialogo()
    {
        dialogOpen = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (dialogOpen)
                FecharDialogo();
        }
    }
}
