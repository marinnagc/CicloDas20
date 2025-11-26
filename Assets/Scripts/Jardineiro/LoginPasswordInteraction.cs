using UnityEngine;
using TMPro;                    // para TMP_InputField e TextMeshProUGUI
using UnityEngine.EventSystems; // para detectar clique em UI

public class LoginPasswordInteraction : MonoBehaviour, IInteractable
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

    private bool playerInside = false;
    private bool dialogOpen = false;   // bilhete pós-login aberto
    private bool askingLogin = false;  // painel de login aberto

    void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }
    bool ClickIsOnRealUI()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            return false;

        // Pega o objeto UI clicado
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Se qualquer UI for Selectable, NÃO FECHA
        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<UnityEngine.UI.Selectable>() != null)
                return true;
        }

        // Clicou em UI que não é Selectable (tipo um painel)? Pode FECHAR
        return false;
    }

    void Update()
    {
        if (!dialogOpen && !askingLogin) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Se clicou em input, botão, etc → NÃO FECHA
            if (ClickIsOnRealUI())
                return;

            // Qualquer clique FORA desses elementos → fecha
            if (dialogOpen)
                CloseDialog();

            if (askingLogin)
                CloseLoginPanel();
        }
    }
    bool IsTypingInInput()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject != null &&
               EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null;
    }


    // ============================================================
    //           IInteractable – chamado pelo Player/E
    // ============================================================
    public void Interact(GameObject interactor)
    {
        // Se estiver digitando → NÃO FAZ NADA
        if (IsTypingInInput())
            return;

        if (!playerInside) return;

        if (!askingLogin && !dialogOpen)
        {
            OpenLoginPanel();
        }
        else if (dialogOpen)
        {
            CloseDialog();
        }
        else if (askingLogin)
        {
            CloseLoginPanel();
        }
    }


    public string GetPrompt()
    {
        if (!askingLogin && !dialogOpen)
            return "Acessar computador";
        if (dialogOpen)
            return "Fechar bilhete";
        if (askingLogin)
            return "Fechar login";
        return "";
    }

    // ============================================================
    //                   Abrir / Fechar paineis
    // ============================================================
    void OpenLoginPanel()
    {
        askingLogin = true;

        if (loginPanel != null) loginPanel.SetActive(true);
        if (dialogPanel != null) dialogPanel.SetActive(false);

        if (userInput != null) userInput.text = "";
        if (passwordInput != null) passwordInput.text = "";
        if (feedbackText != null) feedbackText.text = "";
    }

    void CloseLoginPanel()
    {
        askingLogin = false;

        if (loginPanel != null) loginPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";
    }

    void OpenDialog()
    {
        dialogOpen = true;

        if (dialogPanel != null) dialogPanel.SetActive(true);
    }

    void CloseDialog()
    {
        dialogOpen = false;

        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    // ============================================================
    //                 Botão de confirmar login
    // ============================================================
    public void ConfirmLogin()
    {
        if (userInput == null || passwordInput == null)
        {
            Debug.LogWarning("LoginPasswordInteraction: Inputs não ligados no Inspector!");
            return;
        }

        string enteredUser = userInput.text.Trim();
        string enteredPass = passwordInput.text.Trim();

        Debug.Log($"[Login] User digitado: '{enteredUser}', Senha digitada: '{enteredPass}'");

        if (enteredUser == correctUser && enteredPass == correctPassword)
        {
            Debug.Log("[Login] Login e senha CORRETOS!");

            CloseLoginPanel();
            OpenDialog();

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

    // ============================================================
    //                Trigger de proximidade
    // ============================================================
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
