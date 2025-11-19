using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject pausePanel;    // arrasta o PausePanel aqui

    private bool isPaused = false;

    void Awake()
    {
        // Garante que só exista UM PauseMenuController no jogo inteiro
        var objs = FindObjectsOfType<PauseMenuController>();
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Faz esse objeto sobreviver à troca de cenas
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Garante que o jogo começa despausado na primeira cena
        ResetPauseState();
    }

    /// <summary>
    /// Deixa tudo no estado "despausado"
    /// </summary>
    private void ResetPauseState()
    {
        isPaused = false;
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
        ResetPauseState();
    }

    // Botão "Menu Inicial / Desistir"
    public void GoToMainMenu()
    {
        // garante que vamos sair despausados e sem painel aberto
        ResetPauseState();

        // Mata o DayNightController (cronômetro / dia)
        var dayNight = FindObjectOfType<DayNightController>();
        if (dayNight != null)
        {
            Destroy(dayNight.gameObject);
        }

        // Mata o gerenciador de som (muda o tipo pro nome do seu script!)
        var soundManager = FindObjectOfType<SoundAndAlarmManager>();
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
