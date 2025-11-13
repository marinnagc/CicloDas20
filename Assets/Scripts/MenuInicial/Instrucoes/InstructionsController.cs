using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsController : MonoBehaviour
{
    public void VoltarMenu()
    {
        // Carrega a cena do menu principal
        SceneManager.LoadScene("MenuInicial");
    }
}
