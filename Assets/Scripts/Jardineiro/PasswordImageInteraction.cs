using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordImageInteraction : MonoBehaviour, IInteractable
{
    [Header("UI do bilhete (imagem)")]
    public GameObject dialogPanel;

    [Header("UI da senha")]
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    [Header("Configuração")]
    public string correctPassword = "1234";

    private bool playerInside = false;
    private bool dialogOpen = false;       // bilhete aberto
    private bool askingPassword = false;   // painel de senha aberto

    void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (passwordPanel != null) passwordPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    // ======================================================================
    //           BOTÕES CLOSE – chamados via OnClick no botão
    // ======================================================================
    public void ClosePasswordByButton()
    {
        ClosePasswordPanel();
    }

    public void CloseDialogByButton()
    {
        CloseDialog();
    }

    // ============================================================
    //         Detectar se clique está em UI *interativa*
    // ============================================================
    bool ClickIsOnRealUI()
    {
        // Não clicou em UI → beleza
        if (!EventSystem.current.IsPointerOverGameObject())
            return false;

        // Coletar os objetos UI atingidos pelo clique
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Se clicou num TMP_InputField ou Button → NÃO FECHA
        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<UnityEngine.UI.Selectable>() != null)
                return true; // clicou num UI "real" (input, botão, slider, etc)
        }

        return false; // clicou em UI "vazia" (painel) → pode fechar
    }

    // ============================================================
    //                         UPDATE
    // ============================================================
    void Update()
    {
        if (!dialogOpen && !askingPassword) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Se o clique foi em elemento interativo (input/botão) → não fechar
            if (ClickIsOnRealUI())
                return;

            // 👉 Agora: clicar fora só fecha o bilhete
            if (dialogOpen)
                CloseDialog();

            // ❌ NÃO fecha mais painel de senha ao clicar fora
            // if (askingPassword)
            //     ClosePasswordPanel();
        }
    }

    // ============================================================
    //         IInteractable – chamado pelo Player/E
    // ============================================================
    public void Interact(GameObject interactor)
    {
        // Se estiver digitando, NÃO fecha nem abre nada
        if (IsTypingInInput())
            return;

        if (!playerInside) return;

        // Nada aberto → abrir painel de senha
        if (!askingPassword && !dialogOpen)
        {
            OpenPasswordPanel();
        }
        // Bilhete aberto → fechar com E
        else if (dialogOpen)
        {
            CloseDialog();
        }
        // Senha aberta → AGORA NÃO FECHA MAIS COM E
        else if (askingPassword)
        {
            // Não chamar ClosePasswordPanel();
            // painel de senha só fecha pelo botão ClosePasswordByButton()
        }
    }

    // Bloqueia o E enquanto escrever no InputField
    bool IsTypingInInput()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject != null &&
               EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null;
    }

    public string GetPrompt()
    {
        if (!askingPassword && !dialogOpen)
            return "Inserir senha";
        if (dialogOpen)
            return "Fechar bilhete";
        if (askingPassword)
            return "Fechar senha";
        return "";
    }

    // ============================================================
    //                   Abrir / Fechar paineis
    // ============================================================
    void OpenPasswordPanel()
    {
        askingPassword = true;

        passwordPanel?.SetActive(true);
        if (passwordInput != null) passwordInput.text = "";
        if (feedbackText != null) feedbackText.text = "";
    }

    void ClosePasswordPanel()
    {
        askingPassword = false;

        passwordPanel?.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    void OpenDialog()
    {
        dialogOpen = true;
        dialogPanel?.SetActive(true);
    }

    void CloseDialog()
    {
        dialogOpen = false;
        dialogPanel?.SetActive(false);
    }

    // ============================================================
    //                Confirmar senha
    // ============================================================
    public void ConfirmPassword()
    {
        if (passwordInput == null) return;

        string entered = passwordInput.text.Trim();

        if (entered == correctPassword)
        {
            ClosePasswordPanel();
            OpenDialog();
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Senha incorreta!";
        }
    }

    // ============================================================
    //                   Trigger de proximidade
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
        dialogOpen = false;
        askingPassword = false;

        passwordPanel?.SetActive(false);
        dialogPanel?.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }
}
