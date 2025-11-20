using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;   // <-- necessário para trocar de cena

public class DoorPasswordInteraction : MonoBehaviour
{
    [Header("UI da Senha")]
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    [Header("Configuração")]
    public string correctPassword = "1620";
    public KeyCode interactionKey = KeyCode.E;
    public string winSceneName = "Ganhou";   // <-- nome da cena de vitória

    private bool playerInside = false;
    private bool doorOpened = false;

    void Start()
    {
        if (passwordPanel != null)
            passwordPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";
    }

    void Update()
    {
        if (!playerInside || doorOpened) return;

        if (Input.GetKeyDown(interactionKey))
        {
            passwordPanel.SetActive(true);
            feedbackText.text = "";
            passwordInput.text = "";
        }
    }

    public void CheckPassword()
    {
        if (passwordInput.text == correctPassword)
        {
            feedbackText.text = "Porta liberada!";
            passwordPanel.SetActive(false);
            doorOpened = true;

            Debug.Log("Senha correta — carregando cena de vitória!");

            // 🔥 Troca de cena imediata
            SceneManager.LoadScene(winSceneName);
        }
        else
        {
            feedbackText.text = "Senha incorreta!";
        }
    }

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
            passwordPanel.SetActive(false);
        }
    }
}
