using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DoorPasswordInteraction : MonoBehaviour, IInteractable
{
    [Header("UI da Senha")]
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    [Header("Configuração")]
    public string correctPassword = "1620";
    public string winSceneName = "Ganhou";

    private bool playerInside = false;
    private bool panelOpen = false;
    private bool doorOpened = false;

    void Start()
    {
        passwordPanel?.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    // =====================================================================
    //   BOTÃO CLOSE – para ser chamado no OnClick do botão de fechar
    // =====================================================================
    public void ClosePanelByButton()
    {
        ClosePanel();
    }

    // =====================================================================
    //            Detectar se clique foi em UI "real" (input/botão)
    //            (fica aqui se precisar no futuro, mas não fecha mais)
    // =====================================================================
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
                return true; // clicou em input/button
        }

        return false; // clicou em painel vazio da UI
    }

    // =====================================================================
    //                                UPDATE
    // =====================================================================
    void Update()
    {
        if (!panelOpen) return;

        // ➜ NÃO fecha mais clicando fora
        // if (Input.GetMouseButtonDown(0)) { ... }
    }

    // =====================================================================
    //                      BLOQUEAR E quando digitando
    // =====================================================================
    bool IsTypingInInput()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject != null &&
               EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null;
    }

    // =====================================================================
    //                        IInteractable (PC + MOBILE)
    // =====================================================================
    public void Interact(GameObject interactor)
    {
        if (!playerInside || doorOpened)
            return;

        // Se estiver digitando → ignorar E
        if (IsTypingInInput())
            return;

        // Abrir painel se estiver fechado
        if (!panelOpen)
        {
            OpenPanel();
        }
        // Painel aberto → agora NÃO fecha mais com E
        else
        {
            // não chamar ClosePanel();
        }
    }

    public string GetPrompt()
    {
        if (doorOpened) return "";
        return "Digitar senha";
    }

    // =====================================================================
    //                            Abrir / Fechar
    // =====================================================================
    void OpenPanel()
    {
        panelOpen = true;

        passwordPanel?.SetActive(true);
        if (passwordInput != null) passwordInput.text = "";
        if (feedbackText != null) feedbackText.text = "";
    }

    void ClosePanel()
    {
        panelOpen = false;

        passwordPanel?.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";
    }

    // =====================================================================
    //                    Confirmar senha (botão)
    // =====================================================================
    public void CheckPassword()
    {
        if (passwordInput == null || feedbackText == null) return;

        if (passwordInput.text == correctPassword)
        {
            feedbackText.text = "Porta liberada!";
            ClosePanel();
            doorOpened = true;

            Debug.Log("Senha correta — carregando cena de vitória!");
            SceneManager.LoadScene(winSceneName);
        }
        else
        {
            feedbackText.text = "Senha incorreta!";
        }
    }

    // =====================================================================
    //                      Trigger de proximidade
    // =====================================================================
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            ClosePanel();
        }
    }
}
