using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NovoJogo()
    {
        // Carrega a cena "Quarto"
        SceneManager.LoadScene("quarto");
    }

    public void Historias()
    {
        Debug.Log("Abrir Histórias");
    }

    public void Instrucoes()
    {
        Debug.Log("Abrir Instruções");
    }

    public void Resolucao()
    {
        Debug.Log("Abrir Resolução");
    }
}
