using UnityEngine;
using TMPro;   // para TMP_InputField e TextMeshProUGUI

public class LoginPasswordInteraction : MonoBehaviour
{
    [Header("UI do bilhete (imagem que aparece DEPOIS do login)")]
    public GameObject dialogPanel;          // painel com o post-it / info

    [Header("UI de Login + Senha")]
    public GameObject loginPanel;           // painel com usuario + senha
    public TMP_InputField userInput;        // campo de usuário
    public TMP_InputField passwordInput;    // campo de senha
    public TextMeshProUGUI feedbackText;    // texto de erro ("login ou senha incorretos")

    [Header("Credenciais corretas")]
    public string correctUser = "beatriz";
    public string correctPassword = "0416";

    [Header("Configuração")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInside = false;
    private bool dialogOpen = false;
    private bool askingLogin = false;

    void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    void Update()
    {
        if (!playerInside) return;

        // Aperta E para iniciar a interação
        if (Input.GetKeyDown(interactionKey))
        {
            // Nada aberto ainda → abrir painel de login
            if (!dialogOpen && !askingLogin)
            {
                OpenLoginPanel();
            }
            // Se o bilhete já estiver aberto, E fecha
            else if (dialogOpen)
            {
                CloseDialog();
            }
        }
    }

    void OpenLoginPanel()
    {
        askingLogin = true;

        if (loginPanel != null) loginPanel.SetActive(true);
        if (dialogPanel != null) dialogPanel.SetActive(false);

        if (userInput != null) userInput.text = "";
        if (passwordInput != null) passwordInput.text = "";
        if (feedbackText != null) feedbackText.text = "";
    }

    public void ConfirmLogin()
    {
        if (userInput == null || passwordInput == null)
        {
            Debug.LogWarning("LoginPasswordImageInteraction: Inputs não ligados no Inspector!");
            return;
        }

        string enteredUser = userInput.text.Trim();
        string enteredPass = passwordInput.text.Trim();

        Debug.Log($"[Login] User digitado: '{enteredUser}', Senha digitada: '{enteredPass}'");

        if (enteredUser == correctUser && enteredPass == correctPassword)
        {
            Debug.Log("[Login] Login e senha CORRETOS!");

            if (loginPanel != null) loginPanel.SetActive(false);

            if (dialogPanel != null) dialogPanel.SetActive(true);

            dialogOpen = true;
            askingLogin = false;

            if (feedbackText != null)
                feedbackText.text = "";
        }
        else
        {
            Debug.Log("[Login] Login ou senha INCORRETOS!");
            if (feedbackText != null)
                feedbackText.text = "Login ou senha incorretos!";
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
        askingLogin = false;
        dialogOpen = false;

        if (loginPanel != null) loginPanel.SetActive(false);
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }
}
