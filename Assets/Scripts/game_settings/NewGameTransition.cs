using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class NewGameTransition : MonoBehaviour
{
    public Image fadeScreen;
    public TMP_Text dayText;
    public float fadeDuration = 1.5f;
    public float dayTextDuration = 2f;
    public string nextSceneName = "quarto"; // Nome da sua cena do quarto

    public void StartNewGame()
    {
        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition()
    {
        // Ativa a tela preta
        fadeScreen.gameObject.SetActive(true);
        Color color = fadeScreen.color;
        color.a = 0;
        fadeScreen.color = color;

        // Faz fade in para preto
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeScreen.color = color;
            yield return null;
        }

        // Mostra o texto "DAY 1"
        dayText.gameObject.SetActive(true);
        yield return new WaitForSeconds(dayTextDuration);

        // Carrega a próxima cena
        SceneManager.LoadScene(nextSceneName);
    }
}
