using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject pausePanel;    // arrasta o PausePanel aqui

    private bool isPaused = false;

    void Start()
    {
        // Garante que o jogo começa despausado
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // Chamar essa função no botão de PAUSAR
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f; // pausa TUDO que depende de tempo
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    // Botão "Retomar"
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // volta ao normal
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // Botão "Menu Inicial / Desistir"
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // Mata o DayNightController (cronômetro / dia)
        var dayNight = FindObjectOfType<DayNightController>();
        if (dayNight != null)
        {
            Destroy(dayNight.gameObject);
        }

        // Mata o gerenciador de som (muda o tipo pro nome do seu script!)
        var soundManager = FindObjectOfType<SoundAndAlarmManager>();  // ou Som_fundo, BgmManager, etc.
        if (soundManager != null)
        {
            Destroy(soundManager.gameObject);
        }

        SceneManager.LoadScene("MenuInicial");
    }

    // Opcional: permitir pausar/despausar pela tecla ESC
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }
}
