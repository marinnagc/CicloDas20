using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Configuração da porta")]
    public string sceneToLoad; // Nome da próxima cena (igual ao Build Settings)

    bool isTransitioning = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        isTransitioning = true;

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.StartCoroutine(ScreenFader.Instance.FadeOut());

        SceneManager.LoadScene(sceneToLoad);

        // Espera 1 frame pra garantir que a nova cena carregou
        yield return null;

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.StartCoroutine(ScreenFader.Instance.FadeIn());

        isTransitioning = false;
    }
}
