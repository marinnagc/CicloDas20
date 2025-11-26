using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LoginPasswordInteraction : MonoBehaviour, IInteractable
{
    [Header("UI do bilhete (imagem que aparece DEPOIS do login)")]
    public GameObject dialogPanel;

    [Header("UI de Login + Senha")]
    public GameObject loginPanel;
    public TMP_InputField userInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    [Header("Credenciais corretas")]
    public string correctUser = "beatriz";
    public string correctPassword = "0416";

    private bool playerInside = false;
    private bool dialogOpen = false;     // bilhete pós-login
    private bool askingLogin = false;    // painel de login

    void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    // ======================================================================
    //              BOTÕES CLOSE — chamados pelo OnClick do botão
    // ======================================================================
    public void CloseLoginByButton()
    {
        CloseLoginPanel();
    }

    public void CloseDialogByButton()
    {
        CloseDialog();
    }

    // ======================================================================
    //          Função auxiliar para checar cliques em UI real
    // ======================================================================
    bool ClickIsOnRealUI()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            return false;

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<UnityEngine.UI.Selectable>() != null)
                return true;
        }

        return false;
    }

    // ======================================================================
    //                   UPDATE — clique fora fecha só o bilhete
    // ======================================================================
    void Update()
    {
        if (!dialogOpen && !askingLogin)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // se clicou em botão/input → não fecha nada
            if (ClickIsOnRealUI())
                return;

            // 👉 Apenas o bilhete pode ser fechado clicando fora
            if (dialogOpen)
                CloseDialog();

            // ❌ NÃO fechar mais login clicando fora
        }
    }

    bool IsTypingInInput()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject != null &&
               EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null;
    }

    // ======================================================================
    //                        INTERAÇÃO COM O PLAYER (tecla E)
    // ======================================================================
    public void Interact(GameObject interactor)
    {
        if (IsTypingInInput())
            return;

        if (!playerInside)
            return;

        // E abre login se nada estiver aberto
        if (!askingLogin && !dialogOpen)
        {
            OpenLoginPanel();
        }
        // E fecha bilhete
        else if (dialogOpen)
        {
            CloseDialog();
        }
        // ❌ E NÃO fecha mais o login automaticamente
        else if (askingLogin)
        {
            // NÃO chamar CloseLoginPanel();
            // agora login só fecha por botão CloseLoginByButton()
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

    // ======================================================================
    //                   ABRIR / FECHAR PAINÉIS
    // ======================================================================
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

    // ======================================================================
    //                   BOTÃO CONFIRMAR LOGIN
    // ======================================================================
    public void ConfirmLogin()
    {
        if (userInput == null || passwordInput == null)
        {
            Debug.LogWarning("LoginPasswordInteraction: Inputs não ligados no Inspector!");
            return;
        }

        string enteredUser = userInput.text.Trim();
        string enteredPass = passwordInput.text.Trim();

        if (enteredUser == correctUser && enteredPass == correctPassword)
        {
            CloseLoginPanel();
            OpenDialog();

            if (feedbackText != null)
                feedbackText.text = "";
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Login ou senha incorretos!";
        }
    }

    // ======================================================================
    //                   TRIGGER DO PLAYER
    // ======================================================================
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
