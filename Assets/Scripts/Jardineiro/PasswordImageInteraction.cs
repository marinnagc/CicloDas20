using UnityEngine;
using TMPro;   // para TMP_InputField e TextMeshProUGUI

public class PasswordImageInteraction : MonoBehaviour
{
    [Header("UI do bilhete (imagem)")]
    public GameObject dialogPanel;          // painel com o post-it

    [Header("UI da senha")]
    public GameObject passwordPanel;        // painel com input de senha
    public TMP_InputField passwordInput;    // campo de texto da senha
    public TextMeshProUGUI feedbackText;    // opcional: texto de erro ("senha errada")

    [Header("Configuração")]
    public string correctPassword = "1234";
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInside = false;
    private bool dialogOpen = false;
    private bool askingPassword = false;

    void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (passwordPanel != null) passwordPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    void Update()
    {
        if (!playerInside) return;

        // Aperta E para iniciar a interação (abrir painel de senha)
        if (Input.GetKeyDown(interactionKey))
        {
            // se nada estiver aberto, abre o painel de senha
            if (!dialogOpen && !askingPassword)
            {
                OpenPasswordPanel();
            }
            // se já estiver com o bilhete aberto, E fecha
            else if (dialogOpen)
            {
                CloseDialog();
            }
        }
    }

    void OpenPasswordPanel()
    {
        askingPassword = true;
        if (passwordPanel != null) passwordPanel.SetActive(true);
        if (passwordInput != null) passwordInput.text = "";
        if (feedbackText != null) feedbackText.text = "";
    }

    public void ConfirmPassword()
    {
        if (passwordInput == null) return;

        string entered = passwordInput.text;

        if (entered == correctPassword)
        {
            // senha correta → fecha painel de senha, abre bilhete
            if (passwordPanel != null) passwordPanel.SetActive(false);
            askingPassword = false;

            if (dialogPanel != null) dialogPanel.SetActive(true);
            dialogOpen = true;
        }
        else
        {
            // senha errada
            if (feedbackText != null)
                feedbackText.text = "Senha incorreta!";
            else
                Debug.Log("Senha incorreta!");
        }
    }

    void CloseDialog()
    {
        dialogOpen = false;
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        askingPassword = false;
        dialogOpen = false;

        if (passwordPanel != null) passwordPanel.SetActive(false);
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }
}
