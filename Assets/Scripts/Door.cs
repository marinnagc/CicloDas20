using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Configuração da porta")]
    public string sceneToLoad;      // nome da cena destino (igual ao Build Settings)
    public string spawnPointName;   // ID do SpawnPoint na cena destino

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

        // Guarda o spawn para a próxima cena
        PlayerSpawnManager.nextSpawnPoint = spawnPointName;

        SceneManager.LoadScene(sceneToLoad);

        yield return null;

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.StartCoroutine(ScreenFader.Instance.FadeIn());

        isTransitioning = false;
    }
}
