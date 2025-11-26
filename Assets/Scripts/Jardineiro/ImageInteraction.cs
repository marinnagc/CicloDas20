using UnityEngine;

public class ImageInteraction : MonoBehaviour, IInteractable
{
    [Header("UI a ser exibida (a imagem do bilhete)")]
    public GameObject dialogPanel;

    private bool dialogOpen = false;
    private bool playerInside = false;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void Update()
    {
        // 👉 Se imagem aberta, e o jogador clicar/tocar em qualquer lugar da tela, fecha
        if (dialogOpen && Input.GetMouseButtonDown(0))
        {
            Fechar();
        }
    }

    // ==================================================================
    //           MÉTODOS OBRIGATÓRIOS DO IInteractable
    // ==================================================================

    public void Interact(GameObject interactor)
    {
        // Se ainda não abriu → só abre se o player estiver perto
        if (!dialogOpen)
        {
            if (!playerInside) return;
            Abrir();
        }
        else
        {
            // Se já está aberto → fechar SEMPRE, mesmo se playerInside for false
            Fechar();
        }
    }

    public string GetPrompt()
    {
        return "Ler bilhete";   // texto opcional para UI de prompt
    }

    // ==================================================================
    //               Funções de abrir/fechar o painel
    // ==================================================================

    void Abrir()
    {
        dialogOpen = true;

        if (dialogPanel != null)
            dialogPanel.SetActive(true);
    }

    void Fechar()
    {
        dialogOpen = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    // ==================================================================
    //                  Trigger para detectar proximidade
    // ==================================================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Se sair de perto e estiver aberto, fecha
            if (dialogOpen)
                Fechar();
        }
    }
}
