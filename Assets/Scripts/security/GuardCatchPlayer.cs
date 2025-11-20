using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardCatchPlayer : MonoBehaviour
{
    [Header("Detecção")]
    [SerializeField] private string playerTag = "Player";

    [Header("Tela de DIA 2")]
    [SerializeField] private GameObject dia2Panel;   // panel preto com o texto "DIA 2"
    [SerializeField] private float delayBeforeLoad = 2f;
    [SerializeField] private string sceneToLoad = "quarto";

    // para evitar disparar a transição várias vezes
    private static bool isTransitioning = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTransitioning) return;

        if (collision.collider.CompareTag(playerTag))
        {
            StartCoroutine(HandleCatch());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag(playerTag))
        {
            StartCoroutine(HandleCatch());
        }
    }

    private IEnumerator HandleCatch()
    {
        isTransitioning = true;

        // Mostra a tela "DIA 2"
        if (dia2Panel != null)
            dia2Panel.SetActive(true);

        // Configura para que o próximo dia comece às 8h
        GameTimeState.SetDay2();

        // Espera um tempinho com a tela preta
        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        // Carrega o quarto (dia reinicia lá)
        SceneManager.LoadScene(sceneToLoad);
    }
}
