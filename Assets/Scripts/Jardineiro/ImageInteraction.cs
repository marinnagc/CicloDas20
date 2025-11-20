using UnityEngine;

public class ImageInteraction : MonoBehaviour
{
    [Header("UI a ser exibida (a imagem do bilhete)")]
    public GameObject dialogPanel;

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
                Abrir();
            else
                Fechar();
        }
    }

    void Abrir()
    {
        dialogOpen = true;

        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        // OPCIONAL → congela o player enquanto lê
        /*
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;
        }
        */
    }

    void Fechar()
    {
        dialogOpen = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        // OPCIONAL → libera movimento
        /*
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = true;
        }
        */
    }

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
            Fechar();
        }
    }
}
