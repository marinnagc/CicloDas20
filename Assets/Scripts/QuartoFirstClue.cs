using UnityEngine;
using TMPro;

public class QuartoFirstClue : MonoBehaviour
{
    [Header("UI do Cartão")]
    [SerializeField] private GameObject cardPanel;   // PanelFirstClue
    [SerializeField] private TMP_Text cardText;      // TextFirstClue

    [TextArea(2, 4)]
    [SerializeField]
    private string message =
        "Se você ainda lembra, procure o jardineiro no pátio.";

    [Header("Interação")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactHint; // texto 'E para interagir'

    private bool playerInside = false;
    private bool clueRead = false;

    private void Start()
    {
        if (cardPanel != null)
            cardPanel.SetActive(false);

        if (interactHint != null)
            interactHint.SetActive(false);
    }

    private void Update()
    {
        // só reage se o player estiver dentro do trigger do vaso
        if (!playerInside) return;

        if (Input.GetKeyDown(interactKey))
        {
            // primeira vez: mostra a clue
            if (!clueRead)
            {
                ShowClue();
            }
            else
            {
                // alterna entre ver/fechar o cartão
                bool isOpen = cardPanel != null && cardPanel.activeSelf;

                if (isOpen)
                {
                    // fecha cartão e volta a mostrar o hint
                    if (cardPanel != null)
                        cardPanel.SetActive(false);

                    if (interactHint != null)
                        interactHint.SetActive(true);
                }
                else
                {
                    // abre cartão e esconde hint
                    if (interactHint != null)
                        interactHint.SetActive(false);

                    if (cardPanel != null)
                    {
                        cardPanel.SetActive(true);
                        if (cardText != null)
                            cardText.text = message;
                    }
                }
            }
        }
    }

    private void ShowClue()
    {
        clueRead = true;
        StoryState.FirstClueTaken = true;  // marca que a primeira pista foi lida

        // quando abre o cartão, some o hint
        if (interactHint != null)
            interactHint.SetActive(false);

        if (cardPanel != null)
            cardPanel.SetActive(true);

        if (cardText != null)
            cardText.text = message;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // entrando na área: mostra somente o hint
        if (interactHint != null)
            interactHint.SetActive(true);

        if (cardPanel != null)
            cardPanel.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        // saindo da área: esconde tudo
        if (interactHint != null)
            interactHint.SetActive(false);

        if (cardPanel != null)
            cardPanel.SetActive(false);
    }
}
