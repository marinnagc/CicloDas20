using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NovoJogo()
    {
        // Reseta os dias para 1
        DayManager.ResetarDias();

        // Carrega a cena "Quarto"
        SceneManager.LoadScene("quarto");
    }

    public void Historias()
    {
        SceneManager.LoadScene("Historias");
        Debug.Log("Abrir Histórias");
    }

    public void Instrucoes()
    {
        SceneManager.LoadScene("Instrucoes");
    }

    public void Resolucao()
    {
        SceneManager.LoadScene("TelaCerteza");
        Debug.Log("Abrir Resolução");
    }
}