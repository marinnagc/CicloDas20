using UnityEngine;
using TMPro; // se for usar TextMeshPro

public class JardineiroInteraction : MonoBehaviour
{
    [Header("UI de Diálogo")]
    public GameObject dialogPanel;         // painel inteiro
    public TextMeshProUGUI dialogText;     // texto dentro do painel

    [TextArea(2, 5)]
    public string mensagem = "Você não deveria estar aqui tão tarde... O jardim guarda segredos.";

    [Header("Tecla de interação")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInside = false;
    private bool dialogOpen = false;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
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

        // Se quiser travar o movimento do player enquanto fala,
        // você pode pegar o PlayerController e desabilitar aqui.
    }

    void FecharDialogo()
    {
        dialogOpen = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        // Aqui você pode reabilitar o movimento do player, se tiver travado.
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            // Aqui você pode mostrar uma mensagem "Aperte E para falar"
            // em outro pequeno texto na tela, se quiser.
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
