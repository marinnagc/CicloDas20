using UnityEngine;
using TMPro;

public class QuadroAvisosInteraction : MonoBehaviour
{
    [Header("UI do Quadro")]
    public GameObject avisosPanel;
    public TextMeshProUGUI avisosText;

    [TextArea(3, 8)]
    public string mensagemAvisos = "AVISOS DO DIA:\n\n• Remédio às 16h\n• Toque de recolher às 20h\n• Não saia do quarto após o horário";

    [Header("Tecla de interação")]
    public KeyCode interactionKey = KeyCode.E;

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
        if (!playerInside) return;

        if (Input.GetKeyDown(interactionKey))
        {
            if (!panelOpen)
                AbrirAvisos();
            else
                FecharAvisos();
        }
    }

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