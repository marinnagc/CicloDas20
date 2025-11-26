using UnityEngine;
using TMPro;

public class QuadroAvisosInteraction : MonoBehaviour, IInteractable
{
    [Header("UI do Quadro")]
    public GameObject avisosPanel;
    public TextMeshProUGUI avisosText;

    [TextArea(3, 8)]
    public string mensagemAvisos = "AVISOS DO DIA:\n\n• Remédio às 16h\n• Toque de recolher às 20h\n• Não saia do quarto após o horário";

    [Header("Dica de Interação (opcional)")]
    public GameObject dicaInteracao; // texto "Aperte E para ler"

    private bool playerInside = false;
    private bool panelOpen = false;

    void Start()
    {
        if (avisosPanel != null)
            avisosPanel.SetActive(false);

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);
    }

    void Update()
    {
        // 👉 Fecha ao tocar em qualquer lugar da tela
        if (panelOpen && Input.GetMouseButtonDown(0))
        {
            FecharAvisos();
        }
    }

    // ================================================================
    //                MÉTODO IInteractable – chamado pelo Player
    // ================================================================
    public void Interact(GameObject interactor)
    {
        // Para abrir → precisa estar perto
        if (!panelOpen)
        {
            if (!playerInside) return;
            AbrirAvisos();
        }
        else
        {
            // Para fechar → pode fechar sempre
            FecharAvisos();
        }
    }

    public string GetPrompt()
    {
        return "Ler avisos";
    }

    // ================================================================
    //                     ABRIR E FECHAR O PAINEL
    // ================================================================

    void AbrirAvisos()
    {
        panelOpen = true;

        if (avisosPanel != null)
            avisosPanel.SetActive(true);

        if (avisosText != null)
            avisosText.text = mensagemAvisos;

        if (dicaInteracao != null)
            dicaInteracao.SetActive(false);

        Debug.Log("[QuadroAvisos] Painel aberto");
    }

    void FecharAvisos()
    {
        panelOpen = false;

        if (avisosPanel != null)
            avisosPanel.SetActive(false);

        if (dicaInteracao != null && playerInside)
            dicaInteracao.SetActive(true);

        Debug.Log("[QuadroAvisos] Painel fechado");
    }

    // ================================================================
    //                     DETECÇÃO DE PROXIMIDADE
    // ================================================================

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
                FecharAvisos();

            if (dicaInteracao != null)
                dicaInteracao.SetActive(false);
        }
    }
}
