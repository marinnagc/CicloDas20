using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardCatchPlayer : MonoBehaviour
{
    [Header("Detecção")]
    [SerializeField] private string playerTag = "Player";

    [Header("Tela DIA 2")]
    [SerializeField] private GameObject dia2Panel;   // UIManager/PanelDia2
    [SerializeField] private float delayBeforeLoad = 2f;
    [SerializeField] private string sceneToLoad = "quarto";

    private static bool isTransitioning = false;

    private void Start()
    {
        // garante que começa desligado
        if (dia2Panel != null)
            dia2Panel.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;
        StartTransition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        StartTransition();
    }

    private void StartTransition()
    {
        if (isTransitioning) return;
        StartCoroutine(ShowDia2AndLoad());
    }

    private IEnumerator ShowDia2AndLoad()
    {
        isTransitioning = true;

        // Mostra a tela preta DIA 2 acima de tudo
        if (dia2Panel != null)
        {
            dia2Panel.SetActive(true);
            dia2Panel.transform.SetAsLastSibling(); // joga para o topo do Canvas
        }

        // Próximo dia começa às 8h
        GameTimeState.SetDay2();

        // pausa o jogo enquanto mostra a tela (mas usamos WaitForSecondsRealtime)
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(delayBeforeLoad);
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }
}
